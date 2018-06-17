//
//  BSGNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BSGNRecord: Record {
    public override var description: String { return "BSGN: \(EDID!)" }
    public var EDID: STRVField!  // Editor ID
    public var FULL: STRVField! // Sign name
    public var ICON: FILEField! // Texture
    public var DESC: STRVField! // Description
    public var NPCSs = [STRVField]? = nil // TES3: Spell/ability
    public var SPLOs = [FMIDField<Record>]? = nil // TES4: (points to a SPEL or LVSP record)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "ICON",
             "TNAM": ICON = FILEField(r, dataSize)
        case "DESC": DESC = STRVField(r, dataSize)
        case "SPLO": if SPLOs == nil { SPLOs = [FMIDField<Record>] }; SPLOs?.append(FMIDField<Record>(r, dataSize))
        case "NPCS": if NPCSs == nil { NPCSs = [STRVField]() }; NPCSs?.append(STRVField(r, dataSize))
        default: return false
        }
        return true
    }
}
