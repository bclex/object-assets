//
//  WRLDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import CoreGraphics
import simd

public class WRLDRecord: Record {
    public typealias MNAMField = (
        pusableDimensions: int2,
        // Cell Coordinates
        nwCell_x: Int16,
        nwCell_y: Int16,
        seCell_x: Int16,
        seCell_y: Int16
    )

    public class NAM0Field {
        public let min: float2
        public var max: float2!

        init(_ r: BinaryReader, _ dataSize: Int) {
            min = r.readT(dataSize)
        }

        func NAM9Field(_ r: BinaryReader, _ dataSize: Int) {
            max = r.readT(dataSize)
        }
    }

    public override var description: String { return "WRLD: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var FULL: STRVField!
    public var WNAM: FMIDField<WRLDRecord>? = nil // Parent Worldspace
    public var CNAM: FMIDField<CLMTRecord>? = nil // Climate
    public var NAM2: FMIDField<WATRRecord>? = nil // Water
    public var ICON: FILEField? = nil // Icon
    public var MNAM: MNAMField? = nil // Map Data
    public var DATA: BYTEField? = nil // Flags
    public var NAM0: NAM0Field! // Object Bounds
    public var SNAM: UI32Field? = nil // Music

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "WNAM": WNAM = FMIDField<WRLDRecord>(r, dataSize)
        case "CNAM": CNAM = FMIDField<CLMTRecord>(r, dataSize)
        case "NAM2": NAM2 = FMIDField<WATRRecord>(r, dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "MNAM": MNAM = r.readT(dataSize)
        case "DATA": DATA = r.readT(dataSize)
        case "NAM0": NAM0 = NAM0Field(r, dataSize)
        case "NAM9": NAM0.NAM9Field(r, dataSize)
        case "SNAM": SNAM = r.readT(dataSize)
        case "OFST": r.skipBytes(dataSize)
        default: return false
        }
        return true
    }
}
