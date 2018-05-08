using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class BOOKRecord : Record
    {
        public class BKDTSubRecord : SubRecord
        {
            public float Weight;
            public int Value;
            public int Scroll;
            public int SkillID;
            public int EnchantPts;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Scroll = r.ReadLEInt32();
                SkillID = r.ReadLEInt32();
                EnchantPts = r.ReadLEInt32();
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

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}
