//
//  BOOKRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BOOKRecord: Record, IHaveEDID, IHaveMODL {
    public struct DATAField {
        public let flags: UInt8 //: Scroll - (1 is scroll, 0 not)
        public let teaches: UInt8 //: SkillId - (-1 is no skill)
        public let value: Int32
        public let weight: Float
        //
        public let enchantPts: Int32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                weight = r.readLESingle()
                value = r.readLEInt32()
                flags = UInt8(r.readLEInt32())
                teaches = UInt8(checkMax: r.readLEInt32())
                enchantPts = r.readLEInt32()
                return
            }
            flags = r.readByte()
            teaches = r.readByte()
            value = r.readLEInt32()
            weight = r.readLESingle()
            enchantPts = 0
        }
    }

    public override var description: String { return "BOOK: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model (optional)
    public var FULL: STRVField! // Item Name
    public var DATA: DATAField! // Book Data
    public var DESC: STRVField! // Book Text
    public var ICON: FILEField? = nil // Inventory Icon (optional)
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script Name (optional)
    public var ENAM: FMIDField<ENCHRecord>? = nil // Enchantment FormId (optional)
    // TES4
    public var ANAM: IN16Field? = nil // Enchantment points (optional)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL?.MODBField(r, dataSize)
        case "MODT": MODL?.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = r.readSTRV(dataSize)
        case "DATA",
             "BKDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "DESC",
             "TEXT": DESC = r.readSTRV(dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "ANAM": ANAM = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
