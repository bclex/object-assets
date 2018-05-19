using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class HAIRRecord : Record
    {
        public override string ToString() => $"HAIR: {EDID.Value}";
        public STRVField EDID;
        public STRVField FULL;
        public FILEField MODL;
        public STRVField MODB;
        public FILEField ICON;
        public BYTEField DATA; // Playable, Not Male, Not Female, Fixed

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "MODB": MODB = new STRVField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "DATA": DATA = new BYTEField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}