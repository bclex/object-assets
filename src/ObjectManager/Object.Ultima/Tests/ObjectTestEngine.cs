using OA.Core;
using UnityEngine;

namespace OA.Ultima
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
            var assetManager = AssetManager.GetAssetManager(EngineId.Ultima);
            Asset = assetManager.GetAssetPack(null).Result;
            Data = assetManager.GetDataPack(null).Result;
            Engine = new UltimaEngine(assetManager, Asset, Data, null);

            var scale = ConvertUtils.ExteriorCellSideLengthInMeters;
            //Engine.SpawnPlayerOutside(PlayerPrefab, new Vector3(4 * scale, 20, 25 * scale));
            //Engine.SpawnPlayerOutside(PlayerPrefab, new Vector3(15 * scale, 20, 25 * scale));
            Engine.SpawnPlayerOutside(PlayerPrefab, new Vector3(11 * scale, 20, 29 * scale));
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
