//
//  WRLDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class WRLDRecord: Record {
    public struct MNAMField
    {
        public Vector2Int UsableDimensions;
        // Cell Coordinates
        public short NWCell_X;
        public short NWCell_Y;
        public short SECell_X;
        public short SECell_Y;

        public MNAMField(UnityBinaryReader r, uint dataSize)
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

        public NAM0Field(UnityBinaryReader r, uint dataSize)
        {
            Min = new Vector2(r.ReadLESingle(), r.ReadLESingle());
        }

        public void NAM9Field(UnityBinaryReader r, uint dataSize)
        {
            Max = new Vector2(r.ReadLESingle(), r.ReadLESingle());
        }
    }

    public var description: String { return "WRLD: \(EDID)" }
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

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "WNAM": WNAM = FMIDField<WRLDRecord>(r, dataSize)
        case "CNAM": CNAM = FMIDField<CLMTRecord>(r, dataSize)
        case "NAM2": NAM2 = FMIDField<WATRRecord>(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "MNAM": MNAM = MNAMField(r, dataSize)
        case "DATA": DATA = BYTEField(r, dataSize)
        case "NAM0": NAM0 = NAM0Field(r, dataSize)
        case "NAM9": NAM0.NAM9Field(r, dataSize)
        case "SNAM": SNAM = UI32Field(r, dataSize)
        case "OFST": r.skipBytes(dataSize)
        default: return false
        }
        return true
    }
}
