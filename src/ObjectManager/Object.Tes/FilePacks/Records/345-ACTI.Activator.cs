using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class ACTIRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"ACTI: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model Name
        public FLTVField MODB { get; set; } // Model Bounds
        public BYTVField MODT; // Texture Files Hashes
        public STRVField FULL; // Item Name
        public FMIDField<SCPTRecord> SCRI; // Script (Optional)
        // TES4
        public FMIDField<SOUNRecord> SNAM; // Sound (Optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
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
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "SNAM": SNAM = new FMIDField<SOUNRecord>(r, dataSize); return true;
                default: return false;
            }
        }
    }
}