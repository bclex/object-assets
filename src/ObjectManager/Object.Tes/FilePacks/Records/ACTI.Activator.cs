using System;

namespace OA.Tes.FilePacks.Records
{
    public class ACTIRecord : Record
    {
        public STRVField NAME; // door ID
        public STRVField MODL; // model filename
        public STRVField FNAM; // item name
        public STRVField SCRI; // script ID string

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}