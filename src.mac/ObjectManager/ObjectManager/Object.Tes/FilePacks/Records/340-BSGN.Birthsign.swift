//
//  BSGNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BSGNRecord: Record {
    public override var description: String { return "BSGN: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var FULL: STRVField! // Sign name
    public var ICON: FILEField! // Texture
    public var DESC: STRVField! // Description
    public var NPCSs: [STRVField]? = nil // TES3: Spell/ability
    public var SPLOs: [FMIDField<Record>]? = nil // TES4: (points to a SPEL or LVSP record)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "FULL",
             "FNAM": FULL = r.readSTRV(dataSize)
        case "ICON",
             "TNAM": ICON = r.readSTRV(dataSize)
        case "DESC": DESC = r.readSTRV(dataSize)
        case "SPLO": if SPLOs == nil { SPLOs = [FMIDField<Record>]() }; SPLOs?.append(FMIDField<Record>(r, dataSize))
        case "NPCS": if NPCSs == nil { NPCSs = [STRVField]() }; NPCSs?.append(r.readSTRV(dataSize))
        default: return false
        }
        return true
    }
}
