using System;

namespace OA.Tes.FilePacks.Records
{
    public class DOORRecord : Record
    {
        public STRVField NAME; // door ID
        public STRVField FNAM; // door name
        public STRVField MODL; // model filename
        // public STRVField SCIP; // script
        public STRVField SNAM;
        public STRVField ANAM;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "MODL": MODL = new STRVField(); return MODL;
                /*case "SCIP": SCIP = new STRVSubRecord(); return SCIP;*/
                case "SNAM": SNAM = new STRVField(); return SNAM;
                case "ANAM": ANAM = new STRVField(); return ANAM;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}