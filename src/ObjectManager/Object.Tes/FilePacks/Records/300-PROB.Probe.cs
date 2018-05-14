using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class PROBRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct PBDTField
        {
            public float Weight;
            public int Value;
            public float Quality;
            public int Uses;

            public PBDTField(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Quality = r.ReadLESingle();
                Uses = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"PROB: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public PBDTField PBDT; // Probe Data
        public STRVField ITEX; // Inventory Icon
        public STRVField SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "PBDT": PBDT = new PBDTField(r, dataSize); return true;
                case "ITEX": ITEX = new STRVField(r, dataSize); return true;
                case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}