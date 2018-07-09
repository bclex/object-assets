//
//  SKILRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SKILRecord: Record {
    // TESX
    public struct DATAField {
        public let action: Int32
        public let attribute: Int32
        public let specialization: UInt32 // 0 = Combat, 1 = Magic, 2 = Stealth
        public var useValue: [Float] // The use types for each skill are hard-coded.

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            action = format == .TES3 ? 0 : r.readLEInt32()
            attribute = r.readLEInt32()
            specialization = r.readLEUInt32()
            let useValueCount = format == .TES3 ? 4 : 2
            useValue = r.readTArray(useValueCount << 2, count: useValueCount)
        }
    }

    public override var description: String { return "SKIL: \(INDX!):\(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var INDX: IN32Field! // Skill ID
    public var DATA: DATAField! // Skill Data
    public var DESC: STRVField! // Skill description
    // TES4
    public var ICON: FILEField! // Icon
    public var ANAM: STRVField! // Apprentice Text
    public var JNAM: STRVField! // Journeyman Text
    public var ENAM: STRVField! // Expert Text
    public var MNAM: STRVField! // Master Text

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "INDX": INDX = r.readT(dataSize)
        case "DATA",
             "SKDT": DATA = DATAField(r, dataSize, format)
        case "DESC": DESC = r.readSTRV(dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "ANAM": ANAM = r.readSTRV(dataSize)
        case "JNAM": JNAM = r.readSTRV(dataSize)
        case "ENAM": ENAM = r.readSTRV(dataSize)
        case "MNAM": MNAM = r.readSTRV(dataSize)
        default: return false
        }
        return true
    }
}
