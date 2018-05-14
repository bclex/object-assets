using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class GLOBRecord : Record, IHaveEDID
    {
        public override string ToString() => $"GLOB: {EDID.Value}";
        public STRVField EDID { get; set; } // Global ID
        public BYTEField? FNAM; // Type of global (s, l, f)
        public FLTVField? FLTV; // Float data

        public override bool CreateField(UnityBinaryReader r, string type, uint dataSize)
        {
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new BYTEField(r, dataSize); return true;
                case "FLTV": FLTV = new FLTVField(r, dataSize); return true;
                default: return false;
            }
        }

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FNAM = new BYTEField(r, dataSize); return true;
                case "FLTV": FLTV = new FLTVField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}