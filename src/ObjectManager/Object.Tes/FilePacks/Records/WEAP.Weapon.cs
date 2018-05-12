using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class WEAPRecord : Record
    {
        public class WPDTField : Field
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

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public WPDTField WPDT;
        public STRVField ITEX;
        public STRVField ENAM;
        public STRVField SCRI;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "WPDT": WPDT = new WPDTField(); return WPDT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "ENAM": ENAM = new STRVField(); return ENAM;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}