//
//  GLOBRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class GLOBRecord: Record, IHaveEDID {
    public var description: String { return "GLOB: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public BYTEField? FNAM; // Type of global (s, l, f)
    public FLTVField? FLTV; // Float data

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FNAM": FNAM = BYTEField(r, dataSize)
        case "FLTV": FLTV = FLTVField(r, dataSize)
        default: return false
        }
        return true
    }
}
