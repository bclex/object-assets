using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class MISCRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct MCDTField
        {
            public float Weight;
            public uint Value;
            public uint Unknown;

            public MCDTField(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEUInt32();
                Unknown = r.ReadLEUInt32();
            }
        }

        public override string ToString() => $"MISC: {EDID.Value}";
        public STRVField EDID { get; set; } // item ID
        public FILEField MODL { get; set; } // model
        public STRVField FNAM; // item name
        public MCDTField MCDT; // misc data
        public FILEField ITEX; // inventory icon
        public STRVField ENAM; // enchantment ID
        public STRVField SCRI; // script ID

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "MCDT": MCDT = new MCDTField(r, dataSize); return true;
                    case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                    case "ENAM": ENAM = new STRVField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}