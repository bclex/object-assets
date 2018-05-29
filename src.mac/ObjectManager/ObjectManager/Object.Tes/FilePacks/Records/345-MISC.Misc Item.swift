using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class MISCRecord : Record, IHaveEDID, IHaveMODL
    {
        // TESX
        public struct DATAField
        {
            public float Weight;
            public uint Value;
            public uint Unknown;

            public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
            {
                if (formatId == GameFormatId.TES3)
                {
                    Weight = r.ReadLESingle();
                    Value = r.ReadLEUInt32();
                    Unknown = r.ReadLEUInt32();
                    return;
                }
                Value = r.ReadLEUInt32();
                Weight = r.ReadLESingle();
                Unknown = 0;
            }
        }

        public override string ToString() => $"MISC: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public STRVField FULL; // Item Name
        public DATAField DATA; // Misc Item Data
        public FILEField ICON; // Icon (optional)
        public FMIDField<SCPTRecord> SCRI; // Script FormID (optional)
        // TES3
        public FMIDField<ENCHRecord> ENAM; // enchantment ID

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "FULL":
                case "FNAM": FULL = new STRVField(r, dataSize); return true;
                case "DATA":
                case "MCDT": DATA = new DATAField(r, dataSize, formatId); return true;
                case "ICON":
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "ENAM": ENAM = new FMIDField<ENCHRecord>(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                default: return false;
            }
        }
    }
}