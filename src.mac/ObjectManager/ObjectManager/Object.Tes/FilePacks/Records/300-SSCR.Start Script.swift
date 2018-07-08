//
//  SSCRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SSCRRecord: Record {
    public override var description: String { return "SSCR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var DATA: STRVField! // Digits
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = r.readSTRV(dataSize)
        case "DATA": DATA = r.readSTRV(dataSize)
        default: return false
        }
        return true
    }
}
