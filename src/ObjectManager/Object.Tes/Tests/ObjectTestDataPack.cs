using OA.Core;
using OA.Tes.FilePacks;
using OA.Tes.FilePacks.Records;
using System;
using UnityEngine;

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

            TestLoadCell(new Vector3(((-2 << 8) + 1) * ConvertUtils.ExteriorCellSideLengthInMeters, 0, ((-1 << 8) + 1) * ConvertUtils.ExteriorCellSideLengthInMeters));
            TestLoadCell(new Vector3((((-2 << 8)) + (+1 << 4)) * ConvertUtils.ExteriorCellSideLengthInMeters, 0, (-2 << 8) * ConvertUtils.ExteriorCellSideLengthInMeters));
            //TestLoadCell(new Vector3((-1 << 4) * ConvertUtils.ExteriorCellSideLengthInMeters, 0, (-1 << 4) * ConvertUtils.ExteriorCellSideLengthInMeters));
            //TestLoadCell(new Vector3(0 * ConvertUtils.ExteriorCellSideLengthInMeters, 0, 0 * ConvertUtils.ExteriorCellSideLengthInMeters));
            //TestLoadCell(new Vector3((1 << 4) * ConvertUtils.ExteriorCellSideLengthInMeters, 0, (1 << 4) * ConvertUtils.ExteriorCellSideLengthInMeters));
            //TestLoadCell(new Vector3((1 << 8) * ConvertUtils.ExteriorCellSideLengthInMeters, 0, (1 << 8) * ConvertUtils.ExteriorCellSideLengthInMeters));
            //TestAllCells();
        }

        public static Vector3Int GetCellId(Vector3 point, int worldId) => new Vector3Int(Mathf.FloorToInt(point.x / ConvertUtils.ExteriorCellSideLengthInMeters), Mathf.FloorToInt(point.z / ConvertUtils.ExteriorCellSideLengthInMeters), worldId);

        static void TestLoadCell(Vector3 position)
        {
            var cellId = GetCellId(position, 60);
            var cell = Data.FindCellRecord(cellId);
            //var land = ((TesDataPack)Data).FindLANDRecord(cellId);
        }

        //static void TestAllCells()
        //{
        //    foreach (CELLRecord record in ((TesDataPack)Data).GetRecordsOfType<CELLRecord>())
        //    {
        //        Console.WriteLine(record.EDID.Value);
        //    }
        //}

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
