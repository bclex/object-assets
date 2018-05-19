using OA.Core;
using System;

namespace OA.Tes.FilePacks.Records
{
    public class LIGHRecord : Record, IHaveEDID, IHaveMODL
    {
        public enum ColorFlags
        {
            Dynamic = 0x0001,
            CanCarry = 0x0002,
            Negative = 0x0004,
            Flicker = 0x0008,
            Fire = 0x0010,
            OffDefault = 0x0020,
            FlickerSlow = 0x0040,
            Pulse = 0x0080,
            PulseSlow = 0x0100
        }

        public struct LHDTField
        {
            public float Weight;
            public int Value;
            public int Time;
            public int Radius;
            public byte Red; // ColorRef
            public byte Green; // ColorRef
            public byte Blue; // ColorRef
            public byte NullByte; // ColorRef
            public int Flags;

            public LHDTField(UnityBinaryReader r, uint dataSize)
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

        public override string ToString() => $"LIGH: {EDID.Value}";
        public STRVField EDID { get; set; } // ID
        public STRVField? FNAM; // Item name (optional)
        public LHDTField LHDT; // Light data
        public STRVField? SCPT; // Script name (optional)
        public STRVField? SCRI; // Unknown
        public FILEField? ITEX; // Inventory icon (optional)
        public FILEField MODL { get; set; } // NIF model name
        public STRVField SNAM; // Sound name

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "LHDT": LHDT = new LHDTField(r, dataSize); return true;
                    case "SCPT": SCPT = new STRVField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    case "ITEX": ITEX = new FILEField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "SNAM": SNAM = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}