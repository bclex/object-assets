using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class LOCKRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct LKDTField
        {
            public float Weight;
            public int Value;
            public float Quality;
            public int Uses;

            public LKDTField(UnityBinaryReader r, int dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Quality = r.ReadLESingle();
                Uses = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"LOCK: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public LKDTField LKDT; // Lock Data
        public FILEField ICON; // Inventory Icon
        public FMIDField<SCPTRecord> SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            if (format == GameFormatId.TES3)
                switch (type)
                {
                    case "NAME": EDID = r.ReadSTRV(dataSize); return true;
                    case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                    case "FNAM": FNAM = r.ReadSTRV(dataSize); return true;
                    case "LKDT": LKDT = new LKDTField(r, dataSize); return true;
                    case "ITEX": ICON = r.ReadFILE(dataSize); return true;
                    case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}