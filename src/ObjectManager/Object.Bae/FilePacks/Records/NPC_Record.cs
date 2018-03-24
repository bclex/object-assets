using OA.Core;

namespace OA.Bae.FilePacks
{
    public class NPC_Record : Record
    {
        public class RNAMSubRecord : STRVSubRecord { }
        public class KNAMSubRecord : STRVSubRecord { }
        public class NPDTSubRecord : SubRecord
        {
            public short level;
            public byte strength;
            public byte intelligence;
            public byte willpower;
            public byte agility;
            public byte speed;
            public byte endurance;
            public byte personality;
            public byte luck;
            public byte[] skills;//[27];
            public byte reputation;
            public short health;
            public short spellPts;
            public short fatigue;
            public byte disposition;
            public byte factionID;
            public byte rank;
            public byte unknown1;
            public int gold;
            public byte version;

            // 12 byte version
            //public short level;
            //public byte disposition;
            //public byte factionID;
            //public byte rank;
            //public byte unknown1;
            public byte unknown2;
            public byte unknown3;
            //public long gold;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                if (dataSize == 52)
                {
                    level = r.ReadLEInt16();
                    strength = r.ReadByte();
                    intelligence = r.ReadByte();
                    willpower = r.ReadByte();
                    agility = r.ReadByte();
                    speed = r.ReadByte();
                    endurance = r.ReadByte();
                    personality = r.ReadByte();
                    luck = r.ReadByte();
                    skills = r.ReadBytes(26);
                    reputation = r.ReadByte();
                    health = r.ReadLEInt16();
                    spellPts = r.ReadLEInt16();
                    fatigue = r.ReadLEInt16();
                    disposition = r.ReadByte();
                    factionID = r.ReadByte();
                    rank = r.ReadByte();
                    unknown1 = r.ReadByte();
                    gold = r.ReadLEInt32();
                    version = r.ReadByte();
                }
                else
                {
                    level = r.ReadLEInt16();
                    disposition = r.ReadByte();
                    factionID = r.ReadByte();
                    rank = r.ReadByte();
                    unknown1 = r.ReadByte();
                    unknown2 = r.ReadByte();
                    unknown3 = r.ReadByte();
                    gold = r.ReadLEInt32();
                }
            }
        }
        public class FLAGSubRecord : INTVSubRecord { }
        public class NPCOSubRecord : SubRecord
        {
            public int count;
            public char[] name;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                count = r.ReadLEInt32();
                var bytes = r.ReadBytes(count);
                name = new char[32];
                for (var i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
            }
        }
        public class NPCSSubRecord : SubRecord
        {
            public char[] name;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                var bytes = r.ReadBytes(32);
                name = new char[32];
                for (var i = 0; i < 32; i++)
                    name[i] = System.Convert.ToChar(bytes[i]);
            }
        }
        public class AIDTSubRecord : SubRecord
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

            public byte hello;
            public byte unknown1;
            public byte fight;
            public byte flee;
            public byte alarm;
            public byte unknown2;
            public byte unknown3;
            public byte unknown4;
            public int flags;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                hello = r.ReadByte();
                unknown1 = r.ReadByte();
                fight = r.ReadByte();
                flee = r.ReadByte();
                alarm = r.ReadByte();
                unknown2 = r.ReadByte();
                unknown3 = r.ReadByte();
                unknown4 = r.ReadByte();
                flags = r.ReadLEInt32();
            }
        }
        public class AI_WSubRecord : SubRecord
        {
            public short distance;
            public short duration;
            public byte timeOfDay;
            public byte[] idle;
            public byte unknow;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                distance = r.ReadLEInt16();
                duration = r.ReadLEInt16();
                timeOfDay = r.ReadByte();
                idle = r.ReadBytes(8);
                unknow = r.ReadByte();
            }
        }
        public class AI_TSubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public float unknown;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                x = r.ReadLESingle();
                y = r.ReadLESingle();
                z = r.ReadLESingle();
                unknown = r.ReadLESingle();
            }
        }
        public class AI_FSubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public short duration;
            public char[] id;
            public float unknown;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                x = r.ReadLESingle();
                y = r.ReadLESingle();
                z = r.ReadLESingle();
                duration = r.ReadLEInt16();
                var bytes = r.ReadBytes(32);
                id = new char[32];
                for (var i = 0; i < 32; i++)
                    id[i] = System.Convert.ToChar(bytes[i]);
                unknown = r.ReadLESingle();
            }
        }
        public class AI_ESubRecord : SubRecord
        {
            public float x;
            public float y;
            public float z;
            public short duration;
            public char[] id;
            public float unknown;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                x = r.ReadLESingle();
                y = r.ReadLESingle();
                z = r.ReadLESingle();
                duration = r.ReadLEInt16();
                var bytes = r.ReadBytes(32);
                id = new char[32];
                for (var i = 0; i < 32; i++)
                    id[i] = System.Convert.ToChar(bytes[i]);
                unknown = r.ReadLESingle();
            }
        }
        public class CNDTSubRecord : STRVSubRecord { }
        public class AI_ASubRecord : SubRecord
        {
            public char[] name;
            public byte unknown;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
            }
        }
        public class DODTSubRecord : SubRecord
        {
            public float xPos;
            public float yPos;
            public float zPos;
            public float xRot;
            public float yRot;
            public float zRot;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                xPos = r.ReadLESingle();
                yPos = r.ReadLESingle();
                zPos = r.ReadLESingle();
                xRot = r.ReadLESingle();
                yRot = r.ReadLESingle();
                zRot = r.ReadLESingle();
            }
        }
        public class DNAMSubRecord : STRVSubRecord { }
        public class XSCLSubRecord : FLTVSubRecord { }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public MODLSubRecord MODL;
        public RNAMSubRecord RNAM;
        public ANAMSubRecord ANAM;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
        public KNAMSubRecord KNAM;
        public NPDTSubRecord NPDT;
        public FLAGSubRecord FLAG;
        public NPCOSubRecord NPCO;
        public AIDTSubRecord AIDT;
        public AI_WSubRecord AI_W;
        public AI_TSubRecord AI_T;
        public AI_FSubRecord AI_F;
        public AI_ESubRecord AI_E;
        public CNDTSubRecord CNDT;
        public AI_ASubRecord AI_A;
        public DODTSubRecord DODT;
        public DNAMSubRecord DNAM;
        public XSCLSubRecord XSCL;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "RNAM": RNAM = new RNAMSubRecord(); return RNAM;
                case "ANAM": ANAM = new ANAMSubRecord(); return ANAM;
                case "BNAM": BNAM = new BNAMSubRecord(); return BNAM;
                case "CNAM": CNAM = new CNAMSubRecord(); return CNAM;
                case "KNAM": KNAM = new KNAMSubRecord(); return KNAM;
                case "NPDT": NPDT = new NPDTSubRecord(); return NPDT;
                case "FLAG": FLAG = new FLAGSubRecord(); return FLAG;
                //case "NPCO": NPCO = new NPCOSubRecord(); return NPCO;
                case "AIDT": AIDT = new AIDTSubRecord(); return AIDT;
                case "AI_W": AI_W = new AI_WSubRecord(); return AI_W;
                //case "AI_T": AI_T = new AI_TSubRecord(); return AI_T;
                //case "AI_F": AI_F = new AI_FSubRecord(); return AI_F;
                case "AI_E": AI_E = new AI_ESubRecord(); return AI_E;
                case "CNDT": CNDT = new CNDTSubRecord(); return CNDT;
                case "AI_A": AI_A = new AI_ASubRecord(); return AI_A;
                case "DODT": DODT = new DODTSubRecord(); return DODT;
                case "DNAM": DNAM = new DNAMSubRecord(); return DNAM;
                case "XSCL": XSCL = new XSCLSubRecord(); return XSCL;
            }
            return null;
        }
    }
}