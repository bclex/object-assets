using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CLOTRecord : Record
    {
        public class CTDTField : Field
        {
            public int Type;
            public float Weight;
            public short Value;
            public short EnchantPts;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Type = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt16();
                EnchantPts = r.ReadLEInt16();
            }
        }

        public STRVField NAME;
        public STRVField MODL;
        public STRVField FNAM;
        public CTDTField CTDT;
        public STRVField ITEX;

        public List<INDXBNAMCNAMGroup> INDXBNAMCNAMGroups = new List<INDXBNAMCNAMGroup>();

        public STRVField ENAM;
        public STRVField SCRI;

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "MODL": MODL = new STRVField(); return MODL;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "CTDT": CTDT = new CTDTField(); return CTDT;
                case "ITEX": ITEX = new STRVField(); return ITEX;
                case "INDX": var INDX = new INTVField(); var group = new INDXBNAMCNAMGroup { INDX = INDX }; INDXBNAMCNAMGroups.Add(group); return INDX;
                case "BNAM": var BNAM = new STRVField(); ArrayUtils.Last(INDXBNAMCNAMGroups).BNAM = BNAM; return BNAM;
                case "CNAM": var CNAM = new STRVField(); ArrayUtils.Last(INDXBNAMCNAMGroups).CNAM = CNAM; return CNAM;
                case "ENAM": ENAM = new STRVField(); return ENAM;
                case "SCRI": SCRI = new STRVField(); return SCRI;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}