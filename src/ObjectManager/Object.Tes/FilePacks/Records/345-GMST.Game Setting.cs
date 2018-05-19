using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class GMSTRecord : Record, IHaveEDID
    {
        public override string ToString() => $"GMST: {EDID.Value}";
        public STRVField EDID { get; set; } // Setting ID
        public DATVField DATA; // Data

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "STRV": DATA = DATVField.Create(r, dataSize, 's'); return true;
                    case "INTV": DATA = DATVField.Create(r, dataSize, 'i'); return true;
                    case "FLTV": DATA = DATVField.Create(r, dataSize, 'f'); return true;
                    default: return false;
                }
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "DATA": DATA = DATVField.Create(r, dataSize, EDID.Value[0]); return true;
                default: return false;
            }
        }
    }
}