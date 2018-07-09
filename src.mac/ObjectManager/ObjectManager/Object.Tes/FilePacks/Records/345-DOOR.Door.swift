//
//  DOORRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class DOORRecord: Record, IHaveEDID, IHaveMODL {
    public override var description: String { return "DOOR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var FULL: STRVField! // Door name
    public var MODL: MODLGroup? = nil // NIF model filename
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script (optional)
    public var SNAM: FMIDField<SOUNRecord>!  // Open Sound
    public var ANAM: FMIDField<SOUNRecord>!  // Close Sound
    // TES4
    public var BNAM: FMIDField<SOUNRecord>! // Loop Sound
    public var FNAM: BYTEField! // Flags
    public var TNAM: FMIDField<Record>! // Random teleport destination

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "FNAM": if format != .TES3 { FNAM = r.readT(dataSize) } else { FULL = r.readSTRV(dataSize) }
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
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
