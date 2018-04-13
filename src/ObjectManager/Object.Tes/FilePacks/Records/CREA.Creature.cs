using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class CREARecord : Record
    {
        public class NPDTSubRecord : SubRecord
        {
            public int type;
            public int level;
            public int strength;
            public int intelligence;
            public int willpower;
            public int agility;
            public int speed;
            public int endurance;
            public int personality;
            public int luck;
            public int health;
            public int spellPts;
            public int fatigue;
            public int soul;
            public int combat;
            public int magic;
            public int stealth;
            public int attackMin1;
            public int attackMax1;
            public int attackMin2;
            public int attackMax2;
            public int attackMin3;
            public int attackMax3;
            public int gold;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                type = r.ReadLEInt32();
                level = r.ReadLEInt32();
                strength = r.ReadLEInt32();
                intelligence = r.ReadLEInt32();
                willpower = r.ReadLEInt32();
                agility = r.ReadLEInt32();
                speed = r.ReadLEInt32();
                endurance = r.ReadLEInt32();
                personality = r.ReadLEInt32();
                luck = r.ReadLEInt32();
                health = r.ReadLEInt32();
                spellPts = r.ReadLEInt32();
                fatigue = r.ReadLEInt32();
                soul = r.ReadLEInt32();
                combat = r.ReadLEInt32();
                magic = r.ReadLEInt32();
                stealth = r.ReadLEInt32();
                attackMin1 = r.ReadLEInt32();
                attackMax1 = r.ReadLEInt32();
                attackMin2 = r.ReadLEInt32();
                attackMax2 = r.ReadLEInt32();
                attackMin3 = r.ReadLEInt32();
                attackMax3 = r.ReadLEInt32();
                gold = r.ReadLEInt32();
            }
        }

        public class FLAGSubRecord : Int32SubRecord { }

        public class NPCOSubRecord : SubRecord
        {
            public int count;
            public char[] name;

            public NPCOSubRecord()
            {
                name = new char[32];
            }

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                count = r.ReadLEInt32();
                var bytes = r.ReadBytes(32);
                for (var i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
            }
        }

        public class AI_WSubRecord : SubRecord
        {
            public short distance;
            public byte duration;
            public byte timeOfDay;
            public byte[] idle;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                distance = r.ReadLEInt16();
                duration = r.ReadByte();
                timeOfDay = r.ReadByte();
                idle = r.ReadBytes(10);
            }
        }

        public class AIDTSubRecord : SubRecord
        {
            public byte[] value1;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                value1 = r.ReadBytes(12);
            }
        }

        public class XSCLSubRecord : FLTVSubRecord { }

        public enum Flags
        {
            Biped = 0x0001,
            Respawn = 0x0002,
            WeaponAndShield = 0x0004,
            None = 0x0008,
            Swims = 0x0010,
            Flies = 0x0020,
            Walks = 0x0040,
            DefaultFlags = 0x0048,
            Essential = 0x0080,
            SkeletonBlood = 0x0400,
            MetalBlood = 0x0800
        }

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public NPDTSubRecord NPDT;
        public FLAGSubRecord FLAG;
        public SCRISubRecord SCRI;
        public NPCOSubRecord NPCO;
        public AIDTSubRecord AIDT;
        public AI_WSubRecord AI_W;
        public NPC_Record.AI_TSubRecord AI_T;
        public NPC_Record.AI_FSubRecord AI_F;
        public NPC_Record.AI_ESubRecord AI_E;
        public NPC_Record.AI_ASubRecord AI_A;
        public XSCLSubRecord XSCL;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "NPDT": NPDT = new NPDTSubRecord(); return NPDT;
                case "FLAG": FLAG = new FLAGSubRecord(); return FLAG;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                case "NPCO": NPCO = new NPCOSubRecord(); return NPCO;
                case "AIDT": AIDT = new AIDTSubRecord(); return AIDT;
                case "AI_W": AI_W = new AI_WSubRecord(); return AI_W;
                /* case "AI_T": AI_T = new NPC_Record.AI_TSubRecord(); return AI_T;
                 case "AI_F": AI_F = new NPC_Record.AI_FSubRecord(); return AI_F;
                 case "AI_E": AI_E = new NPC_Record.AI_ESubRecord(); return AI_E;
                 case "AI_A": AI_A = new NPC_Record.AI_ASubRecord(); return AI_A;*/
                case "XSCL": XSCL = new XSCLSubRecord(); return XSCL;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}