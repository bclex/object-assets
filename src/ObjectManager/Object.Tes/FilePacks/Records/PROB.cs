using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class PROBRecord : Record
    {
        public class PBDTField : Field
        {
            public float Weight;
            public int Value;
            public float Quality;
            public int Uses;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Quality = r.ReadLESingle();
                Uses = r.ReadLEInt32();
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public PBDTField PBDT;
        public STRVField ITEX;
        public STRVField SCRI;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "PBDT": PBDT = new PBDTField(); return PBDT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}