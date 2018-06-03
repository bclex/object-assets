//
//  TES4Record.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TES4Record: Record {
    public struct HEDRField {
        public let version: Float
        public let numRecords: Int32 // Number of records and groups (not including TES4 record itself).
        public let nextObjectId: UInt32 // Next available object ID.

        init(_ r: BinaryReader, _ dataSize: Int) {
            version = r.readLESingle()
            numRecords = r.readLEInt32()
            nextObjectId = r.readLEUInt32()
        }
    }

    public HEDR: HEDRField
    public CNAM: STRVField? // author (Optional)
    public SNAM: STRVField? // description (Optional)
    public MASTs: [STRVField] // master
    public DATAs: [INTVField] // fileSize
    public ONAM: UNKNField? // overrides (Optional)
    public INTV: IN32Field // unknown
    public INCC: IN32Field? // unknown (Optional)

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "HEDR": HEDR = HEDRField(r, dataSize)
        case "OFST": r.skipBytes(dataSize)
        case "DELE": r.skipBytes(dataSize)
        case "CNAM": CNAM = STRVField(r, dataSize)
        case "SNAM": SNAM = STRVField(r, dataSize)
        case "MAST": if MASTs == nil { MASTs = [STRVField]() }; MASTs.append(STRVField(r, dataSize))
        case "DATA": if DATAs == nil { DATAs = [INTVField]() }; DATAs.append(INTVField(r, dataSize))
        case "ONAM": ONAM = UNKNField(r, dataSize)
        case "INTV": INTV = IN32Field(r, dataSize)
        case "INCC": INCC = IN32Field(r, dataSize)
        default: return false
        }
        return true
    }
}
