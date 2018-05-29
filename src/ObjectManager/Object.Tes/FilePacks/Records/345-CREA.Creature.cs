using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CREARecord : Record, IHaveEDID, IHaveMODL
    {
        [Flags]
        public enum CREAFlags : uint
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

        public struct NPDTField
        {
            public int Type; // 0 = Creature, 1 = Daedra, 2 = Undead, 3 = Humanoid
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

            public NPDTField(UnityBinaryReader r, int dataSize)
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

        public struct AIDTField
        {
            public enum AIFlags : uint
            {
                Weapon = 0x00001,
                Armor = 0x00002,
                Clothing = 0x00004,
                Books = 0x00008,
                Ingrediant = 0x00010,
                Picks = 0x00020,
                Probes = 0x00040,
                Lights = 0x00080,
                Apparatus = 0x00100,
                Repair = 0x00200,
                Misc = 0x00400,
                Spells = 0x00800,
                MagicItems = 0x01000,
                Potions = 0x02000,
                Training = 0x04000,
                Spellmaking = 0x08000,
                Enchanting = 0x10000,
                RepairItem = 0x20000
            }

            public byte Hello;
            public byte Unknown1;
            public byte Fight;
            public byte Flee;
            public byte Alarm;
            public byte Unknown2;
            public byte Unknown3;
            public byte Unknown4;
            public uint Flags;

            public AIDTField(UnityBinaryReader r, int dataSize)
            {
                Hello = r.ReadByte();
                Unknown1 = r.ReadByte();
                Fight = r.ReadByte();
                Flee = r.ReadByte();
                Alarm = r.ReadByte();
                Unknown2 = r.ReadByte();
                Unknown3 = r.ReadByte();
                Unknown4 = r.ReadByte();
                Flags = r.ReadLEUInt32();
            }
        }

        public struct AI_WField
        {
            public short Distance;
            public short Duration;
            public byte TimeOfDay;
            public byte[] Idle;
            public byte Unknown;

            public AI_WField(UnityBinaryReader r, int dataSize, byte mode)
            {
                Distance = r.ReadLEInt16();
                Duration = r.ReadLEInt16();
                TimeOfDay = r.ReadByte();
                Idle = r.ReadBytes(8);
                Unknown = r.ReadByte();
            }
        }

        public struct AI_TField
        {
            public float X;
            public float Y;
            public float Z;
            public float Unknown;

            public AI_TField(UnityBinaryReader r, int dataSize)
            {
                X = r.ReadLESingle();
                Y = r.ReadLESingle();
                Z = r.ReadLESingle();
                Unknown = r.ReadLESingle();
            }
        }

        public struct AI_FField
        {
            public float X;
            public float Y;
            public float Z;
            public short Duration;
            public string Id;
            public short Unknown;

            public AI_FField(UnityBinaryReader r, int dataSize)
            {
                X = r.ReadLESingle();
                Y = r.ReadLESingle();
                Z = r.ReadLESingle();
                Duration = r.ReadLEInt16();
                Id = r.ReadASCIIString(32, ASCIIFormat.ZeroPadded);
                Unknown = r.ReadLEInt16();
            }
        }

        public struct AI_AField
        {
            public string Name;
            public byte Unknown;

            public AI_AField(UnityBinaryReader r, int dataSize)
            {
                Name = r.ReadASCIIString(32, ASCIIFormat.ZeroPadded);
                Unknown = r.ReadByte();
            }
        }

        public override string ToString() => $"CREA: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // NIF Model
        public STRVField FNAM; // Creature name
        public NPDTField NPDT; // Creature data
        public IN32Field FLAG; // Creature Flags
        public FMIDField<SCPTRecord> SCRI; // Script
        public CNTOField NPCO; // Item record
        public AIDTField AIDT; // AI data
        public AI_WField AI_W; // AI Wander
        public AI_TField? AI_T; // AI Travel
        public AI_FField? AI_F; // AI Follow
        public AI_FField? AI_E; // AI Escort
        public AI_AField? AI_A; // AI Activate
        public FLTVField? XSCL; // Scale (optional), Only present if the scale is not 1.0
        public STRVField? CNAM;
        public List<STRVField> NPCSs = new List<STRVField>();

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            if (formatId == GameFormatId.TES3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "NPDT": NPDT = new NPDTField(r, dataSize); return true;
                    case "FLAG": FLAG = new IN32Field(r, dataSize); return true;
                    case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                    case "NPCO": NPCO = new CNTOField(r, dataSize, formatId); return true;
                    case "AIDT": AIDT = new AIDTField(r, dataSize); return true;
                    case "AI_W": AI_W = new AI_WField(r, dataSize, 0); return true;
                    case "AI_T": AI_T = new AI_TField(r, dataSize); return true;
                    case "AI_F": AI_F = new AI_FField(r, dataSize); return true;
                    case "AI_E": AI_E = new AI_FField(r, dataSize); return true;
                    case "AI_A": AI_A = new AI_AField(r, dataSize); return true;
                    case "XSCL": XSCL = new FLTVField(r, dataSize); return true;
                    case "CNAM": CNAM = new STRVField(r, dataSize); return true;
                    case "NPCS": NPCSs.Add(new STRVField(r, dataSize, ASCIIFormat.ZeroPadded)); return true;
                    default: return false;
                }
            return false;
        }
    }
}