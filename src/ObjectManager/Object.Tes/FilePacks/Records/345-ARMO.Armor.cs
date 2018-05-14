using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class ARMORecord : Record, IHaveEDID, IHaveMODL
    {
        public struct AODTField
        {
            public enum ARMOType
            {
                Helmet = 0,
                Cuirass = 1,
                L_Pauldron = 2,
                R_Pauldron = 3,
                Greaves = 4,
                Boots = 5,
                L_Gauntlet = 6,
                R_Gauntlet = 7,
                Shield = 8,
                L_Bracer = 9,
                R_Bracer = 10,
            }

            public int Type;
            public float Weight;
            public int Value;
            public int Health;
            public int EnchantPts;
            public int Armour;

            public AODTField(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Health = r.ReadLEInt32();
                EnchantPts = r.ReadLEInt32();
                Armour = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"ARMO: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model
        public STRVField FNAM; // Item Name
        public AODTField AODT; // Armour Data
        public FILEField ITEX; // Icon
        public List<CLOTRecord.INDXFieldGroup> INDXs = new List<CLOTRecord.INDXFieldGroup>(); // Body Part Index
        public STRVField SCRI; // Script Name
        public STRVField ENAM; // Enchantment Name

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "AODT": AODT = new AODTField(r, dataSize); return true;
                case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                case "INDX": INDXs.Add(new CLOTRecord.INDXFieldGroup { INDX = new INTVField(r, dataSize) }); return true;
                case "BNAM": ArrayUtils.Last(INDXs).BNAM = new STRVField(r, dataSize); return true;
                case "CNAM": ArrayUtils.Last(INDXs).CNAM = new STRVField(r, dataSize); return true;
                case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                case "ENAM": ENAM = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}