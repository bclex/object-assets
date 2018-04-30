using OA.Core;
using OA.Ultima.FilePacks;
using OA.Ultima.FilePacks.Records;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OA.Ultima
{
    public class UltimaCellManager : ICellManager
    {
        const int _cellRadius = 2;
        const int _detailRadius = 1;

        UltimaAssetPack _asset;
        UltimaDataPack _data;
        TemporalLoadBalancer _loadBalancer;
        Dictionary<Vector2i, InRangeCellInfo> _cellObjects = new Dictionary<Vector2i, InRangeCellInfo>();

        public UltimaCellManager(UltimaAssetPack asset, UltimaDataPack data, TemporalLoadBalancer loadBalancer)
        {
            _asset = asset;
            _data = data;
            _loadBalancer = loadBalancer;
        }

        public Vector2i GetExteriorCellIndices(Vector3 point)
        {
            return new Vector2i(Mathf.FloorToInt(point.x / ConvertUtils.ExteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / ConvertUtils.ExteriorCellSideLengthInMeters));
        }

        public InRangeCellInfo StartCreatingExteriorCell(Vector2i cellIndices)
        {
            var cell = _data.FindExteriorCellRecord(cellIndices);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                _cellObjects[cellIndices] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public void UpdateExteriorCells(Vector3 currentPosition, bool immediate = false, int cellRadiusOverride = -1)
        {
            var cameraCellIndices = GetExteriorCellIndices(currentPosition);

            var cellRadius = cellRadiusOverride >= 0 ? cellRadiusOverride : _cellRadius;
            var minCellX = cameraCellIndices.x - cellRadius;
            var maxCellX = cameraCellIndices.x + cellRadius;
            var minCellY = cameraCellIndices.y - cellRadius;
            var maxCellY = cameraCellIndices.y + cellRadius;

            // Destroy out of range cells.
            var outOfRangeCellIndices = new List<Vector2i>();
            foreach (var x in _cellObjects)
                if (x.Key.x < minCellX || x.Key.x > maxCellX || x.Key.y < minCellY || x.Key.y > maxCellY)
                    outOfRangeCellIndices.Add(x.Key);
            foreach (var cellIndices in outOfRangeCellIndices)
                DestroyExteriorCell(cellIndices);

            // Create new cells.
            for (var r = 0; r <= cellRadius; r++)
                for (var x = minCellX; x <= maxCellX; x++)
                    for (var y = minCellY; y <= maxCellY; y++)
                    {
                        var cellIndices = new Vector2i(x, y);
                        var cellXDistance = Mathf.Abs(cameraCellIndices.x - cellIndices.x);
                        var cellYDistance = Mathf.Abs(cameraCellIndices.y - cellIndices.y);
                        var cellDistance = Mathf.Max(cellXDistance, cellYDistance);
                        if (cellDistance == r && !_cellObjects.ContainsKey(cellIndices))
                        {
                            var cellInfo = StartCreatingExteriorCell(cellIndices);
                            if (cellInfo != null && immediate)
                                _loadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
                        }
                    }

            // Update LODs.
            foreach (var x in _cellObjects)
            {
                var cellIndices = x.Key;
                var cellInfo = x.Value;
                var cellXDistance = Mathf.Abs(cameraCellIndices.x - cellIndices.x);
                var cellYDistance = Mathf.Abs(cameraCellIndices.y - cellIndices.y);
                var cellDistance = Mathf.Max(cellXDistance, cellYDistance);
                if (cellDistance <= _detailRadius)
                {
                    if (!cellInfo.ObjectsContainerGameObject.activeSelf)
                        cellInfo.ObjectsContainerGameObject.SetActive(true);
                }
                else
                {
                    if (cellInfo.ObjectsContainerGameObject.activeSelf)
                        cellInfo.ObjectsContainerGameObject.SetActive(false);
                }
            }
        }

        public InRangeCellInfo StartCreatingInteriorCell(string cellName)
        {
            var cell = _data.FindInteriorCellRecord(cellName);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                _cellObjects[Vector2i.zero] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public InRangeCellInfo StartCreatingInteriorCell(Vector2i gridCoords)
        {
            var cell = _data.FindInteriorCellRecord(gridCoords);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                _cellObjects[Vector2i.zero] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public InRangeCellInfo StartInstantiatingCell(CELLRecord cell)
        {
            Debug.Assert(cell != null);
            string cellObjName = null;
            LANDRecord land = null;
            if (!cell.IsInterior)
            {
                cellObjName = "cell " + cell.GridCoords.ToString();
                land = _data.FindLANDRecord(cell.GridCoords);
            }
            else cellObjName = cell.Name;
            var cellObj = new GameObject(cellObjName) { tag = "Cell" };
            var cellObjectsContainer = new GameObject("objects");
            cellObjectsContainer.transform.parent = cellObj.transform;
            var cellObjectsCreationCoroutine = InstantiateCellObjectsCoroutine(cell, land, cellObj, cellObjectsContainer);
            _loadBalancer.AddTask(cellObjectsCreationCoroutine);
            return new InRangeCellInfo(cellObj, cellObjectsContainer, cell, cellObjectsCreationCoroutine);
        }

        public void DestroyAllCells()
        {
            foreach (var x in _cellObjects)
            {
                _loadBalancer.CancelTask(x.Value.ObjectsCreationCoroutine);
                Object.Destroy(x.Value.GameObject);
            }
            _cellObjects.Clear();
        }

        /// <summary>
        /// A coroutine that instantiates the terrain for, and all objects in, a cell.
        /// </summary>
        private IEnumerator InstantiateCellObjectsCoroutine(CELLRecord cell, LANDRecord land, GameObject cellObj, GameObject cellObjectsContainer)
        {
            cell.Load();
            // Start pre-loading all required textures for the terrain.
            if (land != null)
            {
                land.Load();
                var landTextureFilePaths = GetLANDTextureFilePaths(land);
                if (landTextureFilePaths != null)
                    foreach (var landTextureFilePath in landTextureFilePaths)
                        _asset.PreloadTextureAsync(landTextureFilePath);
                yield return null;
            }
            // Extract information about referenced objects.
            var refCellObjInfos = GetRefCellObjInfos(cell);
            yield return null;
            // Start pre-loading all required files for referenced objects. The NIF manager will load the textures as well.
            foreach (var refCellObjInfo in refCellObjInfos)
                if (refCellObjInfo.ModelFilePath != null)
                    _asset.PreloadObjectAsync(refCellObjInfo.ModelFilePath);
            yield return null;
            // Instantiate terrain.
            if (land != null)
            {
                var instantiateLANDTaskEnumerator = InstantiateLANDCoroutine(land, cellObj);
                // Run the LAND instantiation coroutine.
                while (instantiateLANDTaskEnumerator.MoveNext())
                    // Yield every time InstantiateLANDCoroutine does to avoid doing too much work in one frame.
                    yield return null;
                // Yield after InstantiateLANDCoroutine has finished to avoid doing too much work in one frame.
                yield return null;
            }
            //// Instantiate objects.
            //foreach (var refCellObjInfo in refCellObjInfos)
            //{
            //    InstantiateCellObject(cell, cellObjectsContainer, refCellObjInfo);
            //    yield return null;
            //}
        }

        private RefCellObjInfo[] GetRefCellObjInfos(CELLRecord cell)
        {
            var refCellObjInfos = new RefCellObjInfo[cell.RefObjs.Length];
            for (var i = 0; i < cell.RefObjs.Length; i++)
            {
                var refObjInfo = new RefCellObjInfo
                {
                    RefObj = cell.RefObjs[i]
                };
                // Get the record the RefObjDataGroup references.
                var refObj = (CELLRecord.RefObj)refObjInfo.RefObj;
                _data.objectsByIDString.TryGetValue(refObj.Name, out refObjInfo.ReferencedRecord);
                if (refObjInfo.ReferencedRecord != null)
                {
                    var modelFileName = RecordUtils.GetModelFileName(refObjInfo.ReferencedRecord);
                    // If the model file name is valid, store the model file path.
                    if (!string.IsNullOrEmpty(modelFileName))
                        refObjInfo.ModelFilePath = "meshes\\" + modelFileName;
                }
                refCellObjInfos[i] = refObjInfo;
            }
            return refCellObjInfos;
        }

        /// <summary>
        /// Instantiates an object in a cell. Called by InstantiateCellObjectsCoroutine after the object's assets have been pre-loaded.
        /// </summary>
        private void InstantiateCellObject(CELLRecord cell, GameObject parent, RefCellObjInfo refCellObjInfo)
        {
            if (refCellObjInfo.ReferencedRecord != null)
            {
                GameObject modelObj = null;
                // If the object has a model, instantiate it.
                if (refCellObjInfo.ModelFilePath != null)
                {
                    modelObj = _asset.CreateObject(refCellObjInfo.ModelFilePath);
                    PostProcessInstantiatedCellObject(modelObj, refCellObjInfo);
                    modelObj.transform.parent = parent.transform;
                }
                // If the object has a light, instantiate it.
                if (refCellObjInfo.ReferencedRecord is LIGHRecord)
                {
                    var lightObj = InstantiateLight((LIGHRecord)refCellObjInfo.ReferencedRecord, cell.IsInterior);
                    // If the object also has a model, parent the model to the light.
                    if (modelObj != null)
                    {
                        // Some NIF files have nodes named "AttachLight". Parent it to the light if it exists.
                        var attachLightObj = GameObjectUtils.FindChildRecursively(modelObj, "AttachLight");
                        if (attachLightObj == null)
                        {
                            //attachLightObj = GameObjectUtils.FindChildWithNameSubstringRecursively(modelObj, "Emitter");
                            attachLightObj = modelObj;
                        }
                        if (attachLightObj != null)
                        {
                            lightObj.transform.position = attachLightObj.transform.position;
                            lightObj.transform.rotation = attachLightObj.transform.rotation;
                            lightObj.transform.parent = attachLightObj.transform;
                        }
                        else // If there is no "AttachLight", center the light in the model's bounds.
                        {
                            lightObj.transform.position = GameObjectUtils.CalcVisualBoundsRecursive(modelObj).center;
                            lightObj.transform.rotation = modelObj.transform.rotation;
                            lightObj.transform.parent = modelObj.transform;
                        }
                    }
                    else // If the light has no associated model, instantiate the light as a standalone object.
                    {
                        PostProcessInstantiatedCellObject(lightObj, refCellObjInfo);
                        lightObj.transform.parent = parent.transform;
                    }
                }
            }
            else Utils.Log("Unknown Object: " + ((CELLRecord.RefObj)refCellObjInfo.RefObj).Name);
        }

        private GameObject InstantiateLight(LIGHRecord LIGH, bool indoors)
        {
            var game = UltimaSettings.Game;
            var lightObj = new GameObject("Light") { isStatic = true };
            var lightComponent = lightObj.AddComponent<Light>();
            lightComponent.range = 3 * (LIGH.Radius / ConvertUtils.MeterInUnits);
            lightComponent.color = new Color32(LIGH.Red, LIGH.Green, LIGH.Blue, 255);
            lightComponent.intensity = 1.5f;
            lightComponent.bounceIntensity = 0f;
            lightComponent.shadows = game.RenderLightShadows ? LightShadows.Soft : LightShadows.None;
            if (!indoors && !game.RenderExteriorCellLights) // disabling exterior cell lights because there is no day/night cycle
                lightComponent.enabled = false;
            return lightObj;
        }

        /// <summary>
        /// Finishes initializing an instantiated cell object.
        /// </summary>
        private void PostProcessInstantiatedCellObject(GameObject gameObject, RefCellObjInfo refCellObjInfo)
        {
            var refObj = (CELLRecord.RefObj)refCellObjInfo.RefObj;
            // Handle object transforms.
            //if (refObj.XSCL != null)
            //    gameObject.transform.localScale = Vector3.one * refObj.XSCL.value;
            //gameObject.transform.position += NifUtils.NifPointToUnityPoint(refObj.DATA.position);
            //gameObject.transform.rotation *= NifUtils.NifEulerAnglesToUnityQuaternion(refObj.DATA.eulerAngles);
            //var tagTarget = gameObject;
            //var coll = gameObject.GetComponentInChildren<Collider>(); // if the collider is on a child object and not on the object with the component, we need to set that object's tag instead.
            //if (coll != null)
            //    tagTarget = coll.gameObject;
            //ProcessObjectType<DOORRecord>(tagTarget, refCellObjInfo, "Door");
            //ProcessObjectType<ACTIRecord>(tagTarget, refCellObjInfo, "Activator");
            //ProcessObjectType<CONTRecord>(tagTarget, refCellObjInfo, "Container");
            //ProcessObjectType<LIGHRecord>(tagTarget, refCellObjInfo, "Light");
            //ProcessObjectType<LOCKRecord>(tagTarget, refCellObjInfo, "Lock");
            //ProcessObjectType<PROBRecord>(tagTarget, refCellObjInfo, "Probe");
            //ProcessObjectType<REPARecord>(tagTarget, refCellObjInfo, "RepairTool");
            //ProcessObjectType<WEAPRecord>(tagTarget, refCellObjInfo, "Weapon");
            //ProcessObjectType<CLOTRecord>(tagTarget, refCellObjInfo, "Clothing");
            //ProcessObjectType<ARMORecord>(tagTarget, refCellObjInfo, "Armor");
            //ProcessObjectType<INGRRecord>(tagTarget, refCellObjInfo, "Ingredient");
            //ProcessObjectType<ALCHRecord>(tagTarget, refCellObjInfo, "Alchemical");
            //ProcessObjectType<APPARecord>(tagTarget, refCellObjInfo, "Apparatus");
            //ProcessObjectType<BOOKRecord>(tagTarget, refCellObjInfo, "Book");
            //ProcessObjectType<MISCRecord>(tagTarget, refCellObjInfo, "MiscObj");
            //ProcessObjectType<CREARecord>(tagTarget, refCellObjInfo, "Creature");
            //ProcessObjectType<NPC_Record>(tagTarget, refCellObjInfo, "NPC");
        }

        private void ProcessObjectType<RecordType>(GameObject gameObject, RefCellObjInfo info, string tag) where RecordType : Record
        {
            var record = info.ReferencedRecord;
            if (record is RecordType)
            {
                var obj = GameObjectUtils.FindTopLevelObject(gameObject);
                if (obj == null) return;
                //var component = GenericObjectComponent.Create(obj, record, tag);
                ////only door records need access to the cell object data group so far
                //if (record is DOORRecord)
                //    ((DoorComponent)component).RefObj = info.RefObj;
            }
        }

        private List<string> GetLANDTextureFilePaths(LANDRecord land)
        {
            // Don't return anything if the LAND doesn't have data.
            if (land.Tiles == null) return null;
            var textureFilePaths = new List<string>();
            var distinctTextureIds = land.Tiles.Select(x => x.TextureId).Distinct().ToList();
            for (var i = 0; i < distinctTextureIds.Count; i++)
            {
                var textureIndex = distinctTextureIds[i];
                textureFilePaths.Add($"tex{textureIndex}");
            }
            return textureFilePaths;
        }

        /// <summary>
        /// Creates terrain representing a LAND record.
        /// </summary>
        private IEnumerator InstantiateLANDCoroutine(LANDRecord land, GameObject parent)
        {
            Debug.Assert(land != null);
            // Don't create anything if the LAND doesn't have data.
            if (land.Tiles == null)
                yield break;
            // Return before doing any work to provide an IEnumerator handle to the coroutine.
            yield return null;
            const int LAND_SIDELENGTH = 8 * DataFile.CELL_PACK;
            var heights = new float[LAND_SIDELENGTH, LAND_SIDELENGTH];
            // Read in the heights in units.
            const int VHGTIncrementToUnits = 1;
            for (var y = 0; y < LAND_SIDELENGTH; y++)
                for (var x = 0; x < LAND_SIDELENGTH; x++)
                {
                    var height = land.Tiles[(y * LAND_SIDELENGTH) + x].Z;
                    heights[y, x] = height * VHGTIncrementToUnits;
                }
            //var rowOffset = 0;
            //for (var y = 0; y < LAND_SIDELENGTH; y++)
            //{
            //    rowOffset += land.Tiles[y * LAND_SIDELENGTH].Z;
            //    heights[y, 0] = rowOffset * VHGTIncrementToUnits;
            //    var colOffset = rowOffset;
            //    for (var x = 1; x < LAND_SIDELENGTH; x++)
            //    {
            //        colOffset += land.Tiles[(y * LAND_SIDELENGTH) + x].Z;
            //        heights[y, x] = colOffset * VHGTIncrementToUnits;
            //    }
            //}

            // Change the heights to percentages.
            ArrayUtils.GetExtrema(heights, out float minHeight, out float maxHeight);
            for (var y = 0; y < LAND_SIDELENGTH; y++)
                for (var x = 0; x < LAND_SIDELENGTH; x++)
                    heights[y, x] = Utils.ChangeRange(heights[y, x], minHeight, maxHeight, 0, 1);
            // Texture the terrain.
            SplatPrototype[] splatPrototypes = null;
            float[,,] alphaMap = null;
            const int LAND_TEXTURES = 64;
            var textureIndices = land.Tiles != null ? land.Tiles.Select(x => x.TextureId).ToArray() : new short[LAND_TEXTURES];
            // Create splat prototypes.
            var splatPrototypeList = new List<SplatPrototype>();
            var texInd2SplatInd = new Dictionary<int, int>();
            for (var i = 0; i < textureIndices.Length; i++)
            {
                var textureIndex = textureIndices[i];
                if (!texInd2SplatInd.ContainsKey(textureIndex))
                {
                    // Load terrain texture.
                    var textureFilePath = $"tex{textureIndex}";
                    var texture = _asset.LoadTexture(textureFilePath);
                    // Yield after loading each texture to avoid doing too much work on one frame.
                    yield return null;
                    // Create the splat prototype.
                    var splat = new SplatPrototype
                    {
                        texture = texture,
                        smoothness = 0,
                        metallic = 0,
                        tileSize = new Vector2(2, 2)
                    };
                    // Update collections.
                    var splatIndex = splatPrototypeList.Count;
                    splatPrototypeList.Add(splat);
                    texInd2SplatInd.Add(textureIndex, splatIndex);
                }
            }
            splatPrototypes = splatPrototypeList.ToArray();
            // Create the alpha map.
            //var VTEX_ROWS = 8;
            //var VTEX_COLUMNS = VTEX_ROWS;
            //alphaMap = new float[VTEX_ROWS, VTEX_COLUMNS, splatPrototypes.Length];
            //for (var y = 0; y < VTEX_ROWS; y++)
            //{
            //    var yMajor = y / 4;
            //    var yMinor = y - (yMajor * 4);
            //    for (var x = 0; x < VTEX_COLUMNS; x++)
            //    {
            //        var xMajor = x / 4;
            //        var xMinor = x - (xMajor * 4);
            //        var texIndex = (short)((short)textureIndices[(yMajor * 64) + (xMajor * 16) + (yMinor * 4) + xMinor] - 1);
            //        if (texIndex >= 0) { var splatIndex = texInd2splatInd[(ushort)texIndex]; alphaMap[y, x, splatIndex] = 1; }
            //        else alphaMap[y, x, 0] = 1;
            //    }
            //}
            // Yield before creating the terrain GameObject because it takes a while.
            yield return null;
            // Create the terrain.
            var heightRange = maxHeight - minHeight;
            var terrainPosition = new Vector3(ConvertUtils.ExteriorCellSideLengthInMeters * land.GridCoords.x, minHeight / ConvertUtils.MeterInUnits, ConvertUtils.ExteriorCellSideLengthInMeters * land.GridCoords.y);
            var heightSampleDistance = ConvertUtils.ExteriorCellSideLengthInMeters / LAND_SIDELENGTH;
            var terrain = GameObjectUtils.CreateTerrain(0, heights, heightRange / ConvertUtils.MeterInUnits, heightSampleDistance, splatPrototypes, alphaMap, terrainPosition);
            terrain.GetComponent<Terrain>().materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
            terrain.transform.parent = parent.transform;
            terrain.isStatic = true;
        }

        private void DestroyExteriorCell(Vector2i indices)
        {
            if (_cellObjects.TryGetValue(indices, out InRangeCellInfo cellInfo))
            {
                _loadBalancer.CancelTask(cellInfo.ObjectsCreationCoroutine);
                Object.Destroy(cellInfo.GameObject);
                _cellObjects.Remove(indices);
            }
            else Utils.Error("Tried to destroy a cell that isn't created.");
        }
    }
}
