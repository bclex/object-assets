using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LVLIRecord : Record
    {
        public struct LVLOField
        {
            public short Level;
            public FormId<Record> ItemFormId;
            public int Count;

            public LVLOField(UnityBinaryReader r, int dataSize)
            {
                Level = r.ReadLEInt16();
                r.SkipBytes(2); // Unused
                ItemFormId = new FormId<Record>(r.ReadLEUInt32());
                if (dataSize == 12)
                {
                    Count = r.ReadLEInt16();
                    r.SkipBytes(2); // Unused
                }
                else Count = 0;
            }
        }

        public override string ToString() => $"LVLI: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public BYTEField LVLD; // Chance
        public BYTEField LVLF; // Flags - 0x01 = Calculate from all levels <= player's level, 0x02 = Calculate for each item in count
        public BYTEField? DATA; // Data (optional)
        public List<LVLOField> LVLOs = new List<LVLOField>();

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "LVLD": LVLD = new BYTEField(r, dataSize); return true;
                case "LVLF": LVLF = new BYTEField(r, dataSize); return true;
                case "DATA": DATA = new BYTEField(r, dataSize); return true;
                case "LVLO": LVLOs.Add(new LVLOField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}