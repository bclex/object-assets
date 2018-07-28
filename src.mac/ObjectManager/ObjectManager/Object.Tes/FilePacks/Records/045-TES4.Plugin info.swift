//
//  TES4Record.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TES4Record: Record {
    public typealias HEDRField = (
        version: Float,
        numRecords: Int32, // Number of records and groups (not including TES4 record itself).
        nextObjectId: UInt32 // Next available object ID.
    )

    public var HEDR: HEDRField!
    public var CNAM: STRVField? = nil// author (Optional)
    public var SNAM: STRVField? = nil// description (Optional)
    public var MASTs: [STRVField]? = nil// master
    public var DATAs: [INTVField]? = nil// fileSize
    public var ONAM: UNKNField? = nil // overrides (Optional)
    public var INTV: IN32Field! // unknown
    public var INCC: IN32Field? = nil// unknown (Optional)
    // TES5
    public var TNAM: UNKNField? = nil // overrides (Optional)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "HEDR": HEDR = r.readT(dataSize)
        case "OFST": r.skipBytes(dataSize)
        case "DELE": r.skipBytes(dataSize)
        case "CNAM": CNAM = r.readSTRV(dataSize)
        case "SNAM": SNAM = r.readSTRV(dataSize)
        case "MAST": if MASTs == nil { MASTs = [STRVField]() }; MASTs!.append(r.readSTRV(dataSize))
        case "DATA": if DATAs == nil { DATAs = [INTVField]() }; DATAs!.append(r.readINTV(dataSize))
        case "ONAM": ONAM = r.readBYTV(dataSize)
        case "INTV": INTV = r.readT(dataSize)
        case "INCC": INCC = r.readT(dataSize)
        // TES5
        case "TNAM": TNAM = r.readBYTV(dataSize)
        default: return false
        }
        return true
    }
}
