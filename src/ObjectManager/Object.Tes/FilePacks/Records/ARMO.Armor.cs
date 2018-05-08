using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class ARMORecord : Record
    {
        public class AODTSubRecord : SubRecord
        {
            public int Type;
            public float Weight;
            public int Value;
            public int Health;
            public int EnchantPts;
            public int Armour;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Health = r.ReadLEInt32();
                EnchantPts = r.ReadLEInt32();
                Armour = r.ReadLEInt32();
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