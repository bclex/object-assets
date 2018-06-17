//
//  STATRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class STATRecord: Record, IHaveEDID, IHaveMODL {
    public override var description: String { return "STAT: \(EDID!)" }
    public var EDID: STRVField! // Editor ID
    public var MODL: MODLGroup! // Model

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        default: return false
        }
        return true
    }
}
