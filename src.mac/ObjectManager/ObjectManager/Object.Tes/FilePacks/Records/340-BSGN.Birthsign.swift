//
//  BSGNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BSGNRecord: Record {
    public var description: String { return "BSGN: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FULL; // Sign name
    public FILEField ICON; // Texture
    public STRVField DESC; // Description
    public List<STRVField> NPCSs = new List<STRVField>(); // TES3: Spell/ability
    public List<FMIDField<Record>> SPLOs = new List<FMIDField<Record>>(); // TES4: (points to a SPEL or LVSP record)

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = new STRVField(r, dataSize)
        case "FULL",
             "FNAM": FULL = new STRVField(r, dataSize)
        case "ICON":
        case "TNAM": ICON = new FILEField(r, dataSize)
        case "DESC": DESC = new STRVField(r, dataSize)
        case "SPLO": if SPLOs == nil { SPLOs = [FMIDField<Record>] } SPLOs.append(FMIDField<Record>(r, dataSize))
        case "NPCS": if NPCSs == nil { NPCSs = [STRVField]() } NPCSs.append(STRVField(r, dataSize))
        default: return false
        }
        return true
    }
}
