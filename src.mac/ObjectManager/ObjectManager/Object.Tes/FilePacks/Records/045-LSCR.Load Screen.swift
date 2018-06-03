//
//  LSCRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LSCRRecord: Record {
    public struct LNAMField {
        public let Direct: FormId<Record>
        public let IndirectWorld: FormId<WRLDRecord>
        public let IndirectGridX: Int16
        public let IndirectGridY: Int16

        init(_ r: BinaryReader, _ dataSize: Int) {
            direct = FormId<Record>(r.readLEUInt32())
            indirectWorld = FormId<WRLDRecord>(r.readLEUInt32())
            indirectGridX = r.readLEInt16()
            indirectGridY = r.readLEInt16()
        }
    }

    public var description: String { return "LSCR: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var ICON: FILEField // Icon
    public var DESC: STRVField // Description
    public var LNAMs: [LNAMField] // LoadForm

    init() {
    }
    
    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "DESC": DESC = STRVField(r, dataSize)
        case "LNAM": if LNAMs == nil { LNAMs = [LNAMField]() }; LNAMs.append(LNAMField(r, dataSize))
        default: return false
        }
        return true
    }
}
