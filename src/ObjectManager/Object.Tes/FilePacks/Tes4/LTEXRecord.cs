namespace OA.Tes.FilePacks.Tes4
{
    public class LTEXRecord : Record
    {
        public class DATASubRecord : STRVSubRecord { }

        public NAMESubRecord NAME;
        public INTVSubRecord INTV;
        public DATASubRecord DATA;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "INTV": INTV = new INTVSubRecord(); return INTV;
                case "DATA": DATA = new DATASubRecord(); return DATA;
                default: return null;
            }
        }
    }
}