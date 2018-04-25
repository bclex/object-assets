using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class APPARecord : Record
    {
        public class AADTSubRecord : SubRecord
        {
            public int Type;
            public float Quality;
            public float Weight;
            public int Value;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Quality = r.ReadLESingle();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public AADTSubRecord AADT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "AADT": AADT = new AADTSubRecord(); return AADT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}