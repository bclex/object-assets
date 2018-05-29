using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class EYESRecord : Record
    {
        public override string ToString() => $"EYES: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField FULL;
        public FILEField ICON;
        public BYTEField DATA; // Playable

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "DATA": DATA = new BYTEField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}