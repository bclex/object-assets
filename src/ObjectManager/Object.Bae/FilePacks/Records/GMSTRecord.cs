namespace OA.Bae.FilePacks
{
    public class GMSTRecord : Record
    {
        public NAMESubRecord NAME;
        public STRVSubRecord STRV;
        public INTVSubRecord INTV;
        public FLTVSubRecord FLTV;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "STRV": STRV = new STRVSubRecord(); return STRV;
                case "INTV": INTV = new INTVSubRecord(); return INTV;
                case "FLTV": FLTV = new FLTVSubRecord(); return FLTV;
                default: return null;
            }
        }
    }
}