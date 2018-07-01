using OA.Core;
using UnityEngine;

namespace OA.Tes
{
    public static class ObjectTestEngine
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
            //var assetUri = "game://Morrowind/Morrowind.bsa";
            //var dataUri = "game://Morrowind/Morrowind.esm";

            var assetUri = "game://Oblivion/Oblivion*";
            var dataUri = "game://Oblivion/Oblivion.esm";

            var assetManager = AssetManager.GetAssetManager(EngineId.Tes);
            Asset = assetManager.GetAssetPack(assetUri).Result;
            Data = assetManager.GetDataPack(dataUri).Result;
            Engine = new BaseEngine(assetManager, Asset, Data);

            // engine
            //Engine.SpawnPlayer(PlayerPrefab, new Vector3Int(-2, -9), new Vector3(-137.94f, 2.30f, -1037.6f));
            Engine.SpawnPlayerAndUpdate(PlayerPrefab, new Vector3(-137.94f, 2.30f, -1037.6f));

            // engine - oblivion
            //Engine.SpawnPlayerOutside(PlayerPrefab, new Vector3(0, 0, 0));
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
