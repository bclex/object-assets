//
//  LEVIRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LEVIRecord: Record {
    public override var description: String { return "LEVI: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var DATA: IN32Field! // List data - 1 = Calc from all levels <= PC level, 2 = Calc for each item
    public var NNAM: BYTEField! // Chance None?
    public var INDX: IN32Field! // Number of items in list
    public var INAMs = [STRVField]() // ID string of list item
    public var INTVs = [IN16Field]() // PC level for previous INAM
    // The CNAM/INTV can occur many times in pairs

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = r.readSTRV(dataSize)
        case "DATA": DATA = r.readT(dataSize)
        case "NNAM": NNAM = r.readT(dataSize)
        case "INDX": INDX = r.readT(dataSize)
        case "INAM": INAMs.append(r.readSTRV(dataSize))
        case "INTV": INTVs.append(r.readT(dataSize))
        default: return false
        }
        return true
    }
}
