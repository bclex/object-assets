//
//  DLBRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class DLBRRecord: Record {
    public override var description: String { return "DLBR: \(EDID)" }
    public var EDID: STRVField = STRVField.empty // Editor ID
    public var CNAME: CREFField! // RGB color
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "CNAME": CNAME = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
