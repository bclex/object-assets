using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class KEYMRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct DATAField
        {
            public int Value;
            public float Weight;

            public DATAField(UnityBinaryReader r, int dataSize)
            {
                Value = r.ReadLEInt32();
                Weight = r.ReadLESingle();
            }
        }

        public override string ToString() => $"KEYM: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public STRVField FULL; // Item Name
        public FMIDField<SCPTRecord> SCRI; // Script (optional)
        public DATAField DATA; // Type of soul contained in the gem
        public FILEField ICON; // Icon (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}