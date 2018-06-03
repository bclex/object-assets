using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class IDLERecord : Record
    {
        public override string ToString() => $"IDLE: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL;
        public List<SCPTRecord.CTDAField> CTDAs = new List<SCPTRecord.CTDAField>(); // Conditions
        public BYTEField ANAM;
        public FMIDField<IDLERecord>[] DATAs;

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "CTDA":
                case "CTDT": CTDAs.Add(new SCPTRecord.CTDAField(r, dataSize, format)); return true;
                case "ANAM": ANAM = new BYTEField(r, dataSize); return true;
                case "DATA": DATAs = new FMIDField<IDLERecord>[dataSize >> 2]; for (var i = 0; i < DATAs.Length; i++) DATAs[i] = new FMIDField<IDLERecord>(r, 4); return true;
                    
                default: return false;
            }
        }
    }
}