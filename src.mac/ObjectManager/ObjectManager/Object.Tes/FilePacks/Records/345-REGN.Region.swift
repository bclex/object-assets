//
//  REGNRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class REGNRecord: Record, IHaveEDID {
    // TESX
    public class RDATField {
        public enum REGNType: UInt8 {
            case objects = 2, weather, map, landscape, grass, sound
        }

        public type: UInt32
        public flags: REGNType
        public priority: UInt8
        // groups
        public RDOTs: [RDOTField] // Objects
        public RDMP: STRVField // MapName
        public RDGSs: [RDGSField] // Grasses
        public RDMD: UI32Field // Music Type
        public RDSDs: [RDSDField] // Sounds
        public RDWTs: [RDWTField] // Weather Types

        init() { }
        init(_ r: BinaryReader, _ dataSize: Int) {
            type = r.readLEUInt32()
            flags = REGNType(rawValue: r.readByte())
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
        public let angleVariance: Vector3Int
        public let vertexShading: ColorRef // RGB + Shading radius (0 - 200) %

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
            angleVariance = Vector3Int(r.readLEUInt16(), r.readLEUInt16(), r.readLEUInt16())
            r.skipBytes(2) // Unused
            vertexShading = ColorRef(r)
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
                sound = FormId<SOUNRecord>(r.readASCIIString(32, .zeroPadded))
                flags = 0
                chance = r.readByte()
                return
            }
            sound = FormId<SOUNRecord>(r.readLEUInt32())
            flags = r.readLEUInt32()
            chance = r.readLEUInt32() //: float with TES5
        }
    }

    public struct RDWTField: CustomStringConvertible {
        public var description: String { return "\(weather)" }
        public let weather: FormId<WTHRRecord>
        public let chance: UInt32
        public static var sizeOf(for format: GameFormatId): UInt8 { return format == .TES4 ? 8 : 12 }
        public let global: FormId<GLOBRecord>

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            weather = FormId<WTHRRecord>(r.readLEUInt32())
            chance = r.readLEUInt32()
            global = format == .TES5 ? FormId<GLOBRecord>(r.readLEUInt32()) : FormId<GLOBRecord>()
        }
    }

    // TES3
    public struct WEATField {
        public let clear: UInt8
        public let cloudy: UInt8
        public let foggy: UInt8
        public let overcast: UInt8
        public let rain: UInt8
        public let thunder: UInt8
        public let ash: UInt8
        public let blight: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            clear = r.readByte()
            cloudy = r.readByte()
            foggy = r.readByte()
            overcast = r.readByte()
            rain = r.readByte()
            thunder = r.readByte()
            ash = r.readByte()
            blight = r.readByte()
            // v1.3 ESM files add 2 bytes to WEAT subrecords.
            if dataSize == 10 {
                r.skipBytes(2)
            }
        }
    }

    // TES4
    public class RPLIField {
        public let edgeFalloff: UInt32 // (World Units)
        public var points: [CGVector] // Region Point List Data

        init(_ r: BinaryReader, _ dataSize: Int) {
            edgeFalloff = r.readLEUInt32()
        }

        func RPLDField(_ r: BinaryReader, _ dataSize: Int) {
            points = [CGVector](); points.reserveCapacity(dataSize >> 3)
            for i in 0..<points.capacity {
                points[i] = CGVector(r.readLESingle(), r.readLESingle())
            }
        }
    }

    public var description: String { return "REGN: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var ICON: STRVField // Icon / Sleep creature
    public var WNAM: FMIDField<WRLDRecord> // Worldspace - Region name
    public var RCLR: CREFField // Map Color (COLORREF)
    public var RDATs = [RDATField]() // Region Data Entries / TES3: Sound Record (order determines the sound priority)
    // TES3
    public var WEAT: WEATField? // Weather Data
    // TES4
    public var RPLIs = [RPLIField]() // Region Areas

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "WNAM",
             "FNAM": WNAM = FMIDField<WRLDRecord>(r, dataSize)
        case "WEAT": WEAT = WEATField(r, dataSize)
        case "ICON",
             "BNAM": ICON = STRVField(r, dataSize)
        case "RCLR",
             "CNAM": RCLR = CREFField(r, dataSize)
        case "SNAM": RDATs.append(RDATField(RDSDs: [RDSDField(r, dataSize, format)]))
        case "RPLI": RPLIs.append(RPLIField(r, dataSize))
        case "RPLD": RPLIs.last!.RPLDField(r, dataSize)
        case "RDAT": RDATs.append(RDATField(r, dataSize))
        case "RDOT": var rdot = RDATs.last!.RDOTs = [RDOTField](); rdot.reserveCapactiy(dataSize / 52); for i in 0..<rdot.capacity { rdot[i] = RDOTField(r, dataSize) }
        case "RDMP": RDATs.last!.RDMP = STRVField(r, dataSize)
        case "RDGS": var rdgs = RDATs.last!.RDGSs = [RDGSField](); rdgs.reserveCapacity(dataSize / 8); for i in 0..<rdgs.capacity { rdgs[i] = RDGSField(r, dataSize) }
        case "RDMD": RDATs.last!.RDMD = UI32Field(r, dataSize)
        case "RDSD": var rdsd = RDATs.last!.RDSDs = [RDSDField](); rdsd.reserveCapacity(dataSize / 12); for i in 0..<rdsd.capacity { rdsd[i] = RDSDField(r, dataSize, format) }
        case "RDWT": var rdwt = RDATs.last!.RDWTs = [RDWTField](); rdwt.reserveCapactiy(dataSize / RDWTField.SizeOf(format)); for i in 0..<rdwt.capacity { rdwt[i] = RDWTField(r, dataSize, format) }
        default: return false
        }
        return true
    }
}
