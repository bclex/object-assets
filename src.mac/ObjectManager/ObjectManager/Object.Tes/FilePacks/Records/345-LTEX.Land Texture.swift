//
//  LTEXRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LTEXRecord: Record, IHaveEDID {
    public typealias HNAMField = (
        materialType: UInt8,
        friction: UInt8,
        restitution: UInt8
    )
    
    public override var description: String { return "LTEX: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var ICON: FILEField! // Texture
    // TES3
    public var INTV: INTVField!
    // TES4
    public var HNAM: HNAMField! // Havok data
    public var SNAM: BYTEField! // Texture specular exponent
    public var GNAMs = [FMIDField<GRASRecord>]() // Potential grass

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "INTV": INTV = r.readINTV(dataSize)
        case "ICON",
             "DATA": ICON = r.readSTRV(dataSize)
        // TES4
        case "HNAM": HNAM = r.readT(dataSize)
        case "SNAM": SNAM = r.readT(dataSize)
        case "GNAM": GNAMs.append(FMIDField<GRASRecord>(r, dataSize))
        default: return false
        }
        return true
    }
}
