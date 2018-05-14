using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class ACTIRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"ACTI: {EDID.Value}";
        public STRVField EDID { get; set; } // Item ID
        public FILEField MODL { get; set; } // Model Name
        public STRVField FNAM; // Item Name
        public STRVField SCRI; // Script Name

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}