//
//  SLGMRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SLGMRecord: Record {
    public struct DATAField {
        public let value: Int32
        public let weight: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            value = r.readLEInt32()
            weight = r.readLESingle()
        }
    }

    public var description: String { return "SLGM: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var MODL: MODLGroup // Model
    public var FULL: STRVField // Item Name
    public var SCRI: FMIDField<SCPTRecord> // Script (optional)
    public var DATA: DATAField // Type of soul contained in the gem
    public var ICON: FILEField // Icon (optional)
    public var SOUL: BYTEField // Type of soul contained in the gem
    public var SLCP: BYTEField // Soul gem maximum capacity

    init() {
    }
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "SOUL": SOUL = BYTEField(r, dataSize)
        case "SLCP": SLCP = BYTEField(r, dataSize)
        default: return false
        }
        return true
    }
}
