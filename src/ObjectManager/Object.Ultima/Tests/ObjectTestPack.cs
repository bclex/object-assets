using OA.Core;
using UnityEngine;

namespace OA.Ultima
{
    public static class ObjectTestPack
    {
        static IAssetPack Asset;
        static IDataPack Data;

        public static void Awake()
        {
            Utils.InUnity = true;
        }

        public static void Start()
        {
            var assetManager = AssetManager.GetAssetManager(EngineId.Ultima);
            Asset = assetManager.GetAssetPack(null).Result;
            Data = assetManager.GetDataPack(null).Result;

            //MakeObject("sta10");
            MakeObject("sta3");

            //MakeTexture("lnd1");
            //MakeTexture("lnd516");
            //MakeTexture("lnd1137");
            //MakeTexture("sta69");
            //MakeTexture("gmp65");
            //MakeTexture("tex789");
            //MakeCursor("gmp65");
        }

        static void MakeObject(string path)
        {
            var obj = Asset.CreateObject(path);
        }

        static void MakeTexture(string path)
        {
            var textureManager = new TextureManager(Asset);
            var materialManager = new MaterialManager(textureManager);
            var obj = GameObject.Find("Cube"); // CreatePrimitive(PrimitiveType.Cube);
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            var materialProps = new MaterialProps
            {
                Textures = new MaterialTextures { MainFilePath = path },
            };
            meshRenderer.material = materialManager.BuildMaterialFromProperties(materialProps);
        }

        static void MakeCursor(string path)
        {
            var cursor = Asset.LoadTexture(path);
            Cursor.SetCursor(cursor, Vector2.zero, CursorMode.Auto);
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

        public static void Update()
        {
        }
    }
}
