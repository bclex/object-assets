using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class FURNRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"FURN: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL; // Model
        public STRVField FULL; // Furniture Name
        public FMIDField<SCPTRecord> SCRI; // Script (optional)
        public IN32Field MNAM; // Active marker flags, required. A bit field with a bit value of 1 indicating that the matching marker position in the NIF file is active.

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
                case "MNAM": MNAM = new IN32Field(r, dataSize); return true;
                default: return false;
            }
        }
    }
}