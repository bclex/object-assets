using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class INGRRecord : Record
    {
        public class IRDTSubRecord : SubRecord
        {
            public float Weight;
            public int Value;
            public int[] EffectId;
            public int[] SkillId;
            public int[] AttributeId;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                EffectId = new int[4];
                for (var i = 0; i < EffectId.Length; i++)
                    EffectId[i] = r.ReadLEInt32();
                SkillId = new int[4];
                for (var i = 0; i < SkillId.Length; i++)
                    SkillId[i] = r.ReadLEInt32();
                AttributeId = new int[4];
                for (var i = 0; i < AttributeId.Length; i++)
                    AttributeId[i] = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public IRDTSubRecord IRDT;
        public ITEXSubRecord ITEX;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "IRDT": IRDT = new IRDTSubRecord(); return IRDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}