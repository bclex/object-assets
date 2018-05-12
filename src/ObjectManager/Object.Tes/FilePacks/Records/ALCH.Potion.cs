using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class ALCHRecord : Record
    {
        public class ALDTField : Field
        {
            public float Weight;
            public int Value;
            public int AutoCalc;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                AutoCalc = r.ReadLEInt32();
            }
        }

        public class ENAMField : Field
        {
            public short EffectId;
            public byte SkillId;
            public byte AttributeId;
            public int Unknown1;
            public int Unknown2;
            public int Duration;
            public int Magnitude;
            public int Unknown4;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                EffectId = r.ReadLEInt16();
                SkillId = r.ReadByte();
                AttributeId = r.ReadByte();
                Unknown1 = r.ReadLEInt32();
                Unknown2 = r.ReadLEInt32();
                Duration = r.ReadLEInt32();
                Magnitude = r.ReadLEInt32();
                Unknown4 = r.ReadLEInt32();
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public ALDTField ALDT;
        public ENAMField ENAM;
        public STRVField TEXT;
        public STRVField SCRI;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "ALDT": ALDT = new ALDTField(); return ALDT;
                case "ENAM": ENAM = new ENAMField(); return ENAM;
                case "TEXT": TEXT = new STRVField(); return TEXT;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}