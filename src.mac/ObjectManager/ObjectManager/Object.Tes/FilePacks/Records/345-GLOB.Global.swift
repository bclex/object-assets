//
//  GLOBRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class GLOBRecord: Record, IHaveEDID {
    public var description: String { return "GLOB: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var FNAM: BYTEField? // Type of global (s, l, f)
    public var FLTV: FLTVField? // Float data

    init() {
    }

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
