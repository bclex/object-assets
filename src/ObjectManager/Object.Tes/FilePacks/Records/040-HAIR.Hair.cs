using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class HAIRRecord : Record
    {
        public override string ToString() => $"HAIR: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField FULL;
        public MODLGroup MODL;
        public FILEField ICON;
        public BYTEField DATA; // Playable, Not Male, Not Female, Fixed

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "DATA": DATA = new BYTEField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}