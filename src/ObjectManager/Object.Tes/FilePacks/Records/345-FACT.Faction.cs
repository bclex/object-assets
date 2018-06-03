using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class FACTRecord : Record
    {
        // TESX
        public class RNAMGroup
        {
            public override string ToString() => $"{RNAM.Value}:{MNAM.Value}";
            public IN32Field RNAM; // rank
            public STRVField MNAM; // male
            public STRVField FNAM; // female
            public STRVField INAM; // insignia
        }

        // TES3
        public struct FADTField
        {
            public FADTField(UnityBinaryReader r, int dataSize)
            {
                r.SkipBytes(dataSize);
            }
        }

        // TES4
        public struct XNAMField
        {
            public override string ToString() => $"{FormId}";
            public int FormId;
            public int Mod;
            public int Combat;

            public XNAMField(UnityBinaryReader r, int dataSize, GameFormatId format)
            {
                FormId = r.ReadLEInt32();
                Mod = r.ReadLEInt32();
                Combat = format > GameFormatId.TES4 ? r.ReadLEInt32() : 0; // 0 - Neutral, 1 - Enemy, 2 - Ally, 3 - Friend
            }
        }

        public override string ToString() => $"FACT: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField FNAM; // Faction name
        public List<RNAMGroup> RNAMs = new List<RNAMGroup>(); // Rank Name
        public FADTField FADT; // Faction data
        public List<STRVField> ANAMs = new List<STRVField>(); // Faction name
        public List<INTVField> INTVs = new List<INTVField>(); // Faction reaction
        // TES4
        public XNAMField XNAM; // Interfaction Relations
        public INTVField DATA; // Flags (byte, uint32)
        public UI32Field CNAM;

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            if (format == GameFormatId.TES3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "RNAM": RNAMs.Add(new RNAMGroup { MNAM = new STRVField(r, dataSize) }); return true;
                    case "FADT": FADT = new FADTField(r, dataSize); return true;
                    case "ANAM": ANAMs.Add(new STRVField(r, dataSize)); return true;
                    case "INTV": INTVs.Add(new INTVField(r, dataSize)); return true;
                    default: return false;
                }
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FNAM = new STRVField(r, dataSize); return true;
                case "XNAM": XNAM = new XNAMField(r, dataSize, format); return true;
                case "DATA": DATA = new INTVField(r, dataSize); return true;
                case "CNAM": CNAM = new UI32Field(r, dataSize); return true;
                case "RNAM": RNAMs.Add(new RNAMGroup { RNAM = new IN32Field(r, dataSize) }); return true;
                case "MNAM": ArrayUtils.Last(RNAMs).MNAM = new STRVField(r, dataSize); return true;
                case "FNAM": ArrayUtils.Last(RNAMs).FNAM = new STRVField(r, dataSize); return true;
                case "INAM": ArrayUtils.Last(RNAMs).INAM = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}
