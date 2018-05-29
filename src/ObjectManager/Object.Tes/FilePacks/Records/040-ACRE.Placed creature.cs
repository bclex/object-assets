using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class ACRERecord : Record
    {
        public override string ToString() => $"GMST: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public FMIDField<Record> NAME; // Base
        public REFRRecord.DATAField DATA; // Position/Rotation
        public List<CELLRecord.XOWNGroup> XOWNs; // Ownership (optional)
        public REFRRecord.XESPField? XESP; // Enable Parent (optional)
        public FLTVField? XSCL; // Scale (optional)
        public BYTVField? XRGD; // Ragdoll Data (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "NAME": NAME = new FMIDField<Record>(r, dataSize); return true;
                case "DATA": DATA = new REFRRecord.DATAField(r, dataSize); return true;
                case "XOWN": if (XOWNs == null) XOWNs = new List<CELLRecord.XOWNGroup>(); XOWNs.Add(new CELLRecord.XOWNGroup { XOWN = new FMIDField<Record>(r, dataSize) }); return true;
                case "XRNK": ArrayUtils.Last(XOWNs).XRNK = new IN32Field(r, dataSize); return true;
                case "XGLB": ArrayUtils.Last(XOWNs).XGLB = new FMIDField<Record>(r, dataSize); return true;
                case "XESP": XESP = new REFRRecord.XESPField(r, dataSize); return true;
                case "XSCL": XSCL = new FLTVField(r, dataSize); return true;
                case "XRGD": XRGD = new BYTVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}