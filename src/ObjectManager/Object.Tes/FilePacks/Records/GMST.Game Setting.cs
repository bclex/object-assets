using System;

namespace OA.Tes.FilePacks.Records
{
    public class GMSTRecord : Record
    {
        public STRVField NAME;
        //public STRVSubRecord STRV;
        //public INTVSubRecord INTV;
        //public FLTVSubRecord FLTV;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                //case "STRV": STRV = new STRVSubRecord(); return STRV;
                //case "INTV": INTV = new INTVSubRecord(); return INTV;
                //case "FLTV": FLTV = new FLTVSubRecord(); return FLTV;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId)
        {
            switch (type)
            {
                case "EDID": NAME = new STRVField(); return NAME;
                case "DATA": NAME = new STRVField(); return NAME;
                default: return null;
            }
        }
    }
}