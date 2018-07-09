//
//  LVLCRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LVLCRecord: Record {
    public override var description: String { return "LVLC: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var LVLD: BYTEField! // Chance
    public var LVLF: BYTEField! // Flags - 0x01 = Calculate from all levels <= player's level, 0x02 = Calculate for each item in count
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script (optional)
    public var TNAM: FMIDField<CREARecord>? = nil // Creature Template (optional)
    public var LVLOs = [LVLIRecord.LVLOField]()
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "LVLD": LVLD = r.readT(dataSize)
        case "LVLF": LVLF = r.readT(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "TNAM": TNAM = FMIDField<CREARecord>(r, dataSize)
        case "LVLO": LVLOs.append(LVLIRecord.LVLOField(r, dataSize))
        default: return false
        }
        return true
    }
}
