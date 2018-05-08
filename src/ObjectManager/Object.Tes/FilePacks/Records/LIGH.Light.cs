using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class LIGHRecord : Record
    {
        public class LHDTSubRecord : SubRecord
        {
            public float Weight;
            public int Value;
            public int Time;
            public int Radius;
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte NullByte;
            public int Flags;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Time = r.ReadLEInt32();
                Radius = r.ReadLEInt32();
                Red = r.ReadByte();
                Green = r.ReadByte();
                Blue = r.ReadByte();
                NullByte = r.ReadByte();
                Flags = r.ReadLEInt32();
            }
        }

        public NAMESubRecord NAME;
        public FNAMSubRecord FNAM;
        public LHDTSubRecord LHDT;
        public SCPTSubRecord SCPT;
        public ITEXSubRecord ITEX;
        public MODLSubRecord MODL;
        public SNAMSubRecord SNAM;

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            switch (subRecordName)
            {
                case "NAME": NAME = new NAMESubRecord(); return NAME;
                case "FNAM": FNAM = new FNAMSubRecord(); return FNAM;
                case "LHDT": LHDT = new LHDTSubRecord(); return LHDT;
                case "SCPT": SCPT = new SCPTSubRecord(); return SCPT;
                case "ITEX": ITEX = new ITEXSubRecord(); return ITEX;
                case "MODL": MODL = new MODLSubRecord(); return MODL;
                case "SNAM": SNAM = new SNAMSubRecord(); return SNAM;
                default: return null;
            }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}