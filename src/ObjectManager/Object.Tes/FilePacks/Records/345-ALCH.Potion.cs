using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class ALCHRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct ALDTField
        {
            public float Weight;
            public int Value;
            public int AutoCalc;

            public ALDTField(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                AutoCalc = r.ReadLEInt32();
            }
        }

        public struct ENAMField
        {
            public short EffectId;
            public byte SkillId; // for skill related effects, -1/0 otherwise
            public byte AttributeId; // for attribute related effects, -1/0 otherwise
            public int Unknown1;
            public int Unknown2;
            public int Duration;
            public int Magnitude;
            public int Unknown4;

            public ENAMField(UnityBinaryReader r, uint dataSize)
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

        public override string ToString() => $"ALCH: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public ALDTField ALDT; // Alchemy Data
        public ENAMField ENAM; // Enchantment
        public FILEField TEXT; // Inventory Icon
        public STRVField SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "ALDT": ALDT = new ALDTField(r, dataSize); return true;
                    case "ENAM": ENAM = new ENAMField(r, dataSize); return true;
                    case "TEXT": TEXT = new FILEField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}