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
        public var description: String { return "FACT: \(RNAM!):\(MNAM ?? "")" }
        public var RNAM: IN32Field! // rank
        public var MNAM: STRVField! // male
        public var FNAM: STRVField! // female
        public var INAM: STRVField! // insignia
        
        init(MNAM: STRVField) { self.MNAM = MNAM }
        init(RNAM: IN32Field) { self.RNAM = RNAM }
    }

    // TES3
    public struct FADTField {
        init(_ r: BinaryReader, _ dataSize: Int) {
            r.skipBytes(dataSize)
        }
    }

    // TES4
    public typealias XNAMField = (
        formId: Int32,
        mod: Int32,
        // TES5
        combat: Int32 // 0 - Neutral, 1 - Enemy, 2 - Ally, 3 - Friend
    )

    public override var description: String { return "FACT: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var FNAM: STRVField! // Faction name
    public var RNAMs = [RNAMGroup]() // Rank Name
    public var FADT: FADTField! // Faction data
    public var ANAMs = [STRVField]() // Faction name
    public var INTVs = [INTVField]() // Faction reaction
    // TES4
    public var XNAM: XNAMField! // Interfaction Relations
    public var DATA: INTVField! // Flags (byte, uint32)
    public var CNAM: UI32Field!

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = r.readSTRV(dataSize)
            case "FNAM": FNAM = r.readSTRV(dataSize)
            case "RNAM": RNAMs.append(RNAMGroup(MNAM: r.readSTRV(dataSize)))
            case "FADT": FADT = FADTField(r, dataSize)
            case "ANAM": ANAMs.append(r.readSTRV(dataSize))
            case "INTV": INTVs.append(r.readINTV(dataSize))
            default: return false
            }
            return true
        }
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "FULL": FNAM = r.readSTRV(dataSize)
        case "XNAM": XNAM = r.readT(dataSize)
        case "DATA": DATA = r.readINTV(dataSize)
        case "CNAM": CNAM = r.readT(dataSize)
        case "RNAM": RNAMs.append(RNAMGroup(RNAM: r.readT(dataSize)))
        case "MNAM": RNAMs.last!.MNAM = r.readSTRV(dataSize)
        case "FNAM": RNAMs.last!.FNAM = r.readSTRV(dataSize)
        case "INAM": RNAMs.last!.INAM = r.readSTRV(dataSize)
        default: return false
        }
        return true
    }
}
