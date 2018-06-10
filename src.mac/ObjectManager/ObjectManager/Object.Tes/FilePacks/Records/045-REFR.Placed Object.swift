//
//  REFRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class REFRRecord: Record {
    public struct XTELField {
        public let door: FormId<REFRRecord>
        public let position: SCNVector3
        public let rotation: SCNVector3

        init(_ r: BinaryReader, _ dataSize: Int) {
            door = FormId<REFRRecord>(r.readLEUInt32())
            position = SCNVector3(r.readLESingle(), r.readLESingle(), r.readLESingle())
            rotation = SCNVector3(r.readLESingle(), r.readLESingle(), r.readLESingle())
        }
    }

    public struct DATAField {
        public let position: SCNVector3
        public let rotation: SCNVector3

        init(_ r: BinaryReader, _ dataSize: Int) {
            position = SCNVector3(r.readLESingle(), r.readLESingle(), r.readLESingle())
            rotation = SCNVector3(r.readLESingle(), r.readLESingle(), r.readLESingle())
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
        public var description: String { return "\(FULL)" }
        public var FNAM: BYTEField // Map Flags
        public var FULL: STRVField // Name
        public var TNAM: BYTEField // Type
        
        init() {
        }
    }

    public override var description: String { return "REFR: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var NAME: FMIDField<Record> // Base
    public var XTEL: XTELField? // Teleport Destination (optional)
    public var DATA: DATAField // Position/Rotation
    public var XLOC: XLOCField? // Lock information (optional)
    public var XOWNs: [CELLRecord.XOWNGroup]? // Ownership (optional)
    public var XESP: XESPField? // Enable Parent (optional)
    public var XTRG: FMIDField<Record>? // Target (optional)
    public var XSED: XSEDField? // SpeedTree (optional)
    public var XLOD: BYTVField? // Distant LOD Data (optional)
    public var XCHG: FLTVField? // Charge (optional)
    public var XHLT: FLTVField? // Health (optional)
    public var XPCI: FMIDField<CELLRecord>? // Unused (optional)
    public var XLCM: IN32Field? // Level Modifier (optional)
    public var XRTM: FMIDField<REFRRecord>? // Unknown (optional)
    public var XACT: UI32Field? // Action Flag (optional)
    public var XCNT: IN32Field? // Count (optional)
    public var XMRKs: [XMRKGroup] // Ownership (optional)
    //public var ONAM: Bool? // Open by Default
    public var XRGD: BYTVField? // Ragdoll Data (optional)
    public var XSCL: FLTVField? // Scale (optional)
    public var XSOL: BYTEField? // Contained Soul (optional)
    var _nextFull: Int

    init() {
    }
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "NAME": NAME = FMIDField<Record>(r, dataSize)
        case "XTEL": XTEL = XTELField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "XLOC": XLOC = XLOCField(r, dataSize)
        case "XOWN": if XOWNs == nil { XOWNs = [CELLRecord.XOWNGroup]() }; XOWNs!.append(CELLRecord.XOWNGroup(XOWN: FMIDField<Record>(r, dataSize)))
        case "XRNK": XOWNs!.last!.XRNK = IN32Field(r, dataSize)
        case "XGLB": XOWNs!.last!.XGLB = FMIDField<Record>(r, dataSize)
        case "XESP": XESP = XESPField(r, dataSize)
        case "XTRG": XTRG = FMIDField<Record>(r, dataSize)
        case "XSED": XSED = XSEDField(r, dataSize)
        case "XLOD": XLOD = BYTVField(r, dataSize)
        case "XCHG": XCHG = FLTVField(r, dataSize)
        case "XHLT": XCHG = FLTVField(r, dataSize)
        case "XPCI": XPCI = FMIDField<CELLRecord>(r, dataSize); _nextFull = 1
        case "FULL":
            if _nextFull == 1 { XPCI?.value.adding(name: r.readASCIIString(dataSize)) }
            else if _nextFull == 2 { XMRKs.last!.FULL = STRVField(r, dataSize) }
            _nextFull = 0
        case "XLCM": XLCM = IN32Field(r, dataSize)
        case "XRTM": XRTM = FMIDField<REFRRecord>(r, dataSize)
        case "XACT": XACT = UI32Field(r, dataSize)
        case "XCNT": XCNT = IN32Field(r, dataSize)
        case "XMRK": if XMRKs == nil { XMRKs = [XMRKGroup]() }; XMRKs.append(XMRKGroup()); _nextFull = 2
        case "FNAM": XMRKs.last!.FNAM = BYTEField(r, dataSize)
        case "TNAM": XMRKs.last!.TNAM = BYTEField(r, dataSize); _ = r.readByte()
        case "ONAM": break
        case "XRGD": XRGD = BYTVField(r, dataSize)
        case "XSCL": XSCL = FLTVField(r, dataSize)
        case "XSOL": XSOL = BYTEField(r, dataSize)
        default: return false
        }
        return true
    }
}
