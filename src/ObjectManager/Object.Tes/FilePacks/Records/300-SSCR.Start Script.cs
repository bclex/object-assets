using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class SSCRRecord : Record
    {
        public override string ToString() => $"SSCR: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField DATA; // Digits

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            if (formatId == GameFormatId.TES3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "DATA": DATA = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}
