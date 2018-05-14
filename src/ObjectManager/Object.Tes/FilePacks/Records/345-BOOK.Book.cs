using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class BOOKRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct BKDTField
        {
            public float Weight;
            public int Value;
            public int Scroll; // (1 is scroll, 0 not)
            public int SkillId; // (-1 is no skill)
            public int EnchantPts;

            public BKDTField(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Scroll = r.ReadLEInt32();
                SkillId = r.ReadLEInt32();
                EnchantPts = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"BOOK: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public BKDTField BKDT; // Book Data
        public FILEField ITEX; // Inventory Icon
        public STRVField SCRI; // Script Name
        public STRVField TEXT; // Book text
        public STRVField ENAM; // Unknown

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "BKDT": BKDT = new BKDTField(r, dataSize); return true;
                case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                case "TEXT": TEXT = new STRVField(r, dataSize); return true;
                case "ENAM": ENAM = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}
