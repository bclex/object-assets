//
//  MISCRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class MISCRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField {
        public let weight: Float
        public let value: UInt32
        public let unknown: UInt32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                weight = r.readLESingle()
                value = r.readLEUInt32()
                unknown = r.readLEUInt32()
                return
            }
            value = r.readLEUInt32()
            weight = r.readLESingle()
            unknown = 0
        }
    }

    public override var description: String { return "MISC: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Item Name
    public var DATA: DATAField! // Misc Item Data
    public var ICON: FILEField? = nil // Icon (optional)
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script FormID (optional)
    // TES3
    public var ENAM: FMIDField<ENCHRecord>! // enchantment ID

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
             "MCDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = r.readSTRV(dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
