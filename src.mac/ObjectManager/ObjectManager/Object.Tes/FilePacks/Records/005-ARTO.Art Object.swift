//
//  ARTORecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ARTORecord: Record {
    public override var description: String { return "ARTO: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var CNAME: CREFField // RGB color

    init() {
    }
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "CNAME": CNAME = CREFField(r, dataSize)
        default: return false
        }
        return true
    }
}
