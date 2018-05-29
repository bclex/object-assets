using OA.Core;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    public class WRLDRecord : Record
    {
        public struct MNAMField
        {
            public Vector2Int UsableDimensions;
            // Cell Coordinates
            public short NWCell_X;
            public short NWCell_Y;
            public short SECell_X;
            public short SECell_Y;

            public MNAMField(UnityBinaryReader r, int dataSize)
            {
                UsableDimensions = new Vector2Int(r.ReadLEInt32(), r.ReadLEInt32());
                NWCell_X = r.ReadLEInt16();
                NWCell_Y = r.ReadLEInt16();
                SECell_X = r.ReadLEInt16();
                SECell_Y = r.ReadLEInt16();
            }
        }

        public class NAM0Field
        {
            public Vector2 Min;
            public Vector2 Max;

            public NAM0Field(UnityBinaryReader r, int dataSize)
            {
                Min = new Vector2(r.ReadLESingle(), r.ReadLESingle());
            }

            public void NAM9Field(UnityBinaryReader r, int dataSize)
            {
                Max = new Vector2(r.ReadLESingle(), r.ReadLESingle());
            }
        }

        public override string ToString() => $"WRLD: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public STRVField FULL;
        public FMIDField<WRLDRecord>? WNAM; // Parent Worldspace
        public FMIDField<CLMTRecord>? CNAM; // Climate
        public FMIDField<WATRRecord>? NAM2; // Water
        public FILEField? ICON; // Icon
        public MNAMField? MNAM; // Map Data
        public BYTEField? DATA; // Flags
        public NAM0Field NAM0; // Object Bounds
        public UI32Field? SNAM; // Music

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "WNAM": WNAM = new FMIDField<WRLDRecord>(r, dataSize); return true;
                case "CNAM": CNAM = new FMIDField<CLMTRecord>(r, dataSize); return true;
                case "NAM2": NAM2 = new FMIDField<WATRRecord>(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "MNAM": MNAM = new MNAMField(r, dataSize); return true;
                case "DATA": DATA = new BYTEField(r, dataSize); return true;
                case "NAM0": NAM0 = new NAM0Field(r, dataSize); return true;
                case "NAM9": NAM0.NAM9Field(r, dataSize); return true;
                case "SNAM": SNAM = new UI32Field(r, dataSize); return true;
                case "OFST": r.ReadBytes((int)dataSize); return true;
                default: return false;
            }
        }
    }
}