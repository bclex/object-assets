//
//  BODYRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BODYRecord: Record {
    public struct BYDTField {
        public let part: UInt8
        public let vampire: UInt8
        public let flags: UInt8
        public let partType: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            part = r.readByte()
            vampire = r.readByte()
            flags = r.readByte()
            partType = r.readByte()
        }
    }

    public override var description: String { return "BODY: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // NIF Model
    public var FNAM: STRVField! // Body name
    public var BYDT: BYDTField!

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = r.readSTRV(dataSize)
        case "BYDT": BYDT = BYDTField(r, dataSize)
        default: return false
        }
        return true
    }
}
