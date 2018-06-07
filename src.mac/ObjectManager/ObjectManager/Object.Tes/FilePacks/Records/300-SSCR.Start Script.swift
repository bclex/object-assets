//
//  SSCRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SSCRRecord: Record {
    public var description: String { return "SSCR: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var DATA: STRVField // Digits

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "DATA": DATA = STRVField(r, dataSize)
        default: return false
        }
        return true
    }
}
