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
                flags = (byte)r.readLEInt32()
                teaches = (byte)r.readLEInt32()
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

    public var description: String { return "BOOK: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var MODL: MODLGroup  // Model (optional)
    public var FULL: STRVField // Item Name
    public var DATA: DATAField // Book Data
    public var DESC: STRVField // Book Text
    public var ICON: FILEField // Inventory Icon (optional)
    public var SCRI: FMIDField<SCPTRecord> // Script Name (optional)
    public var ENAM: FMIDField<ENCHRecord> // Enchantment FormId (optional)
    // TES4
    public var ANAM: IN16Field? // Enchantment points (optional)

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL":
        case "FNAM": FULL = STRVField(r, dataSize)
        case "DATA",
             "BKDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "DESC",
             "TEXT": DESC = STRVField(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "ANAM": ANAM = IN16Field(r, dataSize)
        default: return false
        }
        return true
    }
}
