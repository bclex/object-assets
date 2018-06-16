//
//  CELLRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import SceneKit
import CoreGraphics

public class CELLRecord: Record, ICellRecord {
    
    public struct CELLFlags: OptionSet {
        public let rawValue: UInt16
        public static let interior = CELLFlags(rawValue: 0x0001)
        public static let hasWater = CELLFlags(rawValue: 0x0002)
        public static let invertFastTravel = CELLFlags(rawValue: 0x0004) //: IllegalToSleepHere
        public static let behaveLikeExterior = CELLFlags(rawValue: 0x0008) //: BehaveLikeExterior (Tribunal), Force hide land (exterior cell) / Oblivion interior (interior cell)
        public static let unknown1 = CELLFlags(rawValue: 0x0010)
        public static let publicArea = CELLFlags(rawValue: 0x0020) // Public place
        public static let handChanged = CELLFlags(rawValue: 0x0040)
        public static let showSky = CELLFlags(rawValue: 0x0080) // Behave like exterior
        public static let useSkyLighting = CELLFlags(rawValue: 0x0100)
        
        public init(rawValue: UInt16) {
            self.rawValue = rawValue
        }
    }

    public struct XCLCField {
        public let gridX: Int32
        public let gridY: Int32
        public let flags: UInt32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            gridX = r.readLEInt32()
            gridY = r.readLEInt32()
            flags = format == .TES5 ? r.readLEUInt32() : 0
        }
    }

    public struct XCLLField {
        public let ambientColor: ColorRef
        public let directionalColor: ColorRef //: SunlightColor
        public let fogColor: ColorRef
        public let fogNear: Float //: FogDensity
        // TES4
        public let fogFar: Float
        public let directionalRotationXY: Int32
        public let directionalRotationZ: Int32
        public let directionalFade: Float
        public let fogClipDist: Float
        // TES5
        public let fogPow: Float

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            ambientColor = ColorRef(r);
            directionalColor = ColorRef(r);
            fogColor = ColorRef(r);
            fogNear = r.readLESingle();
            guard format == .TES3 else {
                fogFar = 0; directionalFade = 0; fogClipDist = 0; directionalRotationXY = 0; directionalRotationZ = 0
                fogPow = 0
                return
            }
            fogFar = r.readLESingle()
            directionalRotationXY = r.readLEInt32()
            directionalRotationZ = r.readLEInt32()
            directionalFade = r.readLESingle()
            fogClipDist = r.readLESingle()
            guard format == .TES4 else {
                fogPow = 0
                return
            }
            fogPow = r.readLESingle()
        }
    }

    public class XOWNGroup {
        public var XOWN: FMIDField<Record>
        public var XRNK: IN32Field // Faction rank
        public var XGLB: FMIDField<Record>
    }

    public class RefObj: CustomStringConvertible {
        public struct XYZAField {
            public let position: Vector3
            public let eulerAngles: Vector3

            init(_ r: BinaryReader, _ dataSize: Int) {
                position = r.readLEVector3()
                eulerAngles = r.readLEVector3()
            }
        }

        public var FRMR: UI32Field? // Object Index (starts at 1)
        // This is used to uniquely identify objects in the cell. For files the index starts at 1 and is incremented for each object added. For modified
        // objects the index is kept the same.
        public var description: String { return "CREF: \(EDID)" }
        public var EDID: STRVField // Object ID
        public var XSCL: FLTVField? // Scale (Static)
        public var DELE: IN32Field? // Indicates that the reference is deleted.
        public var DODT: XYZAField? // XYZ Pos, XYZ Rotation of exit
        public var DNAM: STRVField // Door exit name (Door objects)
        public var FLTV: FLTVField? // Follows the DNAM optionally, lock level
        public var KNAM: STRVField // Door key
        public var TNAM: STRVField // Trap name
        public var UNAM: BYTEField? // Reference Blocked (only occurs once in MORROWIND.ESM)
        public var ANAM: STRVField // Owner ID string
        public var BNAM: STRVField // Global variable/rank ID
        public var INTV: IN32Field? // Number of uses, occurs even for objects that don't use it
        public var NAM9: UI32Field? // Unknown
        public var XSOL: STRVField // Soul Extra Data (ID string of creature)
        public var DATA: XYZAField // Ref Position Data
        //
        public var CNAM: STRVField  // Unknown
        public var NAM0: UI32Field? // Unknown
        public var XCHG: IN32Field? // Unknown
        public var INDX: IN32Field? // Unknown
    }

    public override var description: String { return "CELL: \(FULL)" }
    public var EDID: STRVField  // Editor ID. Can be an empty string for exterior cells in which case the region name is used instead.
    public var FULL: STRVField // Full Name / TES3:RGNN - Region name
    public var DATA: UI16Field // Flags
    public var XCLC: XCLCField? // Cell Data (only used for exterior cells)
    public var XCLL: XCLLField? // Lighting (only used for interior cells)
    public var XCLW: FLTVField? // Water Height
    // TES3
    public var NAM0: UI32Field? // Number of objects in cell in current file (Optional)
    public var INTV: INTVField // Unknown
    public var NAM5: CREFField? // Map Color (COLORREF)
    // TES4
    public var XCLRs: [FMIDField<REGNRecord>]  // Regions
    public var XCMT: BYTEField? // Music (optional)
    public var XCCM: FMIDField<CLMTRecord>? // Climate
    public var XCWT: FMIDField<WATRRecord>? // Water
    public var XOWNs = [XOWNGroup]() // Ownership

    // Referenced Object Data Grouping
    public var InFRMR = false
    public var RefObjs = [RefObj]()

    public var isInterior: Bool { return Utils.containsBitFlags(DATA.value, 0x01) }
    public var gridCoords: Vector2Int { return Vector2Int(Int(XCLC!.gridX), Int(XCLC!.gridY)) }
    public var ambientLight: CGColor? { return XCLL != nil ? XCLL!.ambientColor.toColor32() : nil }

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if !InFRMR && type == "FRMR" {
            InFRMR = true
        }
        if !InFRMR {
            switch type {
            case "EDID",
                 "NAME": EDID = STRVField(r, dataSize)
            case "FULL",
                 "RGNN": FULL = STRVField(r, dataSize)
            case "DATA": DATA = INTVField(r, format == .TES3 ? 4 : dataSize).toUI16Field(); if format == .TES3 { fallthrough }
            case "XCLC": XCLC = XCLCField(r, dataSize, format)
            case "XCLL",
                 "AMBI": XCLL = XCLLField(r, dataSize, format)
            case "XCLW",
                 "WHGT": XCLW = FLTVField(r, dataSize)
            // TES3
            case "NAM0": NAM0 = UI32Field(r, dataSize)
            case "INTV": INTV = INTVField(r, dataSize)
            case "NAM5": NAM5 = CREFField(r, dataSize)
            // TES4
            case "XCLR":
                XCLRs = [FMIDField<REGNRecord>](); XCLRs.reserveCapacity(dataSize >> 2); for i in 0..<XCLRs.capactiy { XCLRs[i] = FMIDField<REGNRecord>(r, 4) }
            case "XCMT": XCMT = BYTEField(r, dataSize)
            case "XCCM": XCCM = FMIDField<CLMTRecord>(r, dataSize)
            case "XCWT": XCWT = FMIDField<WATRRecord>(r, dataSize)
            case "XOWN": XOWNs.append(XOWNGroup(XOWN: FMIDField<Record>(r, dataSize)))
            case "XRNK": XOWNs.last!.XRNK = IN32Field(r, dataSize)
            case "XGLB": XOWNs.last!.XGLB = FMIDField<Record>(r, dataSize)
            default: return false
            }
            return true
        }
        // Referenced Object Data Grouping
        else {
            switch type {
            // RefObjDataGroup sub-records
            case "FRMR": RefObjs.append(RefObj()); RefObjs.last!.FRMR = UI32Field(r, dataSize)
            case "NAME": RefObjs.last!.EDID = STRVField(r, dataSize)
            case "XSCL": RefObjs.last!.XSCL = FLTVField(r, dataSize)
            case "DODT": RefObjs.last!.DODT = RefObj.XYZAField(r, dataSize)
            case "DNAM": RefObjs.last!.DNAM = STRVField(r, dataSize)
            case "FLTV": RefObjs.last!.FLTV = FLTVField(r, dataSize)
            case "KNAM": RefObjs.last!.KNAM = STRVField(r, dataSize)
            case "TNAM": RefObjs.last!.TNAM = STRVField(r, dataSize)
            case "UNAM": RefObjs.last!.UNAM = BYTEField(r, dataSize)
            case "ANAM": RefObjs.last!.ANAM = STRVField(r, dataSize)
            case "BNAM": RefObjs.last!.BNAM = STRVField(r, dataSize)
            case "INTV": RefObjs.last!.INTV = IN32Field(r, dataSize)
            case "NAM9": RefObjs.last!.NAM9 = UI32Field(r, dataSize)
            case "XSOL": RefObjs.last!.XSOL = STRVField(r, dataSize)
            case "DATA": RefObjs.last!.DATA = RefObj.XYZAField(r, dataSize)
            //
            case "CNAM": RefObjs.last!.CNAM = STRVField(r, dataSize)
            case "NAM0": RefObjs.last!.NAM0 = UI32Field(r, dataSize)
            case "XCHG": RefObjs.last!.XCHG = IN32Field(r, dataSize)
            case "INDX": RefObjs.last!.INDX = IN32Field(r, dataSize)
            default: return false
            }
            return true
        }
    }
}
