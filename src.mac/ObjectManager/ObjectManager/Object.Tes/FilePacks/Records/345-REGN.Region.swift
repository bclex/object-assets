//
//  REGNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import CoreGraphics
import simd

public class REGNRecord: Record, IHaveEDID {
    // TESX
    public class RDATField {
        public enum REGNType: UInt8 {
            case objects = 2, weather, map, landscape, grass, sound
        }

        public var type: UInt32 = 0
        public var flags: REGNType = .objects
        public var priority: UInt8 = 0
        // groups
        public var RDOTs: [RDOTField]! // Objects
        public var RDMP: STRVField! // MapName
        public var RDGSs: [RDGSField]! // Grasses
        public var RDMD: UI32Field! // Music Type
        public var RDSDs: [RDSDField]! // Sounds
        public var RDWTs: [RDWTField]! // Weather Types

        init(RDSDs: [RDSDField]) {
            self.RDSDs = RDSDs
        }
        init(_ r: BinaryReader, _ dataSize: Int) {
            type = r.readLEUInt32()
            flags = REGNType(rawValue: r.readByte())!
            priority = r.readByte()
            r.skipBytes(2) // Unused
        }
    }

    public struct RDOTField: CustomStringConvertible {
        public var description: String { return "\(object)" }
        public let object: FormId<Record>
        public let parentIdx: UInt16
        public let density: Float
        public let clustering: UInt8
        public let minSlope: UInt8 // (degrees)
        public let maxSlope: UInt8 // (degrees)
        public let flags: UInt8
        public let radiusWrtParent: UInt16
        public let radius: UInt16
        public let minHeight: Float
        public let maxHeight: Float
        public let sink: Float
        public let sinkVariance: Float
        public let sizeVariance: Float
        public let angleVariance: int3
        public let vertexShading: ColorRef4 // RGB + Shading radius (0 - 200) %

        init(_ r: BinaryReader, _ dataSize: Int) {
            object = FormId<Record>(r.readLEUInt32())
            parentIdx = r.readLEUInt16()
            r.skipBytes(2) // Unused
            density = r.readLESingle()
            clustering = r.readByte()
            minSlope = r.readByte()
            maxSlope = r.readByte()
            flags = r.readByte()
            radiusWrtParent = r.readLEUInt16()
            radius = r.readLEUInt16()
            minHeight = r.readLESingle()
            maxHeight = r.readLESingle()
            sink = r.readLESingle()
            sinkVariance = r.readLESingle()
            sizeVariance = r.readLESingle()
            angleVariance = int3(Int32(r.readLEUInt16()), Int32(r.readLEUInt16()), Int32(r.readLEUInt16()))
            r.skipBytes(2) // Unused
            vertexShading = r.readT(dataSize)
        }
    }

    public struct RDGSField: CustomStringConvertible {
        public var description: String { return "\(grass)" }
        public let grass: FormId<GRASRecord>

        init(_ r: BinaryReader, _ dataSize: Int) {
            grass = FormId<GRASRecord>(r.readLEUInt32())
            r.skipBytes(4) // Unused
        }
    }

    public struct RDSDField: CustomStringConvertible {
        public var description: String { return "\(sound)" }
        public let sound: FormId<SOUNRecord> 
        public let flags: UInt32
        public let chance: UInt32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                sound = FormId<SOUNRecord>(r.readASCIIString(32, format: .zeroPadded))
                flags = 0
                chance = UInt32(r.readByte())
                return
            }
            sound = FormId<SOUNRecord>(r.readLEUInt32())
            flags = r.readLEUInt32()
            chance = r.readLEUInt32() //: float with TES5
        }
    }

    public struct RDWTField: CustomStringConvertible {
        public var description: String { return "\(weather)" }
        public static func sizeOf(for format: GameFormatId) -> Int { return format == .TES4 ? 8 : 12 }
        public let weather: FormId<WTHRRecord>
        public let chance: UInt32
        public let global: FormId<GLOBRecord>

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            weather = FormId<WTHRRecord>(r.readLEUInt32())
            chance = r.readLEUInt32()
            global = format == .TES5 ? FormId<GLOBRecord>(r.readLEUInt32()) : FormId<GLOBRecord>(0)
        }
    }

    // TES3
    public typealias WEATField = (
        clear: UInt8,
        cloudy: UInt8,
        foggy: UInt8,
        overcast: UInt8,
        rain: UInt8,
        thunder: UInt8,
        ash: UInt8,
        blight: UInt8
    )
    
    // TES4
    public class RPLIField {
        public let edgeFalloff: UInt32 // (World Units)
        public var points: [float2]! // Region Point List Data

        init(_ r: BinaryReader, _ dataSize: Int) {
            edgeFalloff = r.readLEUInt32()
        }

        func RPLDField(_ r: BinaryReader, _ dataSize: Int) {
            points = r.readTArray(dataSize, count: dataSize >> 3)
        }
    }

    public override var description: String { return "REGN: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var ICON: STRVField! // Icon / Sleep creature
    public var WNAM: FMIDField<WRLDRecord>! // Worldspace - Region name
    public var RCLR: CREFField! // Map Color (COLORREF)
    public var RDATs = [RDATField]() // Region Data Entries / TES3: Sound Record (order determines the sound priority)
    // TES3
    public var WEAT: WEATField? = nil // Weather Data
    // TES4
    public var RPLIs = [RPLIField]() // Region Areas

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "WNAM",
             "FNAM": WNAM = FMIDField<WRLDRecord>(r, dataSize)
        case "WEAT": WEAT = r.readT(dataSize)
        case "ICON",
             "BNAM": ICON = r.readSTRV(dataSize)
        case "RCLR",
             "CNAM": RCLR = r.readT(dataSize)
        case "SNAM": RDATs.append(RDATField(RDSDs: [RDSDField(r, dataSize, format)]))
        case "RPLI": RPLIs.append(RPLIField(r, dataSize))
        case "RPLD": RPLIs.last!.RPLDField(r, dataSize)
        case "RDAT": RDATs.append(RDATField(r, dataSize))
        case "RDOT":
            var rdot = [RDOTField](); RDATs.last!.RDOTs = rdot; let capacity = dataSize / 52; rdot.reserveCapacity(capacity)
            for _ in 0..<capacity { rdot.append(RDOTField(r, dataSize)) }
        case "RDMP": RDATs.last!.RDMP = r.readSTRV(dataSize)
        case "RDGS":
            var rdgs = [RDGSField](); RDATs.last!.RDGSs = rdgs; let capacity = dataSize / 8; rdgs.reserveCapacity(capacity)
            for _ in 0..<capacity { rdgs.append(RDGSField(r, dataSize)) }
        case "RDMD": RDATs.last!.RDMD = r.readT(dataSize)
        case "RDSD":
            var rdsd = [RDSDField](); RDATs.last!.RDSDs = rdsd; let capacity = dataSize / 15; rdsd.reserveCapacity(capacity)
            for _ in 0..<capacity { rdsd.append(RDSDField(r, dataSize, format)) }
        case "RDWT":
            var rdwt = [RDWTField](); RDATs.last!.RDWTs = rdwt; let capacity = dataSize / RDWTField.sizeOf(for: format); rdwt.reserveCapacity(capacity)
            for _ in 0..<capacity { rdwt.append(RDWTField(r, dataSize, format)) }
        default: return false
        }
        return true
    }
}
