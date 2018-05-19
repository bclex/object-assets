using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class REGNRecord : Record, IHaveEDID
    {
        public struct WEATField
        {
            public byte Clear;
            public byte Cloudy;
            public byte Foggy;
            public byte Overcast;
            public byte Rain;
            public byte Thunder;
            public byte Ash;
            public byte Blight;

            public WEATField(UnityBinaryReader r, uint dataSize)
            {
                Clear = r.ReadByte();
                Cloudy = r.ReadByte();
                Foggy = r.ReadByte();
                Overcast = r.ReadByte();
                Rain = r.ReadByte();
                Thunder = r.ReadByte();
                Ash = r.ReadByte();
                Blight = r.ReadByte();
                // v1.3 ESM files add 2 bytes to WEAT subrecords.
                if (dataSize == 10)
                {
                    r.ReadByte();
                    r.ReadByte();
                }
            }
        }

        public struct SNAMField
        {
            public override string ToString() => $"{SoundName}";
            public string SoundName;
            public byte Chance;

            public SNAMField(UnityBinaryReader r, uint dataSize)
            {
                SoundName = r.ReadASCIIString(32, ASCIIFormat.ZeroPadded);
                Chance = r.ReadByte();
            }
        }

        public override string ToString() => $"REGN: {EDID.Value}";
        public STRVField EDID { get; set; } // Region ID
        public STRVField FNAM; // Region name
        public WEATField WEAT; // Weather Data
        public STRVField BNAM; // Sleep creature
        public CREFField CNAM; // Map Color (COLORREF)
        public List<SNAMField> SNAMs = new List<SNAMField>(); // Sound Record (order determines the sound priority)

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "WEAT": WEAT = new WEATField(r, dataSize); return true;
                    case "BNAM": BNAM = new STRVField(r, dataSize); return true;
                    case "CNAM": CNAM = new CREFField(r, dataSize); return true;
                    case "SNAM": SNAMs.Add(new SNAMField(r, dataSize)); return true;
                    default: return false;
                }
            return false;
        }
    }
}