using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class APPARecord : Record
    {
        public class AADTField : Field
        {
            public int Type;
            public float Quality;
            public float Weight;
            public int Value;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Quality = r.ReadLESingle();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public AADTField AADT;
        public STRVField ITEX;
        public STRVField SCRI;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "AADT": AADT = new AADTField(); return AADT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}