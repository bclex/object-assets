using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class FACTRecord : Record
    {
        public struct FADTField
        {
            //public uint AttributeID1;
            //public uint AttributeID2;
            public FADTField(UnityBinaryReader r, uint dataSize)
            {
                r.ReadBytes((int)dataSize);
            }
        }

        public override string ToString() => $"FACT: {EDID.Value}";
        public STRVField EDID { get; set; } // Faction ID
        public STRVField FNAM; // Faction name
        public List<STRVField> RNAMs = new List<STRVField>(); // Rank Name
        public FADTField FADT; // Faction data
        public List<STRVField> ANAMs = new List<STRVField>(); // Faction name
        public List<INTVField> INTVs = new List<INTVField>(); // Faction reaction

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "RNAM": RNAMs.Add(new STRVField(r, dataSize)); return true;
                case "FADT": FADT = new FADTField(r, dataSize); return true;
                case "ANAM": ANAMs.Add(new STRVField(r, dataSize)); return true;
                case "INTV": INTVs.Add(new INTVField(r, dataSize)); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}
