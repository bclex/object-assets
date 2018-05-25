using OA.Core;
using OA.Tes.FilePacks;
using OA.Tes.FilePacks.Records;
using System;

namespace OA.Tes
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
            //var assetUri = "game://Morrowind/Morrowind.bsa";
            //var dataUri = "game://Morrowind/Morrowind.esm";
            //var dataUri = "game://Morrowind/Bloodmoon.esm";
            //var dataUri = "game://Morrowind/Tribunal.esm";

            //var assetUri = "game://Oblivion/Oblivion*";
            var dataUri = "game://Oblivion/Oblivion.esm";

            //var assetUri = "game://SkyrimVR/Skyrim*";
            //var dataUri = "game://SkyrimVR/Skyrim.esm";

            //var assetUri = "game://Fallout4/Fallout4*";
            //var dataUri = "game://Fallout4/Fallout4.esm";

            //var assetUri = "game://Fallout4VR/Fallout4*";
            //var dataUri = "game://Fallout4VR/Fallout4.esm";

            var assetManager = AssetManager.GetAssetManager(EngineId.Tes);
            //Asset = assetManager.GetAssetPack(assetUri).Result;
            Data = assetManager.GetDataPack(dataUri).Result;

            //TestAllCells();
        }

        static void TestAllCells()
        {
            foreach (CELLRecord record in ((TesDataPack)Data).GetRecordsOfType<CELLRecord>())
            {
                Console.WriteLine(record.EDID.Value);
            }
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
