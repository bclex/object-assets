using OA.Core;
using UnityEngine;

namespace OA
{
    public static class ObjectTest
    {
        static IAssetPack _asset;
        static IDataPack _data;
        static ICellManager _cellManager;

        public static void Start()
        {
            var assetUri = "file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Morrowind/Data%20Files/Morrowind.*";
            var dataUri = "file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Morrowind/Data%20Files/Morrowind.esm#Morrowind";
            //var file2Uri = "file://192.168.1.3/User/_ASSETS/Fallout4/Textures1";
            //var file3Uri = "game://Morrowind/Morrowind.bsa";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";
            Utils.Info("HERE!");
            var playerPrefab = GameObject.Find("");


            _asset = AssetManager.GetAssetPack(EngineId.Tes, assetUri).Result;
            _data = AssetManager.GetDataPack(EngineId.Tes, dataUri).Result;
            _cellManager = AssetManager.GetCellManager(EngineId.Tes, _asset, _data);
            SpawnPlayerOutside(playerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
            //var obj = pack.CreateObject("meshes/x/ex_common_balcony_01.nif");
            //GameObject.Instantiate(obj);
            //Cursor.SetCursor(pack.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);
        }

        static object _currentCell;
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
            _currentCell = _data.FindExteriorCellRecord(gridCoords);
            Debug.Assert(_currentCell != null);
            CreatePlayer(playerPrefab, position, out _playerCameraObj);
            var cellInfo = _cellManager.StartCreatingExteriorCell(gridCoords);
            AssetManager.WaitForTask(cellInfo.objectsCreationCoroutine);
        }
    }
}
