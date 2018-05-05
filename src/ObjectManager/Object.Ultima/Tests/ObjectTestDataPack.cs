using OA.Core;

namespace OA.Ultima
{
    public static class ObjectTestDataPack
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
