using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class REPARecord : Record
    {
        public class RIDTField : Field
        {
            public float Weight;
            public int Value;
            public int Uses;
            public float Quality;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Uses = r.ReadLEInt32();
                Quality = r.ReadLESingle();
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public RIDTField RIDT;
        public STRVField ITEX;
        public STRVField SCRI;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "RIDT": RIDT = new RIDTField(); return RIDT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}