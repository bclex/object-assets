using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class INGRRecord : Record
    {
        public class IRDTField : Field
        {
            public float Weight;
            public int Value;
            public int[] EffectId;
            public int[] SkillId;
            public int[] AttributeId;

            public override void Read(UnityBinaryReader r, uint dataSize)
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

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public IRDTField IRDT;
        public STRVField ITEX;
        public STRVField SCRI;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "IRDT": IRDT = new IRDTField(); return IRDT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}