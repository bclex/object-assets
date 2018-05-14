using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class ASPCRecord : Record
    {
        public override string ToString() => $"ASPC: {EDID.Value}";
        public STRVField EDID; // Action Name
        public CREFField CNAME; // RGB color

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize) => throw new NotImplementedException();
        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "CNAME": CNAME = new CREFField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}