//
//  GLOBRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class GLOBRecord: Record, IHaveEDID {
    public override var description: String { return "GLOB: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var FNAM: BYTEField? = nil // Type of global (s, l, f)
    public var FLTV: FLTVField? = nil // Float data

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "FNAM": FNAM = r.readT(dataSize)
        case "FLTV": FLTV = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
