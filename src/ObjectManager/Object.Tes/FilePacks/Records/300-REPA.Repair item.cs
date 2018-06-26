using OA.Core;

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

            public RIDTField(UnityBinaryReader r, int dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Uses = r.ReadLEInt32();
                Quality = r.ReadLESingle();
            }
        }

        public override string ToString() => $"REPA: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public RIDTField RIDT; // Repair Data
        public FILEField ICON; // Inventory Icon
        public FMIDField<SCPTRecord> SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            if (format == GameFormatId.TES3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "RIDT": RIDT = new RIDTField(r, dataSize); return true;
                    case "ITEX": ICON = new FILEField(r, dataSize); return true;
                    case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}