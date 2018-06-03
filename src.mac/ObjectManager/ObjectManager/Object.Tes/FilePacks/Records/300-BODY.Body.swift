//
//  BODYRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BODYRecord: Record {
    public struct BYDTField
    {
        public byte Part;
        public byte Vampire;
        public byte Flags;
        public byte PartType;

        public BYDTField(UnityBinaryReader r, uint dataSize)
        {
            Part = r.ReadByte();
            Vampire = r.ReadByte();
            Flags = r.ReadByte();
            PartType = r.ReadByte();
        }
    }

    public var description: String { return "BODY: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // NIF Model
    public STRVField FNAM; // Body name
    public BYDTField BYDT;

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = STRVField(r, dataSize)
        case "BYDT": BYDT = BYDTField(r, dataSize)
        default: return false
        }
        return true
    }
}
