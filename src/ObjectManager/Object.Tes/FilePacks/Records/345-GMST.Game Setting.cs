using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class GMSTRecord : Record, IHaveEDID
    {
        public override string ToString() => $"GMST: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public DATVField DATA; // Data

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            if (format == GameFormatId.TES3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "STRV": DATA = new DATVField(r, dataSize, 's'); return true;
                    case "INTV": DATA = new DATVField(r, dataSize, 'i'); return true;
                    case "FLTV": DATA = new DATVField(r, dataSize, 'f'); return true;
                    default: return false;
                }
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new DATVField(r, dataSize, EDID.Value[0]); return true;
                default: return false;
            }
        }
    }
}