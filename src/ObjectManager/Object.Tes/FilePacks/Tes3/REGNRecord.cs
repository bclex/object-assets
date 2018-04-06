using OA.Core;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Tes3
{
    public class REGNRecord : Record
    {
        public class WEATSubRecord : SubRecord
        {
            public byte clear;
            public byte cloudy;
            public byte foggy;
            public byte overcast;
            public byte rain;
            public byte thunder;
            public byte ash;
            public byte blight;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                clear = r.ReadByte();
                cloudy = r.ReadByte();
                foggy = r.ReadByte();
                overcast = r.ReadByte();
                rain = r.ReadByte();
                thunder = r.ReadByte();
                ash = r.ReadByte();
                blight = r.ReadByte();
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
            byte red;
            byte green;
            byte blue;
            byte nullByte;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                red = r.ReadByte();
                green = r.ReadByte();
                blue = r.ReadByte();
                nullByte = r.ReadByte();
            }
        }
        public class SNAMSubRecord : SubRecord
        {
            byte[] soundName;
            byte chance;

            public override void DeserializeData(UnityBinaryReader r, uint dataSize)
            {
                soundName = r.ReadBytes(32);
                chance = r.ReadByte();
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
    }
}