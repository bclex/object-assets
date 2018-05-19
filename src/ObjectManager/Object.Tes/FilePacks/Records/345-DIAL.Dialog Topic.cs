using OA.Core;
using System;
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
        public STRVField EDID { get; set; } // Dialogue ID
        public BYTEField DATA; // Dialogue Type
        public List<INFORecord> INFOs = new List<INFORecord>(); // Info Records

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); LastRecord = this; return true;
                    case "DATA": DATA = new BYTEField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}
