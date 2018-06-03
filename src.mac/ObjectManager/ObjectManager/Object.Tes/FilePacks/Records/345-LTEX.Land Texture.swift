//
//  LTEXRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LTEXRecord: Record, IHaveEDID {
    public struct HNAMField
    {
        public byte MaterialType;
        public byte Friction;
        public byte Restitution;
        
        public HNAMField(UnityBinaryReader r, uint dataSize)
        {
            MaterialType = r.ReadByte();
            Friction = r.ReadByte();
            Restitution = r.ReadByte();
        }
    }

    public var description: String { return "LTEX: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public FILEField ICON; // Texture
    // TES3
    public INTVField INTV;
    // TES4
    public HNAMField HNAM; // Havok data
    public BYTEField SNAM; // Texture specular exponent
    public List<FMIDField<GRASRecord>> GNAMs = new List<FMIDField<GRASRecord>>(); // Potential grass

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "INTV": INTV = INTVField(r, dataSize)
        case "ICON":
        case "DATA": ICON = FILEField(r, dataSize)
        // TES4
        case "HNAM": HNAM = HNAMField(r, dataSize)
        case "SNAM": SNAM = BYTEField(r, dataSize)
        case "GNAM": GNAMs.append(FMIDField<GRASRecord>(r, dataSize))
        default: return false
        }
        return true
    }
}