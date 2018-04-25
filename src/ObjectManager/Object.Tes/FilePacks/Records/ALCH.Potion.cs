using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class ALCHRecord : Record
    {
        public class ALDTSubRecord : SubRecord
        {
            public float Weight;
            public int Value;
            public int AutoCalc;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                AutoCalc = r.ReadLEInt32();
            }
        }

        public class ENAMSubRecord : SubRecord
        {
            public short EffectId;
            public byte SkillId;
            public byte AttributeId;
            public int Unknown1;
            public int Unknown2;
            public int Duration;
            public int Magnitude;
            public int Unknown4;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
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

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public ALDTSubRecord ALDT;
        public ENAMSubRecord ENAM;
        public TEXTSubRecord TEXT;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "ALDT": ALDT = new ALDTSubRecord(); return ALDT;
                case "ENAM": ENAM = new ENAMSubRecord(); return ENAM;
                case "TEXT": TEXT = new TEXTSubRecord(); return TEXT;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}