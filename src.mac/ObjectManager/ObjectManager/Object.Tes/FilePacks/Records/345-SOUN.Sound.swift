//
//  SOUNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SOUNRecord: Record, IHaveEDID {
    public struct SOUNFlags: OptionSet {
        let rawValue: UInt16
        public static let randomFrequencyShift = SOUNFlags(rawValue: 0x0001)
        public static let playAtRandom = SOUNFlags(rawValue: 0x0002)
        public static let environmentIgnored = SOUNFlags(rawValue: 0x0004)
        public static let randomLocation = SOUNFlags(rawValue: 0x0008)
        public static let loop = SOUNFlags(rawValue: 0x0010)
        public static let menuSound = SOUNFlags(rawValue: 0x0020)
        public static let _2D = SOUNFlags(rawValue: 0x0040)
        public static let _360LFE = SOUNFlags(rawValue: 0x0080)
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
            _ = r.readByte() // Unused
            flags = r.readLEUInt16()
            r.skipBytes(2) // Unused
            guard dataSize != 8 else {
                return
            }
            staticAttenuation = r.readLEUInt16()
            stopTime = r.readByte()
            startTime = r.readByte()
        }
    }

    public override var description: String { return "SOUN: \(EDID)" }
    public var EDID: STRVField?  // Editor ID
    public var FNAM: FILEField // Sound Filename (relative to Sounds\)
    public var DATA: DATAField // Sound Data

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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
