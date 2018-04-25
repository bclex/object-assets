using OA.Core;
using UnityEngine;

namespace OA
{
    public static class ObjectTest
    {
        static IAssetPack Asset;
        static IDataPack Data;
        static BaseEngine Engine;
        static GameObject PlayerPrefab;

        public static void Awake()
        {
            Utils.InUnity = true;
            PlayerPrefab = GameObject.Find("Cube00");
        }

        public static void Start()
        {
            var assetUri = "game://Morrowind/Morrowind.bsa";
            var dataUri = "game://Morrowind/Morrowind.esm";

            var assetManager = AssetManager.GetAssetManager(EngineId.Tes);
            Asset = assetManager.GetAssetPack(assetUri).Result;
            Data = assetManager.GetDataPack(dataUri).Result;
            Engine = new BaseEngine(assetManager, Asset, Data);

            // engine
            Engine.SpawnPlayerOutside(PlayerPrefab, new Vector2i(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
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
            Engine.Update();
        }
    }
}
