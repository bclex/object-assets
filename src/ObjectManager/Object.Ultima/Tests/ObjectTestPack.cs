using OA.Core;
using UnityEngine;

namespace OA.Ultima
{
    public static class ObjectTestPack
    {
        static IAssetPack Asset;
        static IDataPack Data;
        static GameObject PlayerPrefab;

        public static void Awake()
        {
            Utils.InUnity = true;
            PlayerPrefab = GameObject.Find("Cube00");
        }

        public static void Start()
        {
            Utils.Log("UltimaAsset");
            var assetManager = AssetManager.GetAssetManager(EngineId.Ultima);
            Asset = assetManager.GetAssetPack(null).Result;
            Data = assetManager.GetDataPack(null).Result;

            var textureManager = new TextureManager(Asset);
            var materialManager = new MaterialManager(textureManager);

            var obj = GameObject.Find("Cube"); // CreatePrimitive(PrimitiveType.Cube);
            var meshRenderer = obj.GetComponent<MeshRenderer>();
            var materialProps = new MaterialProps
            {
                //textures = new MaterialTextures { mainFilePath = "lnd1" },
                //textures = new MaterialTextures { mainFilePath = "lnd516" },
                //textures = new MaterialTextures { mainFilePath = "lnd1137" },
                //textures = new MaterialTextures { mainFilePath = "sta69" },
                textures = new MaterialTextures { mainFilePath = "gmp65" },
                //textures = new MaterialTextures { mainFilePath = "tex789" },
            };
            meshRenderer.material = materialManager.BuildMaterialFromProperties(materialProps);

            var cursor = Asset.LoadTexture("gmp65");
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
