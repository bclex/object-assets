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
        public let materialType: UInt8
        public let friction: UInt8
        public let restitution: UInt8
        
        init(_ r: BinaryReader, _ dataSize: Int) {
            materialType = r.readByte()
            friction = r.readByte()
            restitution = r.readByte()
        }
    }

    public var description: String { return "LTEX: \(EDID)" }
    public EDID: STRVField // Editor ID
    public ICON: FILEField // Texture
    // TES3
    public INTV: INTVField
    // TES4
    public HNAM: HNAMField // Havok data
    public SNAM: BYTEField // Texture specular exponent
    public GNAMs = [FMIDField<GRASRecord>]() // Potential grass

    init() {
    }

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