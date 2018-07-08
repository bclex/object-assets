//
//  APPARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class APPARecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField {
        public let type: UInt8 // 0 = Mortar and Pestle, 1 = Albemic, 2 = Calcinator, 3 = Retort
        public let quality: Float
        public let weight: Float
        public let value: Int32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                type = UInt8(r.readLEInt32())
                quality = r.readLESingle()
                weight = r.readLESingle()
                value = r.readLEInt32()
                return
            }
            type = r.readByte()
            value = r.readLEInt32()
            weight = r.readLESingle()
            quality = r.readLESingle()
        }
    }

    public override var description: String { return "APPA: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model Name
    public var FULL: STRVField! // Item Name
    public var DATA: DATAField! // Alchemy Data
    public var ICON: FILEField! // Inventory Icon
    public var SCRI: FMIDField<SCPTRecord>! // Script Name

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = r.readSTRV(dataSize)
        case "DATA",
             "AADT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
