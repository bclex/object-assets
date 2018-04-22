using OA.Components;
using OA.Configuration;
using OA.Core;
using OA.Tes.Components;
using OA.Tes.Components.Records;
using OA.Tes.FilePacks;
using OA.Tes.FilePacks.Records;
using OA.Tes.UI;
using System;
using System.IO;
using UnityEngine;

namespace OA.Tes
{
    public class TesEngine
    {
        const float DesiredWorkTimePerFrame = 1.0f / 200;
        const int CellRadiusOnLoad = 2;
        const float MaxInteractDistance = 3;
        public static TesEngine instance;

        internal TesAssetPack _asset;
        internal TesDataPack _data;
        TesUIManager _uiManager;
        internal CellManager _cellManager;
        TemporalLoadBalancer _temporalLoadBalancer;
        GameObject _sunObj;

        public TesEngine(TesAssetPack asset, TesDataPack data, TesUIManager uiManager)
        {
            _asset = asset;
            _data = data;
            _uiManager = uiManager;
            _uiManager.Active = true;
            _temporalLoadBalancer = new TemporalLoadBalancer();
            _cellManager = new CellManager(asset, data, _temporalLoadBalancer);
            var game = BaseSettings.Game;

            // ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientIntensity = game.AmbientIntensity;

            // sun
            _sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            _sunObj.GetComponent<Light>().shadows = game.RenderSunShadows ? LightShadows.Soft : LightShadows.None;
            _sunObj.SetActive(false);
            if (game.DayNightCycle)
                _sunObj.AddComponent<DayNightCycle>();

            //// water
            //_waterObj = GameObject.Instantiate(TesGame.instance.WaterPrefab);
            //_waterObj.SetActive(false);
            //var water = _waterObj.GetComponent<Water>();
            //water.waterMode = TesGame.instance.WaterQuality;
            //if (!TesGame.instance.WaterBackSideTransparent)
            //{
            //    var side = _waterObj.transform.GetChild(0);
            //    var sideMaterial = side.GetComponent<Renderer>().sharedMaterial;
            //    sideMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.One);
            //    sideMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.Zero);
            //    sideMaterial.SetInt("_ZWrite", 1);
            //    sideMaterial.DisableKeyword("_ALPHATEST_ON");
            //    sideMaterial.DisableKeyword("_ALPHABLEND_ON");
            //    sideMaterial.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            //    sideMaterial.renderQueue = -1;
            //}

            Cursor.SetCursor(_asset.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);
        }

        #region Player Spawn

        CELLRecord _currentCell;
        Transform _playerTransform;
        PlayerComponent _playerComponent;
        PlayerInventory _playerInventory;
        GameObject _playerCameraObj;

        Color32 _defaultAmbientColor = new Color32(137, 140, 160, 255);
        //GameObject _waterObj;
        //UnderwaterEffect _underwaterEffect;

