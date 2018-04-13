using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class SOUNRecord : Record
    {
        public class DATASubRecord : SubRecord
        {
            public byte volume;
            public byte minRange;
            public byte maxRange;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                volume = r.ReadByte();
                minRange = r.ReadByte();
                maxRange = r.ReadByte();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public DATASubRecord DATA;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "DATA": DATA = new DATASubRecord(); return DATA;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}