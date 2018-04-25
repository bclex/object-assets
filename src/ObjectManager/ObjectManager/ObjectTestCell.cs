using OA.Core;
using UnityEngine;

namespace OA
{
    public static class ObjectTestCell
    {
        static IAssetPack Asset;
        static IDataPack Data;
        static ICellManager CellManager;
        static TemporalLoadBalancer LoadBalancer = new TemporalLoadBalancer();
        static GameObject PlayerPrefab;

        public static void Awake()
        {
            Utils.InUnity = true;
            PlayerPrefab = GameObject.Find("Cube00");
        }

        static GameObject _sunObj;

        public static void Start()
        {
            var assetUri = "game://Morrowind/Morrowind.bsa";
            var dataUri = "game://Morrowind/Morrowind.esm";

            var assetManager = AssetManager.GetAssetManager(EngineId.Tes);
            Asset = assetManager.GetAssetPack(assetUri).Result;
            Data = assetManager.GetDataPack(dataUri).Result;
            CellManager = assetManager.GetCellManager(Asset, Data, LoadBalancer);

            // ambient
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientIntensity = 1.5f;

            // sun
            _sunObj = GameObjectUtils.CreateDirectionalLight(Vector3.zero, Quaternion.Euler(new Vector3(50, 330, 0)));
            _sunObj.GetComponent<Light>().shadows = LightShadows.Soft;
            _sunObj.SetActive(false);

            Cursor.SetCursor(Asset.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);

            // engine
            SpawnPlayerOutside(PlayerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
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
            LoadBalancer.RunTasks(DesiredWorkTimePerFrame);
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
            LoadBalancer.WaitForTask(cellInfo.objectsCreationCoroutine);
        }
    }
}
