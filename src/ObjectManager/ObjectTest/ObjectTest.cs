using OA.Core;
using OA.Tes;
using OA.Tes.FilePacks;
using UnityEngine;

namespace OA
{
    public static class ObjectTest
    {
        public static void Start()
        {
            var assetUri = "file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Morrowind/Data%20Files/Morrowind.*";
            var dataUri = "file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Morrowind/Data%20Files/Morrowind.esm#Morrowind";
            //var file2Uri = "file://192.168.1.3/User/_ASSETS/Fallout4/Textures1";
            //var file3Uri = "game://Morrowind/Morrowind.bsa";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";
            Utils.Info("HERE!");

            using (var asset = AssetManager.GetAssetPack(EngineId.Tes, assetUri).Result)
            using (var data = AssetManager.GetDataPack(EngineId.Tes, dataUri).Result)
            {
                var cellManager = AssetManager.GetCellManager(EngineId.Tes, asset, data);
                //var obj = pack.CreateObject("meshes/x/ex_common_balcony_01.nif");
                //obj.transform.position += new Vector3(2, 2, 2);
                //GameObject.Instantiate(obj);
                //Cursor.SetCursor(pack.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);
            }
        }
    }
}
