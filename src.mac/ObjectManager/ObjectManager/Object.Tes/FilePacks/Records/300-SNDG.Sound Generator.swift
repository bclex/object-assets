//
//  SNDGRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SNDGRecord: Record {
    public enum SNDGType : uint
    {
        LeftFoot = 0,
        RightFoot = 1,
        SwimLeft = 2,
        SwimRight = 3,
        Moan = 4,
        Roar = 5,
        Scream = 6,
        Land = 7,
    }

    public var description: String { return "SNDG: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public IN32Field DATA; // Sound Type Data
    public STRVField SNAM; // Sound ID
    public STRVField? CNAM; // Creature name (optional)

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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