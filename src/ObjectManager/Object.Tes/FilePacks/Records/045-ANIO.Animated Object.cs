using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class ANIORecord : Record
    {
        public override string ToString() => $"ANIO: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL; // Model
        public FMIDField<IDLERecord> DATA; // IDLE animation

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "DATA": DATA = new FMIDField<IDLERecord>(r, dataSize); return true;
                default: return false;
            }
        }
    }
}