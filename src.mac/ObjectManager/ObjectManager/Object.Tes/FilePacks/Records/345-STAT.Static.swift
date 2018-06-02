//
//  STATRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class STATRecord: Record, IHaveEDID, IHaveMODL {
    //public override string ToString() => $"STAT: {EDID.Value}";
    public var EDID: STRVField // Editor ID
    public var MODL: MODLGroup // Model

    override func createField(r: BinaryReader, forFormat: GameFormatId, type: String, dataSize: UInt) -> Bool {
        switch type {
            case "EDID",
                 "NAME": EDID = STRVField(r, dataSize)
            case "MODL": MODL = MODLGroup(r, dataSize)
            case "MODB": MODL.MODBField(r, dataSize)
            case "MODT": MODL.MODTField(r, dataSize)
            default: return false
        }
        return true
    }
}