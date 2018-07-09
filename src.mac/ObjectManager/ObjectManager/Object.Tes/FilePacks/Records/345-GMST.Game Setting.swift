//
//  GMSTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class GMSTRecord: Record, IHaveEDID {
    public override var description: String { return "GMST: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var DATA: DATVField! // Data

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = r.readSTRV(dataSize)
            case "STRV": DATA = r.readDATV(dataSize, type: "s")
            case "INTV": DATA = r.readDATV(dataSize, type: "i")
            case "FLTV": DATA = r.readDATV(dataSize, type: "f")
            default: return false
            }
            return true
        }
        switch (type)
        {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "DATA": DATA = r.readDATV(dataSize, type: EDID.first!)
        default: return false
        }
        return true
    }
}
