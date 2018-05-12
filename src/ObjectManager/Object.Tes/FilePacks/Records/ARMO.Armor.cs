using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class ARMORecord : Record
    {
        public class AODTField : Field
        {
            public int Type;
            public float Weight;
            public int Value;
            public int Health;
            public int EnchantPts;
            public int Armour;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Health = r.ReadLEInt32();
                EnchantPts = r.ReadLEInt32();
                Armour = r.ReadLEInt32();
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public AODTField AODT;
        public STRVField ITEX;

        public List<INDXBNAMCNAMGroup> INDXBNAMCNAMGroups = new List<INDXBNAMCNAMGroup>();

        public STRVField SCRI;
        public STRVField ENAM;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "AODT": AODT = new AODTField(); return AODT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "INDX": var INDX = new INTVField(); var group = new INDXBNAMCNAMGroup { INDX = INDX }; INDXBNAMCNAMGroups.Add(group); return INDX;
                case "BNAM": var BNAM = new STRVField(); ArrayUtils.Last(INDXBNAMCNAMGroups).BNAM = BNAM; return BNAM;
                case "CNAM": var CNAM = new STRVField(); ArrayUtils.Last(INDXBNAMCNAMGroups).CNAM = CNAM; return CNAM;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                case "ENAM": ENAM = new STRVField(); return ENAM;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}