using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class APPARecord : Record, IHaveEDID, IHaveMODL
    {
        public struct AADTField
        {
            public int Type; // 0 = Mortar and Pestle, 1 = Albemic, 2 = Calcinator, 3 = Retort
            public float Quality;
            public float Weight;
            public int Value;

            public AADTField(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Quality = r.ReadLESingle();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"APPA: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public AADTField AADT; // Alchemy Data
        public FILEField ITEX; // Inventory Icon
        public STRVField SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "AADT": AADT = new AADTField(r, dataSize); return true;
                case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}