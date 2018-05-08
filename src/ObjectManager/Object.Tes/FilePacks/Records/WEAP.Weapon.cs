using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class WEAPRecord : Record
    {
        public class WPDTSubRecord : SubRecord
        {
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
            public int Flags;

            public override void Read(UnityBinaryReader r, uint dataSize)
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

        public NAMESubRecord NAME;
        public MODLSubRecord MODL;
        public FNAMSubRecord FNAM;
        public WPDTSubRecord WPDT;
        public ITEXSubRecord ITEX;
        public ENAMSubRecord ENAM;
        public SCRISubRecord SCRI;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "WPDT": WPDT = new WPDTSubRecord(); return WPDT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "ENAM": ENAM = new ENAMSubRecord(); return ENAM;
                case "SCRI": SCRI = new SCRISubRecord(); return SCRI;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}