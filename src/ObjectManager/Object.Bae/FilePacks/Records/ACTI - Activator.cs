namespace OA.Bae.FilePacks
{
    public class ACTIRecord : Record
    {
        public NAMESubRecord NAME; // door ID
        public MODLSubRecord MODL; // model filename
        public FNAMSubRecord FNAM; // item name
        public SCRISubRecord SCRI; // script ID string

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }
    }
}