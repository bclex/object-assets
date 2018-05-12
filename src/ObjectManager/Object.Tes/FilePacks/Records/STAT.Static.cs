using System;

namespace OA.Tes.FilePacks.Records
{
    public class STATRecord : Record
    {
        public STRVField NAME;
        public STRVField MODL;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}