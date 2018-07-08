//
//  HAIRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class HAIRRecord: Record, IHaveEDID, IHaveMODL {
    public override var description: String { return "HAIR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var FULL: STRVField!
    public var MODL: MODLGroup? = nil
    public var ICON: FILEField!
    public var DATA: BYTEField! // Playable, Not Male, Not Female, Fixed
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "DATA": DATA = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
