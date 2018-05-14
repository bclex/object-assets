using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class STATRecord : Record, IHaveEDID, IHaveMODL
    {
        public override string ToString() => $"STAT: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public FILEField MODL { get; set; } // NIF model

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new FILEField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}