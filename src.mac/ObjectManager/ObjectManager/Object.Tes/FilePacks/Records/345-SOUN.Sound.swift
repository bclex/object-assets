//
//  SOUNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SOUNRecord: Record, IHaveEDID {
    public enum SOUNFlags: UInt16 {
        case randomFrequencyShift = 0x0001
        case playAtRandom = 0x0002
        case environmentIgnored = 0x0004
        case randomLocation = 0x0008
        case loop = 0x0010
        case menuSound = 0x0020
        case _2D = 0x0040
        case _360LFE = 0x0080
    }

    // TESX
    public class DATAField {
        public let volume: UInt8 // (0=0.00, 255=1.00)
        public let minRange: UInt8 // Minimum attenuation distance
        public let maxRange: UInt8 // Maximum attenuation distance
        // Tes4
        public let frequencyAdjustment: Int8 // Frequency adjustment %
        public let flags: UInt16 // Flags
        public let staticAttenuation: UInt16 // Static Attenuation (db)
        public let stopTime: UInt8 // Stop time
        public let startTime: UInt8 // Start time

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            volume = format == .TES3 ? r.readByte() : 0
            minRange = r.readByte()
            maxRange = r.readByte()
            guard format != .TES3 else {
                return
            }
            frequencyAdjustment = r.readSByte()
            r.readByte() // Unused
            flags = r.readLEUInt16()
            r.readLEUInt16() // Unused
            guard dataSize != 8 else {
                return
            }
            staticAttenuation = r.readLEUInt16()
            stopTime = r.readByte()
            startTime = r.readByte()
        }
    }

    public var description: String { return "SOUN: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var FNAM: FILEField // Sound Filename (relative to Sounds\)
    public var DATA: DATAField // Sound Data

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FNAM": FNAM = FILEField(r, dataSize)
        case "SNDX": DATA = DATAField(r, dataSize, format)
        case "SNDD": DATA = DATAField(r, dataSize, format)
        case "DATA": DATA = DATAField(r, dataSize, format)
        default: return false
        }
        return true
    }
}
