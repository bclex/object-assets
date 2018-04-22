using OA.Core;
using UnityEngine;

namespace OA
{
    public static class ObjectTest
    {
        static IAssetPack Asset;
        static IDataPack Data;
        static ICellManager CellManager;
        static GameObject _playerPrefab;

        public static void Awake()
        {
            Utils.InUnity = true;
            _playerPrefab = GameObject.Find("Cube00");
            Utils.Info($"{_playerPrefab}");
        }

        static GameObject _sunObj;
        public static void Start()
        {
            //var assetUri = "file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Morrowind/Data%20Files/Morrowind.*";
            //var dataUri = "file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Morrowind/Data%20Files/Morrowind.esm#Morrowind";
            //var file2Uri = "file://192.168.1.3/User/_ASSETS/Fallout4/Textures1";
            var assetUri = "game://Morrowind/Morrowind.bsa";
            var dataUri = "game://Morrowind/Morrowind.esm";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";

            Asset = AssetManager.GetAssetPack(EngineId.Tes, assetUri).Result;
            Data = AssetManager.GetDataPack(EngineId.Tes, dataUri).Result;
            CellManager = AssetManager.GetCellManager(EngineId.Tes, Asset, Data);

            //var obj = Asset.CreateObject("meshes/x/ex_common_balcony_01.nif");
            //GameObject.Instantiate(obj);

            // ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientIntensity = 1.5f;

            // sun
            _sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            _sunObj.GetComponent<Light>().shadows = LightShadows.Soft;
            _sunObj.SetActive(false);

            Cursor.SetCursor(Asset.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);

            // engine
            SpawnPlayerOutside(_playerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
        }

        public static void OnDestroy()
        {
            if (Asset != null)
            {
                Asset.Dispose();
                Asset = null;
            }
            if (Data != null)
            {
                Data.Dispose();
                Data = null;
            }
        }

        const float DesiredWorkTimePerFrame = 1.0f / 200;
        public static void Update()
        {
            // The current cell can be null if the player is outside of the defined game world.
            if (_currentCell == null || !_currentCell.IsInterior)
                CellManager.UpdateExteriorCells(_playerCameraObj.transform.position);
            AssetManager.RunTasks(DesiredWorkTimePerFrame);
        }

        static ICellRecord _currentCell;
        static GameObject _playerCameraObj;

        private static GameObject CreatePlayer(GameObject playerPrefab, Vector3 position, out GameObject playerCamera)
        {
            var player = GameObject.FindWithTag("Player");
            if (player == null)
            {
                player = GameObject.Instantiate(playerPrefab);
                player.name = "Player";
            }
            player.transform.position = position;
            //_playerTransform = player.GetComponent<Transform>();
            playerCamera = player.GetComponentInChildren<Camera>().gameObject;
            return player;
        }

        private static void SpawnPlayerOutside(GameObject playerPrefab, Vector2i gridCoords, Vector3 position)
        {
            _currentCell = Data.FindExteriorCellRecord(gridCoords);
            Utils.Assert(_currentCell != null);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            var cellInfo = CellManager.StartCreatingExteriorCell(gridCoords);
            AssetManager.WaitForTask(cellInfo.objectsCreationCoroutine);
        }
    }
}
