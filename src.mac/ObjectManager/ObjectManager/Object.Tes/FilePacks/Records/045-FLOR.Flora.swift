//
//  FLORRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class FLORRecord: Record, IHaveEDID, IHaveMODL {
    public override var description: String { return "FLOR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Plant Name
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script (optional)
    public var PFIG: FMIDField<INGRRecord>? = nil // The ingredient the plant produces (optional)
    public var PFPC: BYTVField! // Spring, Summer, Fall, Winter Ingredient Production (byte)
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "PFIG": PFIG = FMIDField<INGRRecord>(r, dataSize)
        case "PFPC": PFPC = r.readBYTV(dataSize)
        default: return false
        }
        return true
    }
}
