using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class NPC_Record : Record
    {
        public class NPDTField : Field
        {
            public short Level;
            public byte Strength;
            public byte Intelligence;
            public byte Willpower;
            public byte Agility;
            public byte Speed;
            public byte Endurance;
            public byte Personality;
            public byte Luck;
            public byte[] Skills;//[27];
            public byte Reputation;
            public short Health;
            public short SpellPts;
            public short Fatigue;
            public byte Disposition;
            public byte FactionId;
            public byte Rank;
            public byte Unknown1;
            public int Gold;
            public byte Version;

            // 12 byte version
            //public short Level;
            //public byte Disposition;
            //public byte FactionId;
            //public byte Rank;
            //public byte Unknown1;
            public byte Unknown2;
            public byte Unknown3;
            //public long Gold;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                if (dataSize == 52)
                {
                    Level = r.ReadLEInt16();
                    Strength = r.ReadByte();
                    Intelligence = r.ReadByte();
                    Willpower = r.ReadByte();
                    Agility = r.ReadByte();
                    Speed = r.ReadByte();
                    Endurance = r.ReadByte();
                    Personality = r.ReadByte();
                    Luck = r.ReadByte();
                    Skills = r.ReadBytes(26);
                    Reputation = r.ReadByte();
                    Health = r.ReadLEInt16();
                    SpellPts = r.ReadLEInt16();
                    Fatigue = r.ReadLEInt16();
                    Disposition = r.ReadByte();
                    FactionId = r.ReadByte();
                    Rank = r.ReadByte();
                    Unknown1 = r.ReadByte();
                    Gold = r.ReadLEInt32();
                    Version = r.ReadByte();
                }
                else
                {
                    Level = r.ReadLEInt16();
                    Disposition = r.ReadByte();
                    FactionId = r.ReadByte();
                    Rank = r.ReadByte();
                    Unknown1 = r.ReadByte();
                    Unknown2 = r.ReadByte();
                    Unknown3 = r.ReadByte();
                    Gold = r.ReadLEInt32();
                }
            }
        }
        public class NPCOField : Field
        {
            public int Count;
            public char[] Name;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Count = r.ReadLEInt32();
                var bytes = r.ReadBytes(Count);
                Name = new char[32];
                for (var i = 0; i < 32; i++)
                    Name[i] = System.Convert.ToChar(bytes[i]);
            }
        }
        public class NPCSField : Field
        {
            public char[] Name;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                var bytes = r.ReadBytes(32);
                Name = new char[32];
                for (var i = 0; i < 32; i++)
                    Name[i] = System.Convert.ToChar(bytes[i]);
            }
        }
        public class AIDTField : Field
        {
            public enum FlagsType
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
            public int Flags;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Hello = r.ReadByte();
                Unknown1 = r.ReadByte();
                Fight = r.ReadByte();
                Flee = r.ReadByte();
                Alarm = r.ReadByte();
                Unknown2 = r.ReadByte();
                Unknown3 = r.ReadByte();
                Unknown4 = r.ReadByte();
                Flags = r.ReadLEInt32();
            }
        }
        public class AI_WField : Field
        {
            public short Distance;
            public short Duration;
            public byte TimeOfDay;
            public byte[] Idle;
            public byte Unknow;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Distance = r.ReadLEInt16();
                Duration = r.ReadLEInt16();
                TimeOfDay = r.ReadByte();
                Idle = r.ReadBytes(8);
                Unknow = r.ReadByte();
            }
        }
        public class AI_TField : Field
        {
            public float X;
            public float Y;
            public float Z;
            public float Unknown;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                X = r.ReadLESingle();
                Y = r.ReadLESingle();
                Z = r.ReadLESingle();
                Unknown = r.ReadLESingle();
            }
        }
        public class AI_FField : Field
        {
            public float X;
            public float Y;
            public float Z;
            public short Duration;
            public char[] Id;
            public float Unknown;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                X = r.ReadLESingle();
                Y = r.ReadLESingle();
                Z = r.ReadLESingle();
                Duration = r.ReadLEInt16();
                var bytes = r.ReadBytes(32);
                Id = new char[32];
                for (var i = 0; i < 32; i++)
                    Id[i] = System.Convert.ToChar(bytes[i]);
                Unknown = r.ReadLESingle();
            }
        }
        public class AI_EField : Field
        {
            public float X;
            public float Y;
            public float Z;
            public short Duration;
            public char[] Id;
            public float Unknown;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                X = r.ReadLESingle();
                Y = r.ReadLESingle();
                Z = r.ReadLESingle();
                Duration = r.ReadLEInt16();
                var bytes = r.ReadBytes(32);
                Id = new char[32];
                for (var i = 0; i < 32; i++)
                    Id[i] = System.Convert.ToChar(bytes[i]);
                Unknown = r.ReadLESingle();
            }
        }
        public class AI_AField : Field
        {
            public char[] Name;
            public byte Unknown;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
            }
        }
        public class DODTField : Field
        {
            public float XPos;
            public float YPos;
            public float ZPos;
            public float XRot;
            public float YRot;
            public float ZRot;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                XPos = r.ReadLESingle();
                YPos = r.ReadLESingle();
                ZPos = r.ReadLESingle();
                XRot = r.ReadLESingle();
                YRot = r.ReadLESingle();
                ZRot = r.ReadLESingle();
            }
        }

        public STRVField NAME;
        public STRVField FNAM;
        public STRVField MODL;
        public STRVField RNAM;
        public STRVField ANAM;
        public STRVField BNAM;
        public STRVField CNAM;
        public STRVField KNAM;
        public NPDTField NPDT;
        public INTVField FLAG;
        public NPCOField NPCO;
        public AIDTField AIDT;
        public AI_WField AI_W;
        public AI_TField AI_T;
        public AI_FField AI_F;
        public AI_EField AI_E;
        public STRVField CNDT;
        public AI_AField AI_A;
        public DODTField DODT;
        public STRVField DNAM;
        public FLTVField XSCL;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "MODL": MODL = new STRVField(); return MODL;
                case "RNAM": RNAM = new STRVField(); return RNAM;
                case "ANAM": ANAM = new STRVField(); return ANAM;
                case "BNAM": BNAM = new STRVField(); return BNAM;
                case "CNAM": CNAM = new STRVField(); return CNAM;
                case "KNAM": KNAM = new STRVField(); return KNAM;
                case "NPDT": NPDT = new NPDTField(); return NPDT;
                case "FLAG": FLAG = new INTVField(); return FLAG;
                //case "NPCO": NPCO = new NPCOSubRecord(); return NPCO;
                case "AIDT": AIDT = new AIDTField(); return AIDT;
                case "AI_W": AI_W = new AI_WField(); return AI_W;
                //case "AI_T": AI_T = new AI_TSubRecord(); return AI_T;
                //case "AI_F": AI_F = new AI_FSubRecord(); return AI_F;
                case "AI_E": AI_E = new AI_EField(); return AI_E;
                case "CNDT": CNDT = new STRVField(); return CNDT;
                case "AI_A": AI_A = new AI_AField(); return AI_A;
                case "DODT": DODT = new DODTField(); return DODT;
                case "DNAM": DNAM = new STRVField(); return DNAM;
                case "XSCL": XSCL = new FLTVField(); return XSCL;
            }
            return null;
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}