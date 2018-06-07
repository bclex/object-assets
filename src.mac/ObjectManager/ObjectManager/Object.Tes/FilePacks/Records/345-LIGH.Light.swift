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
        public enum ColorFlags {
            case dynamic = 0x0001
            case canCarry = 0x0002
            case negative = 0x0004
            case flicker = 0x0008
            case fire = 0x0010
            case offDefault = 0x0020
            case flickerSlow = 0x0040
            case pulse = 0x0080
            case pulseSlow = 0x0100
        }

        public let weight: Float
        public let value: Int32
        public let time: Int32
        public let radius: Int32
        public let lightColor: ColorRef
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
                lightColor = ColorRef(r)
                flags = r.readLEInt32()
                falloffExponent = 1
                fov = 90
                return
            }
            time = r.readLEInt32()
            radius = r.readLEInt32()
            lightColor = ColorRef(r)
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

    public var description: String { return "LIGH: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var MODL: MODLGroup  // Model
    public var FULL: STRVField? // Item Name (optional)
    public var DATA: DATAField  // Light Data
    public var SCPT: STRVField? // Script Name (optional)??
    public var SCRI: FMIDField<SCPTRecord>? // Script FormId (optional)
    public var ICON: FILEField? // Male Icon (optional)
    public var FNAM: FLTVField  // Fade Value
    public var SNAM: FMIDField<SOUNRecord> // Sound FormId (optional)

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "FNAM": if format != .TES3 { FNAM = FLTVField(r, dataSize) } else { FULL = STRVField(r, dataSize) }
        case "DATA":
        case "LHDT": DATA = DATAField(r, dataSize, format)
        case "SCPT": SCPT = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
