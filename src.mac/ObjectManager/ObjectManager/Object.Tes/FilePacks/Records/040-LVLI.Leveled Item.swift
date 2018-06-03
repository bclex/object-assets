//
//  LVLIRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LVLIRecord: Record {
    public struct LVLOField {
        public let level: Int16
        public let itemFormId: FormId<Record>
        public let count: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            level = r.readLEInt16()
            r.skipBytes(2) // Unused
            itemFormId = FormId<Record>(r.readLEUInt32())
            if dataSize == 12 {
                count = r.readLEInt16()
                r.skipBytes(2) // Unused
            }
            else count = 0;
        }
    }

    public var description: String { return "LVLI: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var LVLD: BYTEField // Chance
    public var LVLF: BYTEField // Flags - 0x01 = Calculate from all levels <= player's level, 0x02 = Calculate for each item in count
    public var DATA: BYTEField? // Data (optional)
    public var LVLOs = [LVLOField]()

    init() {
    }
    
    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "LVLD": LVLD = BYTEField(r, dataSize)
        case "LVLF": LVLF = BYTEField(r, dataSize)
        case "DATA": DATA = BYTEField(r, dataSize)
        case "LVLO": LVLOs.append(LVLOField(r, dataSize))
        default: return false
        }
        return true
    }
}
