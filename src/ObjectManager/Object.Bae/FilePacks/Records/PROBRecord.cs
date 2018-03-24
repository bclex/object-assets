using OA.Core;

namespace OA.Bae.FilePacks
{
    public class PROBRecord : Record
    {
        public class PBDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public float quality;
            public int uses;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                weight = r.ReadLESingle();
                value = r.ReadLEInt32();
                quality = r.ReadLESingle();
                uses = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public PBDTSubRecord PBDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "PBDT": PBDT = new PBDTSubRecord(); return PBDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }
    }
}