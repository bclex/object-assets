using System;

namespace OA.Tes.FilePacks.Records
{
    public class GLOBRecord : Record
    {
        public class FNAMSubRecord : ByteSubRecord { }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public FLTVSubRecord FLTV;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "FLTV": FLTV = new FLTVSubRecord(); return FLTV;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}