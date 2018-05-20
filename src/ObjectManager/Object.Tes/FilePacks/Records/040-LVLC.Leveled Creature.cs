using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class LVLCRecord : Record
    {
        public struct LVLOField
        {
            public short Level;
            public FormId<Record> ItemFormId;
            public int Count;

            public LVLOField(UnityBinaryReader r, uint dataSize)
            {
                Level = r.ReadLEInt16();
                r.ReadBytes(2); // Unused
                ItemFormId.Id = r.ReadLEUInt32();
                ItemFormId.Name = null;
                if (dataSize == 12)
                {
                    Count = r.ReadLEInt16();
                    r.ReadBytes(2); // Unused
                }
                else Count = 0;
            }
        }

        public override string ToString() => $"LVLC: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public BYTEField LVLD; // Chance
        public BYTEField LVLF; // Flags - 0x01 = Calculate from all levels <= player's level, 0x02 = Calculate for each item in count
        public FMIDField<SCPTRecord> SCRI; // Script (optional)
        public FMIDField<CREARecord> TNAM; // Creature Template (optional)
        public List<LVLOField> LVLOs = new List<LVLOField>();

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "LVLD": LVLD = new BYTEField(r, dataSize); return true;
                case "LVLF": LVLF = new BYTEField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "TNAM": TNAM = new FMIDField<CREARecord>(r, dataSize); return true;
                case "LVLO": LVLOs.Add(new LVLOField(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}