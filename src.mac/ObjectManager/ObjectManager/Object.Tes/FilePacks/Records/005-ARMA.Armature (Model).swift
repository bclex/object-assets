//
//  ARMARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ARMARecord: Record {
    public var description: String { return "ARMA: \(EDID)" }
    public var EDID: STRVField // Editor ID

    init() {
    }
    
    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        default: return false
        }
        return true
    }
}