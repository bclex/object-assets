using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class REGNRecord : Record
    {
        public class WEATSubRecord : SubRecord
        {
            public byte Clear;
            public byte Cloudy;
            public byte Foggy;
            public byte Overcast;
            public byte Rain;
            public byte Thunder;
            public byte Ash;
            public byte Blight;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
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
        public class CNAMSubRecord : SubRecord
        {
            byte Red;
            byte Green;
            byte Blue;
            byte NullByte;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                Red = r.ReadByte();
                Green = r.ReadByte();
                Blue = r.ReadByte();
                NullByte = r.ReadByte();
            }
        }
        public class SNAMSubRecord : SubRecord
        {
            byte[] SoundName;
            byte Chance;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                SoundName = r.ReadBytes(32);
                Chance = r.ReadByte();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public WEATSubRecord WEAT;
        public BNAMSubRecord BNAM;
        public CNAMSubRecord CNAM;
        public List<SNAMSubRecord> SNAMs = new List<SNAMSubRecord>();

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "WEAT": WEAT = new WEATSubRecord(); return WEAT;
                case "BNAM": BNAM = new BNAMSubRecord(); return BNAM;
                case "CNAM": CNAM = new CNAMSubRecord(); return CNAM;
                case "SNAM": var SNAM = new SNAMSubRecord(); SNAMs.Add(SNAM); return SNAM;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}