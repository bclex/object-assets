//
//  ANIORecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ANIORecord: Record, IHaveEDID, IHaveMODL{
    public override var description: String { return "ANIO: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var DATA: FMIDField<IDLERecord>! // IDLE animation
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "DATA": DATA = FMIDField<IDLERecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
