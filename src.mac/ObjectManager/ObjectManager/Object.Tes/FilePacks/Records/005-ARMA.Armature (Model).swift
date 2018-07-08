//
//  ARMARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ARMARecord: Record {
    public override var description: String { return "ARMA: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        default: return false
        }
        return true
    }
}
