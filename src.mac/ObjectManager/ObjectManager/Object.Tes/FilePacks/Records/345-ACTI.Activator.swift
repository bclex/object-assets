//
//  ACTIRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ACTIRecord: Record, IHaveEDID, IHaveMODL {
    public var description: String { return "ACTI: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var MODL: MODLGroup  // Model Name
    public var MODB: FLTVField  // Model Bounds
    public var MODT: BYTVField // Texture Files Hashes
    public var FULL: STRVField // Item Name
    public var SCRI: FMIDField<SCPTRecord> // Script (Optional)
    // TES4
    public var SNAM: FMIDField<SOUNRecord> // Sound (Optional)

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID":
        case "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
