using System;

namespace OA.Tes.FilePacks.Records
{
    public class LTEXRecord : Record
    {
        public STRVField NAME;
        public INTVField INTV;
        public STRVField DATA;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "INTV": INTV = new INTVField(); return INTV;
                case "DATA": DATA = new STRVField(); return DATA;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}