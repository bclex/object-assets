using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class DIALRecord : Record
    {
        internal static DIALRecord LastRecord;

        public enum DIALType : byte
        {
            RegularTopic = 0,
            Voice = 1,
            Greeting = 2,
            Persuasion = 3,
            Journal = 4,
        }

        public override string ToString() => $"DIAL: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField FULL; // Dialogue Name
        public BYTEField DATA; // Dialogue Type
        public List<FMIDField<QUSTRecord>> QSTIs; // Quests (optional)
        public List<INFORecord> INFOs = new List<INFORecord>(); // Info Records

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); LastRecord = this; return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new BYTEField(r, dataSize); return true;
                case "QSTI":
                case "QSTR": if (QSTIs == null) QSTIs = new List<FMIDField<QUSTRecord>>(); QSTIs.Add(new FMIDField<QUSTRecord>(r, dataSize)); return true;
                default: return false;
            }
        }
    }
}