        private GameObject CreatePlayer(GameObject playerPrefab, Vector3 position, out GameObject playerCamera)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = GameObject.Instantiate(playerPrefab);
                player.name = "Player";
            }
            player.transform.position = position;
            _playerTransform = player.GetComponent<Transform>();
            playerCamera = player.GetComponentInChildren<Camera>().gameObject;
            _playerComponent = player.GetComponent<PlayerComponent>();
            _playerInventory = player.GetComponent<PlayerInventory>();
            //_underwaterEffect = playerCamera.GetComponent<UnderwaterEffect>();
            return player;
        }

        /// <summary>
        /// Spawns the player inside. Be carefull, the name of the cell is not the same for each languages.
        /// Use it with the correct name.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="interiorCellName">The name of the desired cell.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerInside(GameObject playerPrefab, string interiorCellName, Vector3 position)
        {
            _currentCell = _data.FindInteriorCellRecord(interiorCellName);
            Debug.Assert(_currentCell != null);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            var cellInfo = _cellManager.StartCreatingInteriorCell(interiorCellName);
            _temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
            OnInteriorCell(_currentCell);
        }

        /// <summary>
        /// Spawns the player inside using the cell's grid coordinates.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="gridCoords">The grid coordinates.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerInside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
        {
            _currentCell = _data.FindInteriorCellRecord(gridCoords);
            Debug.Assert(_currentCell != null);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            var cellInfo = _cellManager.StartCreatingInteriorCell(gridCoords);
            _temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
            OnInteriorCell(_currentCell);
        }

        /// <summary>
        /// Spawns the player outside using the cell's grid coordinates.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="gridCoords">The grid coordinates.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerOutside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
        {
            _currentCell = _data.FindExteriorCellRecord(gridCoords);
            Debug.Assert(_currentCell != null);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            var cellInfo = _cellManager.StartCreatingExteriorCell(gridCoords);
            _temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
            OnExteriorCell(_currentCell);
        }

        /// <summary>
        /// Spawns the player outside using the position of the player.
        /// </summary>
        /// <param name="playerPrefab">The player prefab.</param>
        /// <param name="position">The target position of the player.</param>
        public void SpawnPlayerOutside(GameObject playerPrefab, Vector3 position)
        {
            var cellIndices = _cellManager.GetExteriorCellIndices(position);
            _currentCell = _data.FindExteriorCellRecord(cellIndices);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            _cellManager.UpdateExteriorCells(_playerCameraObj.transform.position, true, CellRadiusOnLoad);
            OnExteriorCell(_currentCell);
        }

        private void OnExteriorCell(CELLRecord CELL)
        {
            RenderSettings.ambientLight = _defaultAmbientColor;
            _sunObj.SetActive(true);
            //_waterObj.transform.position = Vector3.zero;
            //_waterObj.SetActive(true);
            //_underwaterEffect.enabled = true;
            //_underwaterEffect.Level = 0.0f;
        }

        private void OnInteriorCell(CELLRecord CELL)
        {
            if (CELL.AMBI != null)
                RenderSettings.ambientLight = ColorUtils.B8G8R8ToColor32(CELL.AMBI.ambientColor);
            _sunObj.SetActive(false);
            //_underwaterEffect.enabled = CELL.WHGT != null;
            //if (CELL.WHGT != null)
            //{
            //    var offset = 1.6f; // Interiors cells needs this offset to render at the correct location.
            //    _waterObj.transform.position = new Vector3(0, (CELL.WHGT.value / Convert.meterInMWUnits) - offset, 0);
            //    _waterObj.SetActive(true);
            //    _underwaterEffect.Level = _waterObj.transform.position.y;
            //}
            //else _waterObj.SetActive(false);
        }

        #endregion

        public void Update()
        {
            // The current cell can be null if the player is outside of the defined game world.
            if (_currentCell == null || !_currentCell.IsInterior)
                _cellManager.UpdateExteriorCells(_playerCameraObj.transform.position);
            _temporalLoadBalancer.RunTasks(DesiredWorkTimePerFrame);
            CastInteractRay();
        }

        #region Raycast

        RaycastHit[] _interactRaycastHitBuffer = new RaycastHit[32];
        public void CastInteractRay()
        {
            // Cast a ray to see what the camera is looking at.
            var ray = new Ray(_playerCameraObj.transform.position, _playerCameraObj.transform.forward);
            var raycastHitCount = Physics.RaycastNonAlloc(ray, _interactRaycastHitBuffer, MaxInteractDistance);
            if (raycastHitCount > 0 && !_playerComponent.Paused)
            {
                for (var i = 0; i < raycastHitCount; i++)
                {
                    var hitInfo = _interactRaycastHitBuffer[i];
                    var component = hitInfo.collider.GetComponentInParent<GenericObjectComponent>();
                    if (component != null)
                    {
                        if (string.IsNullOrEmpty(component.objData.name))
                            return;
                        ShowInteractiveText(component);
                        if (InputManager.GetButtonDown("Use"))
                        {
                            if (component is DoorComponent) OpenDoor((DoorComponent)component);
                            else if (component.usable) component.Interact();
                            else if (component.pickable) _playerInventory.Add(component);
                        }
                        break;
                    }
                    else CloseInteractiveText(); //deactivate text if no interactable [ DOORS ONLY - REQUIRES EXPANSION ] is found
                }
            }
            else CloseInteractiveText(); //deactivate text if nothing is raycasted against
        }

        public void ShowInteractiveText(GenericObjectComponent component)
        {
            var data = component.objData;
            _uiManager.InteractiveText.Show(GUIUtils.CreateSprite(data.icon), data.interactionPrefix, data.name, data.value, data.weight);
        }

        public void CloseInteractiveText()
        {
            _uiManager.InteractiveText.Close();
        }

        private void OpenDoor(DoorComponent component)
        {
            if (!component.doorData.leadsToAnotherCell)
                component.Interact();
            else
            {
                // The door leads to another cell, so destroy all currently loaded cells.
                _cellManager.DestroyAllCells();
                // Move the player.
                _playerTransform.position = component.doorData.doorExitPos;
                _playerTransform.localEulerAngles = new Vector3(0, component.doorData.doorExitOrientation.eulerAngles.y, 0);
                // Load the new cell.
                CELLRecord newCell;
                if (component.doorData.leadsToInteriorCell)
                {
                    var cellInfo = _cellManager.StartCreatingInteriorCell(component.doorData.doorExitName);
                    _temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
                    newCell = (CELLRecord)cellInfo.cellRecord;
                    OnInteriorCell(newCell);
                }
                else
                {
                    var cellIndices = _cellManager.GetExteriorCellIndices(component.doorData.doorExitPos);
                    newCell = _data.FindExteriorCellRecord(cellIndices);
                    _cellManager.UpdateExteriorCells(_playerCameraObj.transform.position, true, CellRadiusOnLoad);
                    OnExteriorCell(newCell);
                }
                _currentCell = newCell;
            }
        }
        #endregion

        public void TestAllCells(string resultsFilePath)
        {
            using (var w = new StreamWriter(resultsFilePath))
                foreach (var record in _data.GetRecordsOfType<CELLRecord>())
                {
                    var CELL = (CELLRecord)record;
                    try
                    {
                        var cellInfo = _cellManager.StartInstantiatingCell(CELL);
                        _temporalLoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
                        //DestroyImmediate(cellInfo.gameObject);
                        w.Write("Pass: ");
                    }
                    catch (Exception e) { w.Write($"Fail: {e.Message}"); }
                    if (!CELL.IsInterior)
                        w.WriteLine(CELL.GridCoords.ToString());
                    else w.WriteLine(CELL.NAME.value);
                    w.Flush();
                }
        }
    }
}
