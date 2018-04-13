using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class ARMORecord : Record
    {
        public class AODTSubRecord : SubRecord
        {
            public int type;
            public float weight;
            public int value;
            public int health;
            public int enchantPts;
            public int armour;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                type = r.ReadLEInt32();
                weight = r.ReadLESingle();
                value = r.ReadLEInt32();
                health = r.ReadLEInt32();
                enchantPts = r.ReadLEInt32();
                armour = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public AODTSubRecord AODT;
        public ITEXSubRecord ITEX;

        public List<INDXBNAMCNAMGroup> INDXBNAMCNAMGroups = new List<INDXBNAMCNAMGroup>();

        public SCRISubRecord SCRI;
        public ENAMSubRecord ENAM;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "AODT": AODT = new AODTSubRecord(); return AODT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "INDX": var INDX = new INDXSubRecord(); var group = new INDXBNAMCNAMGroup(); group.INDX = INDX; INDXBNAMCNAMGroups.Add(group); return INDX;
                case "BNAM": var BNAM = new BNAMSubRecord(); ArrayUtils.Last(INDXBNAMCNAMGroups).BNAM = BNAM; return BNAM;
                case "CNAM": var CNAM = new CNAMSubRecord(); ArrayUtils.Last(INDXBNAMCNAMGroups).CNAM = CNAM; return CNAM;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                case "ENAM": ENAM = new ENAMSubRecord(); return ENAM;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}