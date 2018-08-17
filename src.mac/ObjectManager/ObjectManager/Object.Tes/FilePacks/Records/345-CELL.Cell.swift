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

    public typealias XCLCField = (
        gridX: Int32,
        gridY: Int32,
        flags: UInt32)

    public typealias XCLLField = (
        ambientColor: ColorRef4,
        directionalColor: ColorRef4, //: SunlightColor
        fogColor: ColorRef4,
        fogNear: Float, //: FogDensity
        // TES4
        fogFar: Float,
        directionalRotationXY: Int32,
        directionalRotationZ: Int32,
        directionalFade: Float,
        fogClipDist: Float,
        // TES5
        fogPow: Float
    )

    public class XOWNGroup {
        public var XOWN: FMIDField<Record>
        public var XRNK: IN32Field! // Faction rank
        public var XGLB: FMIDField<Record>!
        
        init(XOWN: FMIDField<Record>) {
            self.XOWN = XOWN
        }
    }

    public class RefObj: CustomStringConvertible {
        public typealias XYZAField = (
            position: Float3,
            eulerAngles: Float3
        )

        public var FRMR: UI32Field? = nil // Object Index (starts at 1)
        // This is used to uniquely identify objects in the cell. For files the index starts at 1 and is incremented for each object added. For modified
        // objects the index is kept the same.
        public var description: String { return "CREF: \(EDID)" }
        public var EDID: STRVField = STRVField_empty // Object ID
        public var XSCL: FLTVField? = nil // Scale (Static)
        public var DELE: IN32Field? = nil // Indicates that the reference is deleted.
        public var DODT: XYZAField? = nil // XYZ Pos, XYZ Rotation of exit
        public var DNAM: STRVField! // Door exit name (Door objects)
        public var FLTV: FLTVField? = nil // Follows the DNAM optionally, lock level
        public var KNAM: STRVField! // Door key
        public var TNAM: STRVField! // Trap name
        public var UNAM: BYTEField? = nil // Reference Blocked (only occurs once in MORROWIND.ESM)
        public var ANAM: STRVField! // Owner ID string
        public var BNAM: STRVField! // Global variable/rank ID
        public var INTV: IN32Field? = nil // Number of uses, occurs even for objects that don't use it
        public var NAM9: UI32Field? = nil // Unknown
        public var XSOL: STRVField! // Soul Extra Data (ID string of creature)
        public var DATA: XYZAField! // Ref Position Data
        //
        public var CNAM: STRVField!  // Unknown
        public var NAM0: UI32Field? = nil // Unknown
        public var XCHG: IN32Field? = nil // Unknown
        public var INDX: IN32Field? = nil // Unknown
    }

    public override var description: String { return "CELL: \(FULL!)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID. Can be an empty string for exterior cells in which case the region name is used instead.
    public var FULL: STRVField! // Full Name / TES3:RGNN - Region name
    public var DATA: UI16Field! // Flags
    public var XCLC: XCLCField? = nil // Cell Data (only used for exterior cells)
    public var XCLL: XCLLField? = nil // Lighting (only used for interior cells)
    public var XCLW: FLTVField? = nil // Water Height
    // TES3
    public var NAM0: UI32Field? = nil // Number of objects in cell in current file (Optional)
    public var INTV: INTVField! // Unknown
    public var NAM5: CREFField? = nil // Map Color (COLORREF)
    // TES4
    public var XCLRs: [FMIDField<REGNRecord>]!  // Regions
    public var XCMT: BYTEField? = nil // Music (optional)
    public var XCCM: FMIDField<CLMTRecord>? = nil // Climate
    public var XCWT: FMIDField<WATRRecord>? = nil // Water
    public var XOWNs = [XOWNGroup]() // Ownership

    // Referenced Object Data Grouping
    public var inFRMR = false
    public var refObjs = [RefObj]()
    var _lastRef: RefObj!

    public var isInterior: Bool { return Utils.containsBitFlags(UInt(DATA), 0x01) }
    public var gridId: int3!
    //public var ambientLight: CGColor? { return XCLL != nil ? XCLL!.ambientColor.toColor32 : nil }
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if !inFRMR && type == "FRMR" {
            inFRMR = true
        }
        if !inFRMR {
            switch type {
            case "EDID",
                 "NAME": EDID = r.readSTRV(dataSize)
            case "FULL",
                 "RGNN": FULL = r.readSTRV(dataSize)
            case "DATA": DATA = UI16Field(r.readINTV(format == .TES3 ? 4 : dataSize)); if format == .TES3 { fallthrough }
            case "XCLC": XCLC = r.readT(format == .TES3 ? 8 : dataSize)
            case "XCLL",
                 "AMBI": XCLL = r.readT(dataSize)
            case "XCLW",
                 "WHGT": XCLW = r.readT(dataSize)
            // TES3
            case "NAM0": NAM0 = r.readT(dataSize)
            case "INTV": INTV = r.readINTV(dataSize)
            case "NAM5": NAM5 = r.readT(dataSize)
            // TES4
            case "XCLR":
                XCLRs = [FMIDField<REGNRecord>](); let capacity = dataSize >> 2; XCLRs.reserveCapacity(capacity)
                for _ in 0..<capacity { XCLRs.append(FMIDField<REGNRecord>(r, 4)) }
            case "XCMT": XCMT = r.readT(dataSize)
            case "XCCM": XCCM = FMIDField<CLMTRecord>(r, dataSize)
            case "XCWT": XCWT = FMIDField<WATRRecord>(r, dataSize)
            case "XOWN": XOWNs.append(XOWNGroup(XOWN: FMIDField<Record>(r, dataSize)))
            case "XRNK": XOWNs.last!.XRNK = r.readT(dataSize)
            case "XGLB": XOWNs.last!.XGLB = FMIDField<Record>(r, dataSize)
            default: return false
            }
            return true
        }
        // Referenced Object Data Grouping
        else {
            switch type {
            // RefObjDataGroup sub-records
            case "FRMR":
                _lastRef = RefObj(); refObjs.append(_lastRef)
                _lastRef.FRMR = r.readT(dataSize)
            case "NAME": _lastRef.EDID = r.readSTRV(dataSize)
            case "XSCL": _lastRef.XSCL = r.readT(dataSize)
            case "DODT": _lastRef.DODT = r.readT(dataSize)
            case "DNAM": _lastRef.DNAM = r.readSTRV(dataSize)
            case "FLTV": _lastRef.FLTV = r.readT(dataSize)
            case "KNAM": _lastRef.KNAM = r.readSTRV(dataSize)
            case "TNAM": _lastRef.TNAM = r.readSTRV(dataSize)
            case "UNAM": _lastRef.UNAM = r.readT(dataSize)
            case "ANAM": _lastRef.ANAM = r.readSTRV(dataSize)
            case "BNAM": _lastRef.BNAM = r.readSTRV(dataSize)
            case "INTV": _lastRef.INTV = r.readT(dataSize)
            case "NAM9": _lastRef.NAM9 = r.readT(dataSize)
            case "XSOL": _lastRef.XSOL = r.readSTRV(dataSize)
            case "DATA": _lastRef.DATA = r.readT(dataSize)
            case "CNAM": _lastRef.CNAM = r.readSTRV(dataSize)
            case "NAM0": _lastRef.NAM0 = r.readT(dataSize)
            case "XCHG": _lastRef.XCHG = r.readT(dataSize)
            case "INDX": _lastRef.INDX = r.readT(dataSize)
            default: return false
            }
            return true
        }
    }
}
