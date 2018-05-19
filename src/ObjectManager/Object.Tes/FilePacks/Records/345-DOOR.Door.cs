using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class DOORRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"DOOR: {EDID.Value}";
        public STRVField EDID { get; set; } // door ID
        public STRVField FNAM; // door name
        public FILEField MODL { get; set; } // NIF model filename
        public STRVField? SCRI; // Script (optional)
        public STRVField SNAM; // Sound name open
        public STRVField ANAM; // Sound name close

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    case "SNAM": SNAM = new STRVField(r, dataSize); return true;
                    case "ANAM": ANAM = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}