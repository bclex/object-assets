using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class DOORRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"DOOR: {EDID.Value}";
        public STRVField EDID { get; set; } // door ID
        public STRVField FULL; // door name
        public MODLGroup MODL { get; set; } // NIF model filename
        public FMIDField<SCPTRecord>? SCRI; // Script (optional)
        public FMIDField<SOUNRecord> SNAM; // Open sound
        public FMIDField<SOUNRecord> ANAM; // Close sound
        // TES4
        public FMIDField<SOUNRecord> BNAM; // Loop sound
        public BYTEField FNAM; // Flags
        public FMIDField<Record> TNAM; // Random teleport destination

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "FNAM": if (formatId != GameFormatId.Tes3) FNAM = new BYTEField(r, dataSize); else FULL = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "SNAM": SNAM = new FMIDField<SOUNRecord>(r, dataSize); return true;
                case "ANAM": ANAM = new FMIDField<SOUNRecord>(r, dataSize); return true;
                case "BNAM": ANAM = new FMIDField<SOUNRecord>(r, dataSize); return true;
                case "TNAM": TNAM = new FMIDField<Record>(r, dataSize); return true;
                default: return false;
            }
        }
    }
}