//
//  LVSPRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LVSPRecord: Record {
    public override var description: String { return "LVSP: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var LVLD: BYTEField! // Chance
    public var LVLF: BYTEField! // Flags
    public var LVLOs = [LVLIRecord.LVLOField]() // Number of items in list
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "LVLD": LVLD = r.readT(dataSize)
        case "LVLF": LVLF = r.readT(dataSize)
        case "LVLO": LVLOs.append(LVLIRecord.LVLOField(r, dataSize))
        default: return false
        }
        return true
    }
}
