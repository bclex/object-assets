using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LVLCRecord : Record
    {
        public override string ToString() => $"LVLC: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public BYTEField LVLD; // Chance
        public BYTEField LVLF; // Flags - 0x01 = Calculate from all levels <= player's level, 0x02 = Calculate for each item in count
        public FMIDField<SCPTRecord> SCRI; // Script (optional)
        public FMIDField<CREARecord> TNAM; // Creature Template (optional)
        public List<LVLIRecord.LVLOField> LVLOs = new List<LVLIRecord.LVLOField>();

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "LVLD": LVLD = new BYTEField(r, dataSize); return true;
                case "LVLF": LVLF = new BYTEField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "TNAM": TNAM = new FMIDField<CREARecord>(r, dataSize); return true;
                case "LVLO": LVLOs.Add(new LVLIRecord.LVLOField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}