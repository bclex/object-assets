using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class BOOKRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct DATAField
        {
            public byte Flags; //: Scroll - (1 is scroll, 0 not)
            public byte Teaches; //: SkillId - (-1 is no skill)
            public int Value;
            public float Weight;
            //
            public int EnchantPts;

            public DATAField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                if (format == GameFormatId.TES3)
                {
                    Weight = r.ReadLESingle();
                    Value = r.ReadLEInt32();
                    Flags = (byte)r.ReadLEInt32();
                    Teaches = (byte)r.ReadLEInt32();
                    EnchantPts = r.ReadLEInt32();
                    return;
                }
                Flags = r.ReadByte();
                Teaches = r.ReadByte();
                Value = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                EnchantPts = 0;
            }
        }

        public override string ToString() => $"BOOK: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model (optional)
        public STRVField FULL; // Item Name
        public DATAField DATA; // Book Data
        public STRVField DESC; // Book Text
        public FILEField ICON; // Inventory Icon (optional)
        public FMIDField<SCPTRecord> SCRI; // Script Name (optional)
        public FMIDField<ENCHRecord> ENAM; // Enchantment FormId (optional)
        // TES4
        public IN16Field? ANAM; // Enchantment points (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
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
                case "BKDT": DATA = new DATAField(r, dataSize, format); return true;
                case "ICON":
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "DESC":
                case "TEXT": DESC = new STRVField(r, dataSize); return true;
                case "ENAM": ENAM = new FMIDField<ENCHRecord>(r, dataSize); return true;
                case "ANAM": ANAM = new IN16Field(r, dataSize); return true;
                default: return false;
            }
        }
    }
}
