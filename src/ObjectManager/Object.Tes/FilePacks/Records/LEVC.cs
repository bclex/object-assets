using System;

namespace OA.Tes.FilePacks.Records
{
    public class LEVCRecord : Record
    {
        public STRVField NAME;
        public INTVField DATA;
        public ByteField NNAM;
        public INTVField INDX;
        public STRVField CNAM;
        public INTVField INTV;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "DATA": DATA = new INTVField(); return DATA;
                case "NNAM": NNAM = new ByteField(); break;
                case "INDX": INDX = new INTVField(); break;
                case "CNAM": CNAM = new STRVField(); break;
                case "INTV": INTV = new INTVField(); break;
            }
            return null;
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}