//
//  FLORRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class FLORRecord: Record {
    public var description: String { return "FLOR: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var MODL: MODLGroup // Model
    public var FULL: STRVField // Plant Name
    public var SCRI: FMIDField<SCPTRecord> // Script (optional)
    public var PFIG: FMIDField<INGRRecord> // The ingredient the plant produces (optional)
    public var PFPC: BYTVField // Spring, Summer, Fall, Winter Ingredient Production (byte)

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "PFIG": PFIG = FMIDField<INGRRecord>(r, dataSize)
        case "PFPC": PFPC = BYTVField(r, dataSize)
        default: return false
        }
        return true
    }
}
