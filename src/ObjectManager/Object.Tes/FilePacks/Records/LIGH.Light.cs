using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class LIGHRecord : Record
    {
        public class LHDTField : Field
        {
            public float Weight;
            public int Value;
            public int Time;
            public int Radius;
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte NullByte;
            public int Flags;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Time = r.ReadLEInt32();
                Radius = r.ReadLEInt32();
                Red = r.ReadByte();
                Green = r.ReadByte();
                Blue = r.ReadByte();
                NullByte = r.ReadByte();
                Flags = r.ReadLEInt32();
            }
        }

        public STRVField NAME;
        public STRVField FNAM;
        public LHDTField LHDT;
        public STRVField SCPT;
        public STRVField ITEX;
        public STRVField MODL;
        public STRVField SNAM;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "LHDT": LHDT = new LHDTField(); return LHDT;
                case "SCPT": SCPT = new STRVField(); return SCPT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "MODL": MODL = new STRVField(); return MODL;
                case "SNAM": SNAM = new STRVField(); return SNAM;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}