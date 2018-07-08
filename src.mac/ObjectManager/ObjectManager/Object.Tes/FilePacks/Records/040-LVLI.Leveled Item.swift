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
        public let count: Int16

        init(_ r: BinaryReader, _ dataSize: Int) {
            level = r.readLEInt16()
            r.skipBytes(2) // Unused
            itemFormId = FormId<Record>(r.readLEUInt32())
            if dataSize == 12 {
                count = r.readLEInt16()
                r.skipBytes(2) // Unused
            }
            else { count = 0 }
        }
    }

    public override var description: String { return "LVLI: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var LVLD: BYTEField! // Chance
    public var LVLF: BYTEField! // Flags - 0x01 = Calculate from all levels <= player's level, 0x02 = Calculate for each item in count
    public var DATA: BYTEField? = nil // Data (optional)
    public var LVLOs = [LVLOField]()
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "LVLD": LVLD = r.readT(dataSize)
        case "LVLF": LVLF = r.readT(dataSize)
        case "DATA": DATA = r.readT(dataSize)
        case "LVLO": LVLOs.append(LVLOField(r, dataSize))
        default: return false
        }
        return true
    }
}
