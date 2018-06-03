//
//  ACTIRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ACTIRecord: Record, IHaveEDID, IHaveMODL {
    public var description: String { return "ACTI: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Model Name
    public FLTVField MODB { get; set; } // Model Bounds
    public BYTVField MODT; // Texture Files Hashes
    public STRVField FULL; // Item Name
    public FMIDField<SCPTRecord> SCRI; // Script (Optional)
    // TES4
    public FMIDField<SOUNRecord> SNAM; // Sound (Optional)

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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
