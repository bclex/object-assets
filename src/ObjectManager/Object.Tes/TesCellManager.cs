using OA.Tes.FilePacks;
using OA.Tes.Formats;
using OA.Core;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OA.Tes.FilePacks.Records;

namespace OA.Tes
{
    public class TesCellManager : ICellManager
    {
        const int _cellRadius = 1; //4;
        const int _detailRadius = 1; //3;
        const string _defaultLandTextureFilePath = "textures/_land_default.dds";

        TesAssetPack _asset;
        TesDataPack _data;
        TemporalLoadBalancer _loadBalancer;
        Dictionary<Vector3Int, InRangeCellInfo> _cellObjects = new Dictionary<Vector3Int, InRangeCellInfo>();

        public TesCellManager(TesAssetPack asset, TesDataPack data, TemporalLoadBalancer loadBalancer)
        {
            _asset = asset;
            _data = data;
            _loadBalancer = loadBalancer;
        }

        public Vector3Int GetCellId(Vector3 point, int world) => new Vector3Int(Mathf.FloorToInt(point.x / ConvertUtils.ExteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / ConvertUtils.ExteriorCellSideLengthInMeters), world);

        public InRangeCellInfo StartCreatingCell(Vector3Int cellId)
        {
            var cell = _data.FindCellRecord(cellId);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                _cellObjects[cellId.z != -1 ? cellId : Vector3Int.zero] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public InRangeCellInfo StartCreatingCellByName(int world, int id, string name)
        {
            if (world != -1)
                throw new System.ArgumentOutOfRangeException("world");
            var cell = _data.FindCellRecordByName(world, id, name);
            if (cell != null)
            {
                var cellInfo = StartInstantiatingCell(cell);
                _cellObjects[Vector3Int.zero] = cellInfo;
                return cellInfo;
            }
            return null;
        }

        public void UpdateCells(Vector3 currentPosition, int world, bool immediate = false, int cellRadiusOverride = -1)
        {
            var cameraCellId = GetCellId(currentPosition, world);

            var cellRadius = cellRadiusOverride >= 0 ? cellRadiusOverride : _cellRadius;
            var minCellX = cameraCellId.x - cellRadius;
            var maxCellX = cameraCellId.x + cellRadius;
            var minCellY = cameraCellId.y - cellRadius;
            var maxCellY = cameraCellId.y + cellRadius;

            // Destroy out of range cells.
            var outOfRangeCellIds = new List<Vector3Int>();
            foreach (var x in _cellObjects)
                if (x.Key.x < minCellX || x.Key.x > maxCellX || x.Key.y < minCellY || x.Key.y > maxCellY)
                    outOfRangeCellIds.Add(x.Key);
            foreach (var cellId in outOfRangeCellIds)
                DestroyCell(cellId);

            // Create new cells.
            for (var r = 0; r <= cellRadius; r++)
                for (var x = minCellX; x <= maxCellX; x++)
                    for (var y = minCellY; y <= maxCellY; y++)
                    {
                        var cellId = new Vector3Int(x, y, world);
                        var cellXDistance = Mathf.Abs(cameraCellId.x - cellId.x);
                        var cellYDistance = Mathf.Abs(cameraCellId.y - cellId.y);
                        var cellDistance = Mathf.Max(cellXDistance, cellYDistance);
                        if (cellDistance == r && !_cellObjects.ContainsKey(cellId))
                        {
                            var cellInfo = StartCreatingCell(cellId);
                            if (cellInfo != null && immediate)
                                _loadBalancer.WaitForTask(cellInfo.ObjectsCreationCoroutine);
                        }
                    }

            // Update LODs.
            foreach (var x in _cellObjects)
            {
                var cellId = x.Key;
                var cellInfo = x.Value;
                var cellXDistance = Mathf.Abs(cameraCellId.x - cellId.x);
                var cellYDistance = Mathf.Abs(cameraCellId.y - cellId.y);
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

        public InRangeCellInfo StartInstantiatingCell(CELLRecord cell)
        {
            Debug.Assert(cell != null);
            string cellObjName = null;
            LANDRecord land = null;
            if (!cell.IsInterior)
            {
                cellObjName = "cell " + cell.GridId.ToString();
                land = _data.FindLANDRecord(cell.GridId);
            }
            else cellObjName = cell.EDID.Value;
            var cellObj = new GameObject(cellObjName) { tag = "Cell" };
            var cellObjectsContainer = new GameObject("objects");
            cellObjectsContainer.transform.parent = cellObj.transform;
            var cellObjectsCreationCoroutine = InstantiateCellObjectsCoroutine(cell, land, cellObj, cellObjectsContainer);
            _loadBalancer.AddTask(cellObjectsCreationCoroutine);
            return new InRangeCellInfo(cellObj, cellObjectsContainer, cell, cellObjectsCreationCoroutine);
        }

        void DestroyCell(Vector3Int cellId)
        {
            if (_cellObjects.TryGetValue(cellId, out InRangeCellInfo cellInfo))
            {
                _loadBalancer.CancelTask(cellInfo.ObjectsCreationCoroutine);
                Object.Destroy(cellInfo.GameObject);
                _cellObjects.Remove(cellId);
            }
            else Utils.Error("Tried to destroy a cell that isn't created.");
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
        IEnumerator InstantiateCellObjectsCoroutine(CELLRecord cell, LANDRecord land, GameObject cellObj, GameObject cellObjectsContainer)
        {
            // Start pre-loading all required textures for the terrain.
            if (land != null)
            {
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
            // Instantiate objects.
            foreach (var refCellObjInfo in refCellObjInfos)
            {
                InstantiateCellObject(cell, cellObjectsContainer, refCellObjInfo);
                yield return null;
            }
        }

        RefCellObjInfo[] GetRefCellObjInfos(CELLRecord cell)
        {
            if (_data.Format != GameFormatId.TES3) return new RefCellObjInfo[0];
            var refCellObjInfos = new RefCellObjInfo[cell.RefObjs.Count];
            for (var i = 0; i < cell.RefObjs.Count; i++)
            {
                var refObjInfo = new RefCellObjInfo
                {
                    RefObj = cell.RefObjs[i]
                };
                // Get the record the RefObjDataGroup references.
                var refObj = (CELLRecord.RefObj)refObjInfo.RefObj;
                _data._MANYsById.TryGetValue(refObj.EDID.Value, out refObjInfo.ReferencedRecord);
                if (refObjInfo.ReferencedRecord != null)
                {
                    var modelFileName = (refObjInfo.ReferencedRecord is IHaveMODL modl ? modl.MODL.Value : null);
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
        void InstantiateCellObject(CELLRecord cell, GameObject parent, RefCellObjInfo refCellObjInfo)
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
            else Utils.Log("Unknown Object: " + ((CELLRecord.RefObj)refCellObjInfo.RefObj).EDID.Value);
        }

        GameObject InstantiateLight(LIGHRecord LIGH, bool indoors)
        {
            var game = TesSettings.Game;
            var lightObj = new GameObject("Light") { isStatic = true };
            var lightComponent = lightObj.AddComponent<Light>();
            lightComponent.range = 3 * (LIGH.DATA.Radius / ConvertUtils.MeterInUnits);
            lightComponent.color = LIGH.DATA.LightColor.ToColor32();
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
        void PostProcessInstantiatedCellObject(GameObject gameObject, RefCellObjInfo refCellObjInfo)
        {
            var refObj = (CELLRecord.RefObj)refCellObjInfo.RefObj;
            // Handle object transforms.
            if (refObj.XSCL != null)
                gameObject.transform.localScale = Vector3.one * refObj.XSCL.Value.Value;
            gameObject.transform.position += NifUtils.NifPointToUnityPoint(refObj.DATA.Position.ToVector3());
            gameObject.transform.rotation *= NifUtils.NifEulerAnglesToUnityQuaternion(refObj.DATA.EulerAngles.ToVector3());
            var tagTarget = gameObject;
            var coll = gameObject.GetComponentInChildren<Collider>(); // if the collider is on a child object and not on the object with the component, we need to set that object's tag instead.
            if (coll != null)
                tagTarget = coll.gameObject;
            ProcessObjectType<DOORRecord>(tagTarget, refCellObjInfo, "Door");
            ProcessObjectType<ACTIRecord>(tagTarget, refCellObjInfo, "Activator");
            ProcessObjectType<CONTRecord>(tagTarget, refCellObjInfo, "Container");
            ProcessObjectType<LIGHRecord>(tagTarget, refCellObjInfo, "Light");
            ProcessObjectType<LOCKRecord>(tagTarget, refCellObjInfo, "Lock");
            ProcessObjectType<PROBRecord>(tagTarget, refCellObjInfo, "Probe");
            ProcessObjectType<REPARecord>(tagTarget, refCellObjInfo, "RepairTool");
            ProcessObjectType<WEAPRecord>(tagTarget, refCellObjInfo, "Weapon");
            ProcessObjectType<CLOTRecord>(tagTarget, refCellObjInfo, "Clothing");
            ProcessObjectType<ARMORecord>(tagTarget, refCellObjInfo, "Armor");
            ProcessObjectType<INGRRecord>(tagTarget, refCellObjInfo, "Ingredient");
            ProcessObjectType<ALCHRecord>(tagTarget, refCellObjInfo, "Alchemical");
            ProcessObjectType<APPARecord>(tagTarget, refCellObjInfo, "Apparatus");
            ProcessObjectType<BOOKRecord>(tagTarget, refCellObjInfo, "Book");
            ProcessObjectType<MISCRecord>(tagTarget, refCellObjInfo, "MiscObj");
            ProcessObjectType<CREARecord>(tagTarget, refCellObjInfo, "Creature");
            ProcessObjectType<NPC_Record>(tagTarget, refCellObjInfo, "NPC");
        }

        void ProcessObjectType<RecordType>(GameObject gameObject, RefCellObjInfo info, string tag) where RecordType : Record
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

        List<string> GetLANDTextureFilePaths(LANDRecord land)
        {
            // Don't return anything if the LAND doesn't have height data or texture data.
            if (land.VTEX == null) return null;
            var textureFilePaths = new List<string>();
            var distinctTextureIndices = land.VTEX.Value.TextureIndicesT3.Distinct().ToList();
            for (var i = 0; i < distinctTextureIndices.Count; i++)
            {
                var textureIndex = ((short)distinctTextureIndices[i] - 1);
                if (textureIndex < 0)
                {
                    textureFilePaths.Add(_defaultLandTextureFilePath);
                    continue;
                }
                var ltex = _data.FindLTEXRecord(textureIndex);
                var textureFilePath = ltex.ICON.Value;
                textureFilePaths.Add(textureFilePath);
            }
            return textureFilePaths;
        }

        /// <summary>
        /// Creates terrain representing a LAND record.
        /// </summary>
        IEnumerator InstantiateLANDCoroutine(LANDRecord land, GameObject parent)
        {
            Debug.Assert(land != null);
            // Don't create anything if the LAND doesn't have height data.
            if (land.VHGT.HeightData == null)
                yield break;
            // Return before doing any work to provide an IEnumerator handle to the coroutine.
            yield return null;
            const int LAND_SIDELENGTH_IN_SAMPLES = 65;
            var heights = new float[LAND_SIDELENGTH_IN_SAMPLES, LAND_SIDELENGTH_IN_SAMPLES];
            // Read in the heights in Morrowind units.
            const int VHGTIncrementToUnits = 8;
            var rowOffset = land.VHGT.ReferenceHeight;
            for (var y = 0; y < LAND_SIDELENGTH_IN_SAMPLES; y++)
            {
                rowOffset += land.VHGT.HeightData[y * LAND_SIDELENGTH_IN_SAMPLES];
                heights[y, 0] = rowOffset * VHGTIncrementToUnits;
                var colOffset = rowOffset;
                for (var x = 1; x < LAND_SIDELENGTH_IN_SAMPLES; x++)
                {
                    colOffset += land.VHGT.HeightData[(y * LAND_SIDELENGTH_IN_SAMPLES) + x];
                    heights[y, x] = colOffset * VHGTIncrementToUnits;
                }
            }
            // Change the heights to percentages.
            heights.GetExtrema(out var minHeight, out var maxHeight);
            for (var y = 0; y < LAND_SIDELENGTH_IN_SAMPLES; y++)
                for (var x = 0; x < LAND_SIDELENGTH_IN_SAMPLES; x++)
                    heights[y, x] = Utils.ChangeRange(heights[y, x], minHeight, maxHeight, 0, 1);
            
            // Texture the terrain.
            SplatPrototype[] splatPrototypes = null;
            const int LAND_TEXTUREINDICES = 256;
            var textureIndices = land.VTEX != null ? land.VTEX.Value.TextureIndicesT3 : new ushort[LAND_TEXTUREINDICES];
            // Create splat prototypes.
            var splatPrototypeList = new List<SplatPrototype>();
            var texInd2SplatInd = new Dictionary<ushort, int>();
            for (var i = 0; i < textureIndices.Length; i++)
            {
                var textureIndex = (int)(textureIndices[i] - 1);
                if (!texInd2SplatInd.ContainsKey((ushort)textureIndex))
                {
                    // Load terrain texture.
                    string textureFilePath;
                    if (textureIndex < 0)
                        textureFilePath = _defaultLandTextureFilePath;
                    else
                    {
                        var LTEX = _data.FindLTEXRecord(textureIndex);
                        textureFilePath = LTEX.ICON.Value;
                    }
                    var texture = _asset.LoadTexture(textureFilePath);
                    // Yield after loading each texture to avoid doing too much work on one frame.
                    yield return null;
                    // Create the splat prototype.
                    var splat = new SplatPrototype
                    {
                        texture = texture,
                        smoothness = 0,
                        metallic = 0,
                        tileSize = new Vector2(6, 6)
                    };
                    // Update collections.
                    var splatIndex = splatPrototypeList.Count;
                    splatPrototypeList.Add(splat);
                    texInd2SplatInd.Add((ushort)textureIndex, splatIndex);
                }
            }
            splatPrototypes = splatPrototypeList.ToArray();

            // Create the alpha map.
            var VTEX_ROWS = 16;
            var VTEX_COLUMNS = VTEX_ROWS;
            float[,,] alphaMap = null;
            alphaMap = new float[VTEX_ROWS, VTEX_COLUMNS, splatPrototypes.Length];
            for (var y = 0; y < VTEX_ROWS; y++)
            {
                var yMajor = y / 4;
                var yMinor = y - (yMajor * 4);
                for (var x = 0; x < VTEX_COLUMNS; x++)
                {
                    var xMajor = x / 4;
                    var xMinor = x - (xMajor * 4);
                    var texIndex = ((short)textureIndices[(yMajor * 64) + (xMajor * 16) + (yMinor * 4) + xMinor] - 1);
                    if (texIndex >= 0) { var splatIndex = texInd2SplatInd[(ushort)texIndex]; alphaMap[y, x, splatIndex] = 1; }
                    else alphaMap[y, x, 0] = 1;
                }
            }

            // Yield before creating the terrain GameObject because it takes a while.
            yield return null;
            
            // Create the terrain.
            var heightRange = maxHeight - minHeight;
            var terrainPosition = new Vector3(ConvertUtils.ExteriorCellSideLengthInMeters * land.GridId.x, minHeight / ConvertUtils.MeterInUnits, ConvertUtils.ExteriorCellSideLengthInMeters * land.GridId.y);
            var heightSampleDistance = ConvertUtils.ExteriorCellSideLengthInMeters / (LAND_SIDELENGTH_IN_SAMPLES - 1);
            var terrain = GameObjectUtils.CreateTerrain(-1, heights, heightRange / ConvertUtils.MeterInUnits, heightSampleDistance, splatPrototypes, alphaMap, terrainPosition);
            terrain.GetComponent<Terrain>().materialType = Terrain.MaterialType.BuiltInLegacyDiffuse;
            terrain.transform.parent = parent.transform;
            terrain.isStatic = true;
        }
    }
}
