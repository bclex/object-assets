//
//  SNDGRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SNDGRecord: Record {
    public enum SNDGType: UInt32 {
        case leftFoot = 0, rightFoot, swimLeft, swimRight, moan, roar, scream, land = 7
    }

    public var description: String { return "SNDG: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var DATA: IN32Field // Sound Type Data
    public var SNAM: STRVField // Sound ID
    public var CNAM: STRVField? // Creature name (optional)

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "DATA": DATA = IN32Field(r, dataSize)
        case "SNAM": SNAM = STRVField(r, dataSize)
        case "CNAM": CNAM = STRVField(r, dataSize)
        default: return false
        }
        return true
    }
}
