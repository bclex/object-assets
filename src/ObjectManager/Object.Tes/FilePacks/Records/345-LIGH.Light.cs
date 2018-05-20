﻿using OA.Core;

namespace OA.Tes.FilePacks.Records
{
    public class LIGHRecord : Record, IHaveEDID, IHaveMODL
    {
        // TESX
        public struct DATAField
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

            public float Weight;
            public int Value;
            public int Time;
            public int Radius;
            public ColorRef LightColor;
            public int Flags;
            // TES4
            public float FalloffExponent;
            public float FOV;

            public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
            {
                if (formatId == GameFormatId.Tes3)
                {
                    Weight = r.ReadLESingle();
                    Value = r.ReadLEInt32();
                    Time = r.ReadLEInt32();
                    Radius = r.ReadLEInt32();
                    LightColor = new ColorRef(r);
                    Flags = r.ReadLEInt32();
                    FalloffExponent = 1;
                    FOV = 90;
                    return;
                }

                Time = r.ReadLEInt32();
                Radius = r.ReadLEInt32();
                LightColor = new ColorRef(r);
                Flags = r.ReadLEInt32();
                if (dataSize == 32)
                {
                    FalloffExponent = r.ReadLESingle();
                    FOV = r.ReadLESingle();
                }
                else
                {
                    FalloffExponent = 1;
                    FOV = 90;
                }
                Value = r.ReadLEInt32();
                Weight = r.ReadLESingle();
            }
        }

        public override string ToString() => $"LIGH: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public MODLGroup MODL { get; set; } // Model
        public STRVField? FULL; // Item Name (optional)
        public DATAField DATA; // Light Data
        public STRVField? SCPT; // Script Name (optional)??
        public FMIDField<SCPTRecord>? SCRI; // Script FormId (optional)
        public FILEField? ICON; // Male Icon (optional)
        public FLTVField FNAM; // Fade Value
        public FMIDField<SOUNRecord> SNAM; // Sound FormId (optional)

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "FNAM": if (formatId != GameFormatId.Tes3) FNAM = new FLTVField(r, dataSize); else FULL = new STRVField(r, dataSize); return true;
                case "DATA":
                case "LHDT": DATA = new DATAField(r, dataSize, formatId); return true;
                case "SCPT": SCPT = new STRVField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                case "ICON":
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "MODT": MODL.MODTField(r, dataSize); return true;
                case "SNAM": SNAM = new FMIDField<SOUNRecord>(r, dataSize); return true;
                default: return false;
            }
        }
    }
}