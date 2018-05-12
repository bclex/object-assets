using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class BOOKRecord : Record
    {
        public class BKDTField : Field
        {
            public float Weight;
            public int Value;
            public int Scroll;
            public int SkillID;
            public int EnchantPts;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Scroll = r.ReadLEInt32();
                SkillID = r.ReadLEInt32();
                EnchantPts = r.ReadLEInt32();
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public BKDTField BKDT;
        public STRVField ITEX;
        public STRVField SCRI;
        public STRVField TEXT;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "BKDT": BKDT = new BKDTField(); return BKDT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                case "TEXT": TEXT = new STRVField(); return TEXT;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId formatId) => throw new NotImplementedException();
    }
}
