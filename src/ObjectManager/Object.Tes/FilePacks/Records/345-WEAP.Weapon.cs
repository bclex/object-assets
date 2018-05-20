using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class WEAPRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct DATAField
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
            public ushort Type;
            public short Health;
            public float Speed;
            public float Reach;
            public short Damage; //: EnchantPts;
            public byte ChopMin;
            public byte ChopMax;
            public byte SlashMin;
            public byte SlashMax;
            public byte ThrustMin;
            public byte ThrustMax;
            public int Flags; // 0 = ?, 1 = Ignore Normal Weapon Resistance?

            public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
            {
                if (formatId == GameFormatId.Tes3)
                {
                    Weight = r.ReadLESingle();
                    Value = r.ReadLEInt32();
                    Type = r.ReadLEUInt16();
                    Health = r.ReadLEInt16();
                    Speed = r.ReadLESingle();
                    Reach = r.ReadLESingle();
                    Damage = r.ReadLEInt16();
                    ChopMin = r.ReadByte();
                    ChopMax = r.ReadByte();
                    SlashMin = r.ReadByte();
                    SlashMax = r.ReadByte();
                    ThrustMin = r.ReadByte();
                    ThrustMax = r.ReadByte();
                    Flags = r.ReadLEInt32();
                    return;
                }
                Type = (ushort)r.ReadLEUInt32();
                Speed = r.ReadLESingle();
                Reach = r.ReadLESingle();
                Flags = r.ReadLEInt32();
                Value = r.ReadLEInt32();
                Health = (short)r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Damage = r.ReadLEInt16();
                ChopMin = 0;
                ChopMax = 0;
                SlashMin = 0;
                SlashMax = 0;
                ThrustMin = 0;
                ThrustMax = 0;
            }
        }

        public override string ToString() => $"WEAP: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public STRVField FULL; // Item Name
        public DATAField DATA; // Weapon Data
        public FILEField ICON; // Male Icon (optional)
        public FMIDField<ENCHRecord> ENAM; // Enchantment ID
        public FMIDField<SCPTRecord> SCRI; // Script (optional)
        // TES4
        public IN16Field? ANAM; // Enchantment points (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "FULL":
                case "FNAM": FULL = new STRVField(r, dataSize); return true;
                case "DATA":
                case "WPDT": DATA = new DATAField(r, dataSize, formatId); return true;
                case "ICON":
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "ENAM": ENAM = new FMIDField<ENCHRecord>(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "ANAM": ANAM = new IN16Field(r, dataSize); return true;
                default: return false;
            }
        }
    }
}