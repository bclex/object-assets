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

    public override var description: String { return "SNDG: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var DATA: IN32Field! // Sound Type Data
    public var SNAM: STRVField! // Sound ID
    public var CNAM: STRVField? = nil// Creature name (optional)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = r.readSTRV(dataSize)
        case "DATA": DATA = r.readT(dataSize)
        case "SNAM": SNAM = r.readSTRV(dataSize)
        case "CNAM": CNAM = r.readSTRV(dataSize)
        default: return false
        }
        return true
    }
}
