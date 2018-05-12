using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class REGNRecord : Record
    {
        public class WEATField : Field
        {
            public byte Clear;
            public byte Cloudy;
            public byte Foggy;
            public byte Overcast;
            public byte Rain;
            public byte Thunder;
            public byte Ash;
            public byte Blight;

            public override void Read(UnityBinaryReader r, uint dataSize)
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
        public class CNAMField : Field
        {
            byte Red;
            byte Green;
            byte Blue;
            byte NullByte;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Red = r.ReadByte();
                Green = r.ReadByte();
                Blue = r.ReadByte();
                NullByte = r.ReadByte();
            }
        }
        public class SNAMField : Field
        {
            byte[] SoundName;
            byte Chance;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                SoundName = r.ReadBytes(32);
                Chance = r.ReadByte();
            }
        }

        public STRVField NAME;
        public STRVField FNAM;
        public WEATField WEAT;
        public STRVField BNAM;
        public CNAMField CNAM;
        public List<SNAMField> SNAMs = new List<SNAMField>();

        public override Field CreateField(string type)
        {
            switch (type)
            {
                case "NAME": NAME = new STRVField(); return NAME;
                case "FNAM": FNAM = new STRVField(); return FNAM;
                case "WEAT": WEAT = new WEATField(); return WEAT;
                case "BNAM": BNAM = new STRVField(); return BNAM;
                case "CNAM": CNAM = new CNAMField(); return CNAM;
                case "SNAM": var SNAM = new SNAMField(); SNAMs.Add(SNAM); return SNAM;
                default: return null;
            }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}