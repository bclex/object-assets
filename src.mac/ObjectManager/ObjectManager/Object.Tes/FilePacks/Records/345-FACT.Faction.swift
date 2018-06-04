//
//  FACTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class FACTRecord: Record {
    // TESX
    public class RNAMGroup: CustomStringConvertible {
        public var description: String { return "FACT: \(RNAM.value):\(MNAM.value)" }
        public var RNAM: IN32Field // rank
        public var MNAM: STRVField // male
        public var FNAM: STRVField // female
        public var INAM: STRVField // insignia
    }

    // TES3
    public struct FADTField {
        init(_ r: BinaryReader, _ dataSize: Int) {
            r.skipBytes(dataSize)
        }
    }

    // TES4
    public struct XNAMField: CustomStringConvertible {
        public var description: String { return "FACT: \(formId)" }
        public let formId: Int32
        public let mod: Int32
        public let combat: Int32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            formId = r.readLEInt32()
            mod = r.readLEInt32()
            combat = format > .TES4 ? r.readLEInt32() : 0 // 0 - Neutral, 1 - Enemy, 2 - Ally, 3 - Friend
        }
    }

    public var description: String { return "FACT: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var FNAM: STRVField // Faction name
    public var RNAMs = [RNAMGroup]() // Rank Name
    public var FADTField FADT // Faction data
    public var ANAMs = [STRVField]() // Faction name
    public var INTVs = [INTVField]() // Faction reaction
    // TES4
    public var XNAM: XNAMField // Interfaction Relations
    public var DATA: INTVField // Flags (byte, uint32)
    public var CNAM: UI32Field

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = STRVField(r, dataSize)
            case "FNAM": FNAM = STRVField(r, dataSize)
            case "RNAM": RNAMs.append(RNAMGroup(MNAM: STRVField(r, dataSize)))
            case "FADT": FADT = FADTField(r, dataSize)
            case "ANAM": ANAMs.append(STRVField(r, dataSize))
            case "INTV": INTVs.append(INTVField(r, dataSize))
            default: return false
            }
            return true
        }
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "FULL": FNAM = STRVField(r, dataSize)
        case "XNAM": XNAM = XNAMField(r, dataSize, format)
        case "DATA": DATA = INTVField(r, dataSize)
        case "CNAM": CNAM = UI32Field(r, dataSize)
        case "RNAM": RNAMs.append(RNAMGroup(RNAM: IN32Field(r, dataSize)))
        case "MNAM": RNAMs.last!.MNAM = STRVField(r, dataSize)
        case "FNAM": RNAMs.last!.FNAM = STRVField(r, dataSize)
        case "INAM": RNAMs.last!.INAM = STRVField(r, dataSize)
        default: return false
        }
        return true
    }
}
