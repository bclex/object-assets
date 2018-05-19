using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CLOTRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct CTDTField
        {
            public enum CLOTType
            {
                Pants = 0,
                Shoes = 1,
                Shirt = 2,
                Belt = 3,
                Robe = 4,
                R_Glove = 5,
                L_Glove = 6,
                Skirt = 7,
                Ring = 8,
                Amulet = 9,
            }

            public int Type;
            public float Weight;
            public short Value;
            public short EnchantPts;

            public CTDTField(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt16();
                EnchantPts = r.ReadLEInt16();
            }
        }

        public class INDXFieldGroup
        {
            public override string ToString() => $"{INDX.Value}: {BNAM.Value}";
            public INTVField INDX;
            public STRVField BNAM;
            public STRVField CNAM;
        }

        public override string ToString() => $"CLOT: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public CTDTField CTDT; // Clothing Data
        public FILEField ITEX; // Inventory Icon
        public List<INDXFieldGroup> INDXs = new List<INDXFieldGroup>(); // Body Part Index
        public STRVField ENAM; // Enchantment Name
        public STRVField SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "CTDT": CTDT = new CTDTField(r, dataSize); return true;
                    case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                    case "INDX": INDXs.Add(new INDXFieldGroup { INDX = new INTVField(r, dataSize) }); return true;
                    case "BNAM": ArrayUtils.Last(INDXs).BNAM = new STRVField(r, dataSize); return true;
                    case "CNAM": ArrayUtils.Last(INDXs).CNAM = new STRVField(r, dataSize); return true;
                    case "ENAM": ENAM = new STRVField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}