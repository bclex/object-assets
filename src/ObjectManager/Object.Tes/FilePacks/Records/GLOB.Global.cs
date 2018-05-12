using System;

namespace OA.Tes.FilePacks.Records
{
    public class GLOBRecord : Record
    {
        public STRVField NAME;
        public ByteField FNAM;
        public FLTVField FLTV;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "FNAM": FNAM = new ByteField(); return FNAM;
                case "FLTV": FLTV = new FLTVField(); return FLTV;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}