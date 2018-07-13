//
//  LIGHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LIGHRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField {
        public struct ColorFlags: OptionSet {
            public let rawValue: UInt32
            public static let dynamic = ColorFlags(rawValue: 0x0001)
            public static let canCarry = ColorFlags(rawValue: 0x0002)
            public static let negative = ColorFlags(rawValue: 0x0004)
            public static let flicker = ColorFlags(rawValue: 0x0008)
            public static let fire = ColorFlags(rawValue: 0x0010)
            public static let offDefault = ColorFlags(rawValue: 0x0020)
            public static let flickerSlow = ColorFlags(rawValue: 0x0040)
            public static let pulse = ColorFlags(rawValue: 0x0080)
            public static let pulseSlow = ColorFlags(rawValue: 0x0100)
            
            public init(rawValue: UInt32) {
                self.rawValue = rawValue
            }
        }

        public let weight: Float
        public let value: Int32
        public let time: Int32
        public let radius: Int32
        public let lightColor: ColorRef4
        public let flags: Int32
        // TES4
        public let falloffExponent: Float
        public let fov: Float

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                weight = r.readLESingle()
                value = r.readLEInt32()
                time = r.readLEInt32()
                radius = r.readLEInt32()
                lightColor = r.readT(4)
                flags = r.readLEInt32()
                falloffExponent = 1
                fov = 90
                return
            }
            time = r.readLEInt32()
            radius = r.readLEInt32()
            lightColor = r.readT(4)
            flags = r.readLEInt32()
            if dataSize == 32 {
                falloffExponent = r.readLESingle()
                fov = r.readLESingle()
            }
            else {
                falloffExponent = 1
                fov = 90
            }
            value = r.readLEInt32()
            weight = r.readLESingle()
        }
    }

    public override var description: String { return "LIGH: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField? = nil // Item Name (optional)
    public var DATA: DATAField!  // Light Data
    public var SCPT: STRVField? = nil // Script Name (optional)??
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script FormId (optional)
    public var ICON: FILEField? = nil // Male Icon (optional)
    public var FNAM: FLTVField! // Fade Value
    public var SNAM: FMIDField<SOUNRecord>? = nil // Sound FormId (optional)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "FNAM": if format != .TES3 { FNAM = r.readT(dataSize) } else { FULL = r.readSTRV(dataSize) }
        case "DATA",
             "LHDT": DATA = DATAField(r, dataSize, format)
        case "SCPT": SCPT = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ICON",
             "ITEX": ICON = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
