using OA.Core;

namespace OA.Tes.FilePacks.Tes4
{
    public class APPARecord : Record
    {
        public class AADTSubRecord : SubRecord
        {
            public int type;
            public float quality;
            public float weight;
            public int value;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                type = r.ReadLEInt32();
                quality = r.ReadLESingle();
                weight = r.ReadLESingle();
                value = r.ReadLEInt32();
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
    }
}