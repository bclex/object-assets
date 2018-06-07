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

    public var description: String { return "MISC: \(EDID)" }
    public EDID: STRVField  // Editor ID
    public MODL: MODLGroup  // Model
    public FULL: STRVField // Item Name
    public DATA: DATAField // Misc Item Data
    public ICON: FILEField // Icon (optional)
    public SCRI: FMIDField<SCPTRecord> // Script FormID (optional)
    // TES3
    public ENAM: FMIDField<ENCHRecord> // enchantment ID

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "DATA",
             "MCDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
