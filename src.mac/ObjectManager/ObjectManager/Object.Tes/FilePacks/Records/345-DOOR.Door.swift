//
//  DOORRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class DOORRecord: Record, IHaveEDID, IHaveMODL {
    public var description: String { return "DOOR: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FULL; // Door name
    public MODLGroup MODL { get; set; } // NIF model filename
    public FMIDField<SCPTRecord>? SCRI; // Script (optional)
    public FMIDField<SOUNRecord> SNAM; // Open Sound
    public FMIDField<SOUNRecord> ANAM; // Close Sound
    // TES4
    public FMIDField<SOUNRecord> BNAM; // Loop Sound
    public BYTEField FNAM; // Flags
    public FMIDField<Record> TNAM; // Random teleport destination

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
                "NAME": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "FNAM": if format != .TES3 { FNAM = new BYTEField(r, dataSize) } else { FULL = STRVField(r, dataSize) }
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
        case "ANAM": ANAM = FMIDField<SOUNRecord>(r, dataSize)
        case "BNAM": ANAM = FMIDField<SOUNRecord>(r, dataSize)
        case "TNAM": TNAM = FMIDField<Record>(r, dataSize)
        default: return false
        }
        return true
    }
}