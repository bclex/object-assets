//
//  LEVIRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LEVIRecord: Record {
    public var description: String { return "LEVI: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var DATA: IN32Field // List data - 1 = Calc from all levels <= PC level, 2 = Calc for each item
    public var NNAM: BYTEField // Chance None?
    public var INDX: IN32Field // Number of items in list
    public var INAMs = [STRVField]() // ID string of list item
    public var INTVs = [IN16Field]() // PC level for previous INAM
    // The CNAM/INTV can occur many times in pairs

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "DATA": DATA = IN32Field(r, dataSize)
        case "NNAM": NNAM = BYTEField(r, dataSize)
        case "INDX": INDX = IN32Field(r, dataSize)
        case "INAM": INAMs.append(STRVField(r, dataSize))
        case "INTV": INTVs.append(IN16Field(r, dataSize))
        default: return false
        }
        return true
    }
}
