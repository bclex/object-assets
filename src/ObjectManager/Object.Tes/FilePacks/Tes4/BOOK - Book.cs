using OA.Core;

namespace OA.Tes.FilePacks.Tes4
{
    public class BOOKRecord : Record
    {
        public class BKDTSubRecord : SubRecord
        {
            public float weight;
            public int value;
            public int scroll;
            public int skillID;
            public int enchantPts;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                weight = r.ReadLESingle();
                value = r.ReadLEInt32();
                scroll = r.ReadLEInt32();
                skillID = r.ReadLEInt32();
                enchantPts = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public BKDTSubRecord BKDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;
        public TEXTSubRecord TEXT;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "BKDT": BKDT = new BKDTSubRecord(); return BKDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                case "TEXT": TEXT = new TEXTSubRecord(); return TEXT;
                default: return null;
            }
        }
    }
}
