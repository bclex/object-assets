using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LVSPRecord : Record
    {
        public override string ToString() => $"LVSP: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public BYTEField LVLD; // Chance
        public BYTEField LVLF; // Flags
        public List<LVLIRecord.LVLOField> LVLOs = new List<LVLIRecord.LVLOField>(); // Number of items in list

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "LVLD": LVLD = new BYTEField(r, dataSize); return true;
                case "LVLF": LVLF = new BYTEField(r, dataSize); return true;
                case "LVLO": LVLOs.Add(new LVLIRecord.LVLOField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}