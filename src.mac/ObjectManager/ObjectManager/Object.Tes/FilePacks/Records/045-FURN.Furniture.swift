//
//  FURNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class FURNRecord: Record, IHaveEDID, IHaveMODL {
    public override var description: String { return "FURN: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Furniture Name
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script (optional)
    public var MNAM: IN32Field! // Active marker flags, required. A bit field with a bit value of 1 indicating that the matching marker position in the NIF file is active.
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "MNAM": MNAM = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
