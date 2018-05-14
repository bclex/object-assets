using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class SOUNRecord : Record, IHaveEDID
    {
        public struct DATAField
        {
            public byte Volume; // (0=0.00, 255=1.00)
            public byte MinRange;
            public byte MaxRange;

            public DATAField(UnityBinaryReader r, uint dataSize)
            {
                Volume = r.ReadByte();
                MinRange = r.ReadByte();
                MaxRange = r.ReadByte();
            }
        }

        public override string ToString() => $"SOUN: {EDID.Value}";
        public STRVField EDID { get; set; } // Sound ID
        public FILEField FNAM; // Sound Filename (relative to Sounds\)
        public DATAField DATA; // Sound Data

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new FILEField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize) => throw new NotImplementedException();
    }
}