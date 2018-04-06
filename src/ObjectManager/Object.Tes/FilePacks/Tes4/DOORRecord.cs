namespace OA.Tes.FilePacks.Tes4
{
    public class DOORRecord : Record
    {
        public NAMESubRecord NAME; // door ID
        public FNAMSubRecord FNAM; // door name
        public MODLSubRecord MODL; // model filename
                                   // public SCIPSubRecord SCIP; // script
        public SNAMSubRecord SNAM;
        public ANAMSubRecord ANAM;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                /*case "SCIP": SCIP = new SCIPSubRecord(); return SCIP;*/
                case "SNAM": SNAM = new SNAMSubRecord(); return SNAM;
                case "ANAM": ANAM = new ANAMSubRecord(); return ANAM;
                default: return null;
            }
        }
    }
}