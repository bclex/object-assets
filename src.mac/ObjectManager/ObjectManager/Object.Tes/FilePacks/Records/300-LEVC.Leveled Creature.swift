//
//  LEVCRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LEVCRecord: Record {
    public var description: String { return "LEVC: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public IN32Field DATA; // List data - 1 = Calc from all levels <= PC level
    public BYTEField NNAM; // Chance None?
    public IN32Field INDX; // Number of items in list
    public List<STRVField> CNAMs = new List<STRVField>(); // ID string of list item
    public List<IN16Field> INTVs = new List<IN16Field>(); // PC level for previous CNAM
    // The CNAM/INTV can occur many times in pairs

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "DATA": DATA = IN32Field(r, dataSize)
        case "NNAM": NNAM = BYTEField(r, dataSize)
        case "INDX": INDX = IN32Field(r, dataSize)
        case "CNAM": CNAMs.append(STRVField(r, dataSize))
        case "INTV": INTVs.append(IN16Field(r, dataSize))
        default: return false
        }
        return true
    }
}