using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class CREARecord : Record
    {
        public class NPDTSubRecord : SubRecord
        {
            public int Type;
            public int Level;
            public int Strength;
            public int Intelligence;
            public int Willpower;
            public int Agility;
            public int Speed;
            public int Endurance;
            public int Personality;
            public int Luck;
            public int Health;
            public int SpellPts;
            public int Fatigue;
            public int Soul;
            public int Combat;
            public int Magic;
            public int Stealth;
            public int AttackMin1;
            public int AttackMax1;
            public int AttackMin2;
            public int AttackMax2;
            public int AttackMin3;
            public int AttackMax3;
            public int Gold;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Level = r.ReadLEInt32();
                Strength = r.ReadLEInt32();
                Intelligence = r.ReadLEInt32();
                Willpower = r.ReadLEInt32();
                Agility = r.ReadLEInt32();
                Speed = r.ReadLEInt32();
                Endurance = r.ReadLEInt32();
                Personality = r.ReadLEInt32();
                Luck = r.ReadLEInt32();
                Health = r.ReadLEInt32();
                SpellPts = r.ReadLEInt32();
                Fatigue = r.ReadLEInt32();
                Soul = r.ReadLEInt32();
                Combat = r.ReadLEInt32();
                Magic = r.ReadLEInt32();
                Stealth = r.ReadLEInt32();
                AttackMin1 = r.ReadLEInt32();
                AttackMax1 = r.ReadLEInt32();
                AttackMin2 = r.ReadLEInt32();
                AttackMax2 = r.ReadLEInt32();
                AttackMin3 = r.ReadLEInt32();
                AttackMax3 = r.ReadLEInt32();
                Gold = r.ReadLEInt32();
            }
        }

        public class FLAGSubRecord : Int32SubRecord { }

        public class NPCOSubRecord : SubRecord
        {
            public int Count;
            public char[] Name;

            public NPCOSubRecord()
            {
                Name = new char[32];
            }

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Count = r.ReadLEInt32();
                var bytes = r.ReadBytes(32);
                for (var i = 0; i < 32; i++)
                    Name[i] = Convert.ToChar(bytes[i]);
            }
        }

        public class AI_WSubRecord : SubRecord
        {
            public short Distance;
            public byte Duration;
            public byte TimeOfDay;
            public byte[] Idle;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Distance = r.ReadLEInt16();
                Duration = r.ReadByte();
                TimeOfDay = r.ReadByte();
                Idle = r.ReadBytes(10);
            }
        }

        public class AIDTSubRecord : SubRecord
        {
            public byte[] value1;

            public override void Read(UnityBinaryReader r, uint dataSize)
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