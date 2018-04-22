using UnityEngine;

namespace OA
{
    public class ObjectTest // : MonoBehaviour
    {
        public void Start()
        {
            //var box = Instantiate(GameObject.Find("Cube00"), this.transform);
            //box.transform.position += new Vector3(2, 2, 2);
            Run();
            //GameObject.Destroy(box);
        }

        public static void Run()
        {
            var file1Uri = "file:///C:/Program%20Files%20(x86)/Steam/steamapps/common/Morrowind/Data%20Files/Morrowind.bsa";
            //var file2Uri = "file://192.168.1.3/User/_ASSETS/Fallout4/Textures1";
            //var file3Uri = "game://Morrowind/Morrowind.bsa";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";
            //var file4Uri = "http://192.168.1.3/assets/Morrowind/Morrowind.bsa";
            var fileUri = file1Uri;

            using (var pack = AssetManager.GetAssetPack(EngineId.Tes, fileUri).Result)
            {
                //var obj = pack.CreateObject("meshes/x/ex_common_balcony_01.nif");
                var obj = pack.CreateObject("meshes/f/furn_dwrv_tranny00.nif");
                obj.transform.position += new Vector3(2, 2, 2);
                GameObject.Instantiate(obj);
                //Cursor.SetCursor(pack.LoadTexture("tx_cursor", true), Vector2.zero, CursorMode.Auto);
            }
        }
    }
}
