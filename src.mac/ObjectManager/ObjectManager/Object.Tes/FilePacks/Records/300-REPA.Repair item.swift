//
//  REPARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class REPARecord: Record, IHaveEDID, IHaveMODL {
    public struct RIDTField {
        public let weight: Float
        public let value: Int32
        public let uses: Int32
        public let quality: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            weight = r.readLESingle()
            value = r.readLEInt32()
            uses = r.readLEInt32()
            quality = r.readLESingle()
        }
    }

    public override var description: String { return "REPA: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model Name
    public var FNAM: STRVField! // Item Name
    public var RIDT: RIDTField! // Repair Data
    public var ICON: FILEField! // Inventory Icon
    public var SCRI: FMIDField<SCPTRecord>! // Script Name

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = r.readSTRV(dataSize)
        case "RIDT": RIDT = RIDTField(r, dataSize)
        case "ITEX": ICON = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
