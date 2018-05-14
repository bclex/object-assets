using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class ENCHRecord : Record
    {
        public struct ENDTField
        {
            public int Type; // 0 = Cast Once, 1 = Cast Strikes, 2 = Cast when Used, 3 = Constant Effect
            public int EnchantCost;
            public int Charge;
            public int AutoCalc;

            public ENDTField(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                EnchantCost = r.ReadLEInt32();
                Charge = r.ReadLEInt32();
                AutoCalc = r.ReadLEInt32();
            }
        }

        public struct ENAMField
        {
            public short EffectId;
            public byte SkillId; // (-1 if NA)
            public byte AttributeId; // (-1 if NA)
            public int RangeType; // 0 = Self, 1 = Touch, 2 = Target
            public int Area;
            public int Duration;
            public int MagMin;
            public int MagMax;

            public ENAMField(UnityBinaryReader r, uint dataSize)
            {
                EffectId = r.ReadLEInt16();
                SkillId = r.ReadByte();
                AttributeId = r.ReadByte();
                RangeType = r.ReadLEInt32();
                Area = r.ReadLEInt32();
                Duration = r.ReadLEInt32();
                MagMin = r.ReadLEInt32();
                MagMax = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"ENCH: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public ENDTField ENDT; // Enchant Data
        public ENAMField ENAM; // Single enchantment

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "ENDT": ENDT = new ENDTField(r, dataSize); return true;
                case "ENAM": ENAM = new ENAMField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}
