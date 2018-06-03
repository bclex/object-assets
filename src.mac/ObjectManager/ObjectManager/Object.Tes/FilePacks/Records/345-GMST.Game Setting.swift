//
//  GMSTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class GMSTRecord: Record, IHaveEDID {
    public var description: String { return "GMST: \(EDID)" }
    public STRVField EDID  // Editor ID
    public DATVField DATA // Data

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = STRVField(r, dataSize)
            case "STRV": DATA = DATVField(r, dataSize, 's')
            case "INTV": DATA = DATVField(r, dataSize, 'i')
            case "FLTV": DATA = DATVField(r, dataSize, 'f')
            default: return false
            }
            return true
        }
        switch (type)
        {
        case "EDID": EDID = STRVField(r, dataSize)
        case "DATA": DATA = DATVField(r, dataSize, EDID[0])
        default: return false
        }
        return true
    }
}