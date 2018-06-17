using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class CLMTRecord : Record, IHaveEDID, IHaveMODL
    {
        public struct WLSTField
        {
            public FormId<WTHRRecord> Weather;
            public int Chance;

            public WLSTField(UnityBinaryReader r, int dataSize)
            {
                Weather = new FormId<WTHRRecord>(r.ReadLEUInt32());
                Chance = r.ReadLEInt32();
            }
        }

        public struct TNAMField
        {
            public byte Sunrise_Begin;
            public byte Sunrise_End;
            public byte Sunset_Begin;
            public byte Sunset_End;
            public byte Volatility;
            public byte MoonsPhaseLength;

            public TNAMField(UnityBinaryReader r, int dataSize)
            {
                Sunrise_Begin = r.ReadByte();
                Sunrise_End = r.ReadByte();
                Sunset_Begin = r.ReadByte();
                Sunset_End = r.ReadByte();
                Volatility = r.ReadByte();
                MoonsPhaseLength = r.ReadByte();
            }
        }

        public override string ToString() => $"CLMT: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public FILEField FNAM; // Sun Texture
        public FILEField GNAM; // Sun Glare Texture
        public List<WLSTField> WLSTs = new List<WLSTField>(); // Climate
        public TNAMField TNAM; // Timing

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "FNAM": FNAM = new FILEField(r, dataSize); return true;
                case "GNAM": GNAM = new FILEField(r, dataSize); return true;
                case "WLST": for (var i = 0; i < dataSize >> 3; i++) WLSTs.Add(new WLSTField(r, dataSize)); return true;
                case "TNAM": TNAM = new TNAMField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}