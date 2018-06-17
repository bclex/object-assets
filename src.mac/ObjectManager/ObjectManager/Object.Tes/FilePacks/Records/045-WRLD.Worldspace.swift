//
//  WRLDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import CoreGraphics

public class WRLDRecord: Record {
    public struct MNAMField {
        public let usableDimensions: Vector2Int
        // Cell Coordinates
        public let nwCell_x: Int16
        public let nwCell_y: Int16
        public let seCell_x: Int16
        public let seCell_y: Int16

        init(_ r: BinaryReader, _ dataSize: Int) {
            usableDimensions = Vector2Int(Int(r.readLEInt32()), Int(r.readLEInt32()))
            nwCell_x = r.readLEInt16()
            nwCell_y = r.readLEInt16()
            seCell_x = r.readLEInt16()
            seCell_y = r.readLEInt16()
        }
    }

    public class NAM0Field {
        public let min: Vector2 
        public var max: Vector2!

        init(_ r: BinaryReader, _ dataSize: Int) {
            min = Vector2(dx: CGFloat(r.readLESingle()), dy: CGFloat(r.readLESingle()))
        }

        func NAM9Field(_ r: BinaryReader, _ dataSize: Int) {
            max = Vector2(dx: CGFloat(r.readLESingle()), dy: CGFloat(r.readLESingle()))
        }
    }

    public override var description: String { return "WRLD: \(EDID)" }
    public var EDID: STRVField = STRVField.empty // Editor ID
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
