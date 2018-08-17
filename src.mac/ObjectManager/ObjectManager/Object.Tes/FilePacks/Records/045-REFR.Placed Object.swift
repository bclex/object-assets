//
//  REFRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
import simd

public class REFRRecord: Record {
    public struct XTELField {
        public let door: FormId<REFRRecord>
        public let position: float3
        public let rotation: float3

        init(_ r: BinaryReader, _ dataSize: Int) {
            door = FormId<REFRRecord>(r.readLEUInt32())
            position = r.readLEFloat3()
            rotation = r.readLEFloat3()
        }
    }

    public struct DATAField {
        public let position: float3
        public let rotation: float3

        init(_ r: BinaryReader, _ dataSize: Int) {
            position = r.readLEFloat3()
            rotation = r.readLEFloat3()
        }
    }

    public struct XLOCField: CustomStringConvertible {
        public var description: String { return "\(key)" }
        public let lockLevel: UInt8
        public let key: FormId<KEYMRecord>
        public let flags: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            lockLevel = r.readByte()
            r.skipBytes(3) // Unused
            key = FormId<KEYMRecord>(r.readLEUInt32())
            if dataSize == 16 {
                r.skipBytes(4) // Unused
            }
            flags = r.readByte()
            r.skipBytes(3) // Unused
        }
    }

    public struct XESPField: CustomStringConvertible {
        public var description: String { return "\(reference)" }
        public let reference: FormId<Record>
        public let flags: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            reference = FormId<Record>(r.readLEUInt32())
            flags = r.readByte()
            r.skipBytes(3) // Unused
        }
    }

    public struct XSEDField: CustomStringConvertible {
        public var description: String { return "\(seed)" }
        public let seed: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            seed = r.readByte()
            if dataSize == 4 {
                r.skipBytes(3) // Unused
            }
        }
    }

    public class XMRKGroup: CustomStringConvertible {
        public var description: String { return "\(FULL!)" }
        public var FNAM: BYTEField! // Map Flags
        public var FULL: STRVField! // Name
        public var TNAM: BYTEField! // Type
    }

    public override var description: String { return "REFR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var NAME: FMIDField<Record>! // Base
    public var XTEL: XTELField? = nil// Teleport Destination (optional)
    public var DATA: DATAField! // Position/Rotation
    public var XLOC: XLOCField? = nil // Lock information (optional)
    public var XOWNs: [CELLRecord.XOWNGroup]? = nil // Ownership (optional)
    public var XESP: XESPField? = nil // Enable Parent (optional)
    public var XTRG: FMIDField<Record>? = nil // Target (optional)
    public var XSED: XSEDField? = nil // SpeedTree (optional)
    public var XLOD: BYTVField? = nil // Distant LOD Data (optional)
    public var XCHG: FLTVField? = nil // Charge (optional)
    public var XHLT: FLTVField? = nil // Health (optional)
    public var XPCI: FMIDField<CELLRecord>? = nil // Unused (optional)
    public var XLCM: IN32Field? = nil // Level Modifier (optional)
    public var XRTM: FMIDField<REFRRecord>? = nil // Unknown (optional)
    public var XACT: UI32Field? = nil // Action Flag (optional)
    public var XCNT: IN32Field? = nil // Count (optional)
    public var XMRKs: [XMRKGroup]? = nil // Ownership (optional)
    //public var ONAM: Bool? // Open by Default
    public var XRGD: BYTVField? = nil // Ragdoll Data (optional)
    public var XSCL: FLTVField? = nil // Scale (optional)
    public var XSOL: BYTEField? = nil // Contained Soul (optional)
    var _nextFull: Int!
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "NAME": NAME = FMIDField<Record>(r, dataSize)
        case "XTEL": XTEL = XTELField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "XLOC": XLOC = XLOCField(r, dataSize)
        case "XOWN": if XOWNs == nil { XOWNs = [CELLRecord.XOWNGroup]() }; XOWNs!.append(CELLRecord.XOWNGroup(XOWN: FMIDField<Record>(r, dataSize)))
        case "XRNK": XOWNs!.last!.XRNK = r.readT(dataSize)
        case "XGLB": XOWNs!.last!.XGLB = FMIDField<Record>(r, dataSize)
        case "XESP": XESP = XESPField(r, dataSize)
        case "XTRG": XTRG = FMIDField<Record>(r, dataSize)
        case "XSED": XSED = XSEDField(r, dataSize)
        case "XLOD": XLOD = r.readBYTV(dataSize)
        case "XCHG": XCHG = r.readT(dataSize)
        case "XHLT": XCHG = r.readT(dataSize)
        case "XPCI": XPCI = FMIDField<CELLRecord>(r, dataSize); _nextFull = 1
        case "FULL":
            if _nextFull == 1 { XPCI!.add(name: r.readASCIIString(dataSize)) }
            else if _nextFull == 2 { XMRKs!.last!.FULL = r.readSTRV(dataSize) }
            _nextFull = 0
        case "XLCM": XLCM = r.readT(dataSize)
        case "XRTM": XRTM = FMIDField<REFRRecord>(r, dataSize)
        case "XACT": XACT = r.readT(dataSize)
        case "XCNT": XCNT = r.readT(dataSize)
        case "XMRK": if XMRKs == nil { XMRKs = [XMRKGroup]() }; XMRKs!.append(XMRKGroup()); _nextFull = 2
        case "FNAM": XMRKs!.last!.FNAM = r.readT(dataSize)
        case "TNAM": XMRKs!.last!.TNAM = r.readT(dataSize) //; r.skipBytes(1)
        case "ONAM": break
        case "XRGD": XRGD = r.readBYTV(dataSize)
        case "XSCL": XSCL = r.readT(dataSize)
        case "XSOL": XSOL = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
