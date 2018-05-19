using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class WEAPRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct WPDTField
        {
            public enum WEAPType
            {
                ShortBladeOneHand = 0,
                LongBladeOneHand = 1,
                LongBladeTwoClose = 2,
                BluntOneHand = 3,
                BluntTwoClose = 4,
                BluntTwoWide = 5,
                SpearTwoWide = 6,
                AxeOneHand = 7,
                AxeTwoHand = 8,
                MarksmanBow = 9,
                MarksmanCrossbow = 10,
                MarksmanThrown = 11,
                Arrow = 12,
                Bolt = 13,
            }

            public float Weight;
            public int Value;
            public short Type;
            public short Health;
            public float Speed;
            public float Reach;
            public short EnchantPts;
            public byte ChopMin;
            public byte ChopMax;
            public byte SlashMin;
            public byte SlashMax;
            public byte ThrustMin;
            public byte ThrustMax;
            public int Flags; // 0 = ?, 1 = Ignore Normal Weapon Resistance?

            public WPDTField(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Type = r.ReadLEInt16();
                Health = r.ReadLEInt16();
                Speed = r.ReadLESingle();
                Reach = r.ReadLESingle();
                EnchantPts = r.ReadLEInt16();
                ChopMin = r.ReadByte();
                ChopMax = r.ReadByte();
                SlashMin = r.ReadByte();
                SlashMax = r.ReadByte();
                ThrustMin = r.ReadByte();
                ThrustMax = r.ReadByte();
                Flags = r.ReadLEInt32();
            }
        }

        public override string ToString() => $"WEAP: {EDID.Value}";
        public STRVField EDID { get; set; } // item ID
        public FILEField MODL { get; set; } // model filename
        public STRVField FNAM; // item name
        public WPDTField WPDT; // Weapon Data
        public FILEField ITEX; // Inventory icon
        public STRVField ENAM; // Enchantment ID
        public STRVField SCRI; // script ID

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "WPDT": WPDT = new WPDTField(r, dataSize); return true;
                    case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                    case "ENAM": ENAM = new STRVField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}