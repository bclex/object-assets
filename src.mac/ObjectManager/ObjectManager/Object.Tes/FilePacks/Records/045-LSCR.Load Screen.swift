//
//  LSCRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LSCRRecord: Record {
    public struct LNAMField {
        public let direct: FormId<Record>
        public let indirectWorld: FormId<WRLDRecord>
        public let indirectGridX: Int16
        public let indirectGridY: Int16

        init(_ r: BinaryReader, _ dataSize: Int) {
            direct = FormId<Record>(r.readLEUInt32())
            indirectWorld = FormId<WRLDRecord>(r.readLEUInt32())
            indirectGridX = r.readLEInt16()
            indirectGridY = r.readLEInt16()
        }
    }

    public override var description: String { return "LSCR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var ICON: FILEField! // Icon
    public var DESC: STRVField! // Description
    public var LNAMs: [LNAMField]! // LoadForm
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "DESC": DESC = r.readSTRV(dataSize)
        case "LNAM": if LNAMs == nil { LNAMs = [LNAMField]() }; LNAMs.append(LNAMField(r, dataSize))
        default: return false
        }
        return true
    }
}
