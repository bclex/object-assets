//
//  PACKRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class PACKRecord: Record {
    public struct PKDTField {
        public let flags: UInt16
        public let type: UInt8;

        init(_ r: BinaryReader, _ dataSize: Int) {
            flags = r.readLEUInt16()
            type = r.readByte()
            r.skipBytes(dataSize - 3) // Unused
        }
    }

    public struct PLDTField {
        public let type: Int32
        public let target: UInt32
        public let radius: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            type = r.readLEInt32()
            target = r.readLEUInt32()
            radius = r.readLEInt32()
        }
    }

    public struct PSDTField {
        public let month: UInt8
        public let dayOfWeek: UInt8
        public let date: UInt8
        public let time: Int8
        public let duration: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            month = r.readByte()
            dayOfWeek = r.readByte()
            date = r.readByte()
            time = Int8(r.readByte())
            duration = r.readLEInt32()
        }
    }

    public struct PTDTField {
        public let type: Int32
        public let target: UInt32
        public let count: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            type = r.readLEInt32()
            target = r.readLEUInt32()
            count = r.readLEInt32()
        }
    }

    public override var description: String { return "PACK: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var PKDT: PKDTField! // General
    public var PLDT: PLDTField! // Location
    public var PSDT: PSDTField! // Schedule
    public var PTDT: PTDTField! // Target
    public var CTDAs = [SCPTRecord.CTDAField]() // Conditions

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "PKDT": PKDT = PKDTField(r, dataSize)
        case "PLDT": PLDT = PLDTField(r, dataSize)
        case "PSDT": PSDT = PSDTField(r, dataSize)
        case "PTDT": PTDT = PTDTField(r, dataSize)
        case "CTDA",
             "CTDT": CTDAs.append(SCPTRecord.CTDAField(r, dataSize, format))
        default: return false
        }
        return true
    }
}
