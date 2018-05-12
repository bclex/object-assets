using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class CREARecord : Record
    {
        public class NPDTField : Field
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

        public class NPCOField : Field
        {
            public int Count;
            public char[] Name;

            public NPCOField()
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

        public class AI_WField : Field
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

        public class AIDTField : Field
        {
            public byte[] value1;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                value1 = r.ReadBytes(12);
            }
        }

        public class XSCLField : FLTVField { }

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

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public NPDTField NPDT;
        public Int32Field FLAG;
        public STRVField SCRI;
        public NPCOField NPCO;
        public AIDTField AIDT;
        public AI_WField AI_W;
        public NPC_Record.AI_TField AI_T;
        public NPC_Record.AI_FField AI_F;
        public NPC_Record.AI_EField AI_E;
        public NPC_Record.AI_AField AI_A;
        public XSCLField XSCL;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "NPDT": NPDT = new NPDTField(); return NPDT;
                case "FLAG": FLAG = new Int32Field(); return FLAG;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                case "NPCO": NPCO = new NPCOField(); return NPCO;
                case "AIDT": AIDT = new AIDTField(); return AIDT;
                case "AI_W": AI_W = new AI_WField(); return AI_W;
                /* case "AI_T": AI_T = new NPC_Record.AI_TSubRecord(); return AI_T;
                 case "AI_F": AI_F = new NPC_Record.AI_FSubRecord(); return AI_F;
                 case "AI_E": AI_E = new NPC_Record.AI_ESubRecord(); return AI_E;
                 case "AI_A": AI_A = new NPC_Record.AI_ASubRecord(); return AI_A;*/
                case "XSCL": XSCL = new XSCLField(); return XSCL;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}