using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class LTEXRecord : Record, IHaveEDID
    {
        public override string ToString() => $"LTEX: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public INTVField INTV;
        public STRVField DATA;

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "INTV": INTV = new INTVField(r, dataSize); return true;
                case "DATA": DATA = new STRVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}