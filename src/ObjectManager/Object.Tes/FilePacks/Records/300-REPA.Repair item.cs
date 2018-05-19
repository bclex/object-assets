using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class REPARecord : Record, IHaveEDID, IHaveMODL
    {
        public struct RIDTField
        {
            public float Weight;
            public int Value;
            public int Uses;
            public float Quality;

            public RIDTField(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Uses = r.ReadLEInt32();
                Quality = r.ReadLESingle();
            }
        }

        public override string ToString() => $"REPA: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public RIDTField RIDT; // Repair Data
        public FILEField ITEX; // Inventory Icon
        public STRVField SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "RIDT": RIDT = new RIDTField(r, dataSize); return true;
                case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                default: return false;
            }
            return false;
        }
    }
}