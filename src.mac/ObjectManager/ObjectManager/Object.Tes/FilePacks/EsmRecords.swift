//
//  EsmRecords.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public enum GameFormatId {
    case TES3, TES4, TES5
}

public class Header: CustomStringConvertible {
    public struct Flags: OptionSet {
        public let rawValue: UInt32
        public static let esmFile = Flags(rawValue: 0x00000001)               // ESM file. (TES4.HEDR record only.)
        public static let deleted = Flags(rawValue: 0x00000020)               // Deleted
        public static let r00 = Flags(rawValue: 0x00000040)                   // Constant / (REFR) Hidden From Local Map (Needs Confirmation: Related to shields)
        public static let r01 = Flags(rawValue: 0x00000100)                   // Must Update Anims / (REFR) Inaccessible
        public static let r02 = Flags(rawValue: 0x00000200)                   // (REFR) Hidden from local map / (ACHR) Starts dead / (REFR) MotionBlurCastsShadows
        public static let r03 = Flags(rawValue: 0x00000400)                   // Quest item / Persistent reference / (LSCR) Displays in Main Menu
        public static let initiallyDisabled = Flags(rawValue: 0x00000800)     // Initially disabled
        public static let ignored = Flags(rawValue: 0x00001000)               // Ignored
        public static let visibleWhenDistant = Flags(rawValue: 0x00008000)    // Visible when distant
        public static let r04 = Flags(rawValue: 0x00010000)                   // (ACTI) Random Animation Start
        public static let r05 = Flags(rawValue: 0x00020000)                   // (ACTI) Dangerous / Off limits (Interior cell) Dangerous Can't be set withough Ignore Object Interaction
        public static let compressed = Flags(rawValue: 0x00040000)            // Data is compressed
        public static let cantWait = Flags(rawValue: 0x00080000)              // Can't wait
                                                // tes5
        public static let r06 = 0x00100000                   // (ACTI) Ignore Object Interaction Ignore Object Interaction Sets Dangerous Automatically
        public static let isMarker = 0x00800000              // Is Marker
        public static let r07 = 0x02000000                   // (ACTI) Obstacle / (REFR) No AI Acquire
        public static let navMesh01 = 0x04000000             // NavMesh Gen - Filter
        public static let navMesh02 = 0x08000000             // NavMesh Gen - Bounding Box
        public static let r08 = 0x10000000                   // (FURN) Must Exit to Talk / (REFR) Reflected By Auto Water
        public static let r09 = 0x20000000                   // (FURN/IDLM) Child Can Use / (REFR) Don't Havok Settle
        public static let r10 = 0x40000000                   // NavMesh Gen - Ground / (REFR) NoRespawn
        public static let r11 = 0x80000000                   // (REFR) MultiBound
    
        public init(rawValue: UInt32) {
            self.rawValue = rawValue
        }
    }

    public enum GroupType: Int32 {
        case top = 0                    // Label: Record type
        case worldChildren              // Label: Parent (WRLD)
        case interiorCellBlock          // Label: Block number
        case interiorCellSubBlock       // Label: Sub-block number
        case exteriorCellBlock          // Label: Grid Y, X (Note the reverse order)
        case exteriorCellSubBlock       // Label: Grid Y, X (Note the reverse order)
        case cellChildren               // Label: Parent (CELL)
        case topicChildren              // Label: Parent (DIAL)
        case cellPersistentChilden      // Label: Parent (CELL)
        case cellTemporaryChildren      // Label: Parent (CELL)
        case cellVisibleDistantChildren // Label: Parent (CELL)
    }

    public var description: String { return "\(type):\(groupType ?? .top)" }
    public let parent: Header?
    public let type: String // 4 bytes
    public var dataSize: UInt32
    public let flags: Flags
    public var compressed: Bool { return flags.contains(.compressed) }
    public let formId: UInt32
    public var position: UInt64
    // group
    public let label: Data?
    public let groupType: GroupType?

    init(label: Data?, dataSize: UInt32, position: UInt64) {
        parent = nil
        type = ""
        self.dataSize = dataSize
        flags = Flags(rawValue: 0)
        formId = 0
        self.position = position
        self.label = label
        groupType = nil
    }
    init(_ r: BinaryReader, for format: GameFormatId, parent: Header?) {
        self.parent = parent
        type = r.readASCIIString(4)
        if type == "GRUP" {
            dataSize = r.readLEUInt32() - (format == .TES4 ? 20 : 24)
            label = r.readBytes(4)
            groupType = GroupType(rawValue: r.readLEInt32())!
            _ = r.readLEUInt32() // stamp | stamp + uknown
            if format != .TES4 {
                _ = r.readLEUInt32() // version + uknown
            }
            position = r.baseStream.position
            flags = Flags(rawValue: 0)
            formId = 0
            return
        }
        label = nil
        groupType = nil
        dataSize = r.readLEUInt32()
        if format == .TES3 {
            _ = r.readLEUInt32() // Unknown
        }
        flags = Flags(rawValue: r.readLEUInt32())
        if format == .TES3 {
            position = r.baseStream.position
            formId = 0
            return
        }
        // tes4
        formId = r.readLEUInt32()
        _ = r.readLEUInt32()
        if format == .TES4 {
            position = r.baseStream.position
            return
        }
        // tes5
        _ = r.readLEUInt32()
        position = r.baseStream.position
    }

    static let createMap:[String : (f: (Header) -> Record, l: (Int) -> Bool)] = [
        "TES3" : ({x in return TES3Record(x)}, {x in return true}),
        "TES4" : ({x in return TES4Record(x)}, {x in return true}),
        // 0
        "LTEX" : ({x in return LTEXRecord(x)}, {x in return x > 0}),
        "STAT" : ({x in return STATRecord(x)}, {x in return x > 0}),
        "CELL" : ({x in return CELLRecord(x)}, {x in return x > 0}),
        "LAND" : ({x in return LANDRecord(x)}, {x in return x > 0}),
        // 1
        "DOOR" : ({x in return DOORRecord(x)}, {x in return x > 1}),
        "MISC" : ({x in return MISCRecord(x)}, {x in return x > 1}),
        "WEAP" : ({x in return WEAPRecord(x)}, {x in return x > 1}),
        "CONT" : ({x in return CONTRecord(x)}, {x in return x > 1}),
        "LIGH" : ({x in return LIGHRecord(x)}, {x in return x > 1}),
        "ARMO" : ({x in return ARMORecord(x)}, {x in return x > 1}),
        "CLOT" : ({x in return CLOTRecord(x)}, {x in return x > 1}),
        "REPA" : ({x in return REPARecord(x)}, {x in return x > 1}),
        "ACTI" : ({x in return ACTIRecord(x)}, {x in return x > 1}),
        "APPA" : ({x in return APPARecord(x)}, {x in return x > 1}),
        "LOCK" : ({x in return LOCKRecord(x)}, {x in return x > 1}),
        "PROB" : ({x in return PROBRecord(x)}, {x in return x > 1}),
        "INGR" : ({x in return INGRRecord(x)}, {x in return x > 1}),
        "BOOK" : ({x in return BOOKRecord(x)}, {x in return x > 1}),
        "ALCH" : ({x in return ALCHRecord(x)}, {x in return x > 1}),
        "CREA" : ({x in return CREARecord(x)}, {x in return x > 1}), // && BaseSettings.Game.CreaturesEnabled}),
        "NPC_" : ({x in return NPC_Record(x)}, {x in return x > 1}), // && BaseSettings.Game.NpcsEnabled}),
        // 2
        "GMST" : ({x in return GMSTRecord(x)}, {x in return x > 2}),
        "GLOB" : ({x in return GLOBRecord(x)}, {x in return x > 2}),
        "SOUN" : ({x in return SOUNRecord(x)}, {x in return x > 2}),
        "REGN" : ({x in return REGNRecord(x)}, {x in return x > 2}),
        // 3
        "CLAS" : ({x in return CLASRecord(x)}, {x in return x > 3}),
        "SPEL" : ({x in return SPELRecord(x)}, {x in return x > 3}),
        "BODY" : ({x in return BODYRecord(x)}, {x in return x > 3}),
        "PGRD" : ({x in return PGRDRecord(x)}, {x in return x > 3}),
        "INFO" : ({x in return INFORecord(x)}, {x in return x > 3}),
        "DIAL" : ({x in return DIALRecord(x)}, {x in return x > 3}),
        "SNDG" : ({x in return SNDGRecord(x)}, {x in return x > 3}),
        "ENCH" : ({x in return ENCHRecord(x)}, {x in return x > 3}),
        "SCPT" : ({x in return SCPTRecord(x)}, {x in return x > 3}),
        "SKIL" : ({x in return SKILRecord(x)}, {x in return x > 3}),
        "RACE" : ({x in return RACERecord(x)}, {x in return x > 3}),
        "MGEF" : ({x in return MGEFRecord(x)}, {x in return x > 3}),
        "LEVI" : ({x in return LEVIRecord(x)}, {x in return x > 3}),
        "LEVC" : ({x in return LEVCRecord(x)}, {x in return x > 3}),
        "BSGN" : ({x in return BSGNRecord(x)}, {x in return x > 3}),
        "FACT" : ({x in return FACTRecord(x)}, {x in return x > 3}),
        "SSCR" : ({x in return SSCRRecord(x)}, {x in return x > 3}),
        // 4 - Oblivion
        "WRLD" : ({x in return WRLDRecord(x)}, {x in return x > 0}),
        "ACRE" : ({x in return ACRERecord(x)}, {x in return x > 1}),
        "ACHR" : ({x in return ACHRRecord(x)}, {x in return x > 1}),
        "REFR" : ({x in return REFRRecord(x)}, {x in return x > 1}),
        //
        "AMMO" : ({x in return AMMORecord(x)}, {x in return x > 4}),
        "ANIO" : ({x in return ANIORecord(x)}, {x in return x > 4}),
        "CLMT" : ({x in return CLMTRecord(x)}, {x in return x > 4}),
        "CSTY" : ({x in return CSTYRecord(x)}, {x in return x > 4}),
        "EFSH" : ({x in return EFSHRecord(x)}, {x in return x > 4}),
        "EYES" : ({x in return EYESRecord(x)}, {x in return x > 4}),
        "FLOR" : ({x in return FLORRecord(x)}, {x in return x > 4}),
        "FURN" : ({x in return FURNRecord(x)}, {x in return x > 4}),
        "GRAS" : ({x in return GRASRecord(x)}, {x in return x > 4}),
        "HAIR" : ({x in return HAIRRecord(x)}, {x in return x > 4}),
        "IDLE" : ({x in return IDLERecord(x)}, {x in return x > 4}),
        "KEYM" : ({x in return KEYMRecord(x)}, {x in return x > 4}),
        "LSCR" : ({x in return LSCRRecord(x)}, {x in return x > 4}),
        "LVLC" : ({x in return LVLCRecord(x)}, {x in return x > 4}),
        "LVLI" : ({x in return LVLIRecord(x)}, {x in return x > 4}),
        "LVSP" : ({x in return LVSPRecord(x)}, {x in return x > 4}),
        "PACK" : ({x in return PACKRecord(x)}, {x in return x > 4}),
        "QUST" : ({x in return QUSTRecord(x)}, {x in return x > 4}),
        "ROAD" : ({x in return ROADRecord(x)}, {x in return x > 4}),
        "SBSP" : ({x in return SBSPRecord(x)}, {x in return x > 4}),
        "SGST" : ({x in return SGSTRecord(x)}, {x in return x > 4}),
        "SLGM" : ({x in return SLGMRecord(x)}, {x in return x > 4}),
        "TREE" : ({x in return TREERecord(x)}, {x in return x > 4}),
        "WATR" : ({x in return WATRRecord(x)}, {x in return x > 4}),
        "WTHR" : ({x in return WTHRRecord(x)}, {x in return x > 4}),
        // 5 - Skyrim
        "AACT" : ({x in return AACTRecord(x)}, {x in return x > 5}),
        "ADDN" : ({x in return ADDNRecord(x)}, {x in return x > 5}),
        "ARMA" : ({x in return ARMARecord(x)}, {x in return x > 5}),
        "ARTO" : ({x in return ARTORecord(x)}, {x in return x > 5}),
        "ASPC" : ({x in return ASPCRecord(x)}, {x in return x > 5}),
        "ASTP" : ({x in return ASTPRecord(x)}, {x in return x > 5}),
        "AVIF" : ({x in return AVIFRecord(x)}, {x in return x > 5}),
        "DLBR" : ({x in return DLBRRecord(x)}, {x in return x > 5}),
        "DLVW" : ({x in return DLVWRecord(x)}, {x in return x > 5}),
        "SNDR" : ({x in return SNDRRecord(x)}, {x in return x > 5}),
    ]

    func createRecord(at position: UInt64, recordLevel: Int) -> Record? {
        guard let recordType = Header.createMap[type] else {
            debugPrint("Unsupported ESM record type: \(type)")
            return nil
        }
        guard recordType.l(recordLevel) else {
            return nil
        }
        let record = recordType.f(self)
        record.header = self;
        return record;
    }
}

public class RecordGroup: CustomStringConvertible, CustomDebugStringConvertible {
    public var description: String { return "\(headers.first?.description ?? "none")" }
    public var debugDescription: String { return description }
    public var label: Data? { return headers.first!.label }
    public var headers = [Header]()
    public var records = [Record]()
    public var groups: [RecordGroup]?
    public var groupsByLabel: [UInt32 : [RecordGroup]]?
    let _r: BinaryReader
    let _filePath: String
    let _format: GameFormatId
    let _recordLevel: Int
    var _headerSkip = 0
    // Extension
    var _ensureCELLsByLabel: Set<UInt32>!
    var CELLsById: [int3 : CELLRecord]!
    var LANDsById: [int3 : LANDRecord]!

    init(_ r: BinaryReader, _ filePath: String, for format: GameFormatId, recordLevel: Int) {
        _r = r
        _filePath = filePath
        _format = format
        _recordLevel = recordLevel
    }
    convenience init(_ r: BinaryReader, _ filePath: String, for format: GameFormatId, recordLevel: Int, label: String, records: [Record]) {
        self.init(r, filePath, for: format, recordLevel: recordLevel)
        self.records = records
        headers.append(Header(label: Data(label.utf8), dataSize: 0, position: 0))
        _headerSkip = headers.count
    }

    func addHeader(_ header: Header) {
        //debugPrint("Read: \(header.label!)")
        headers.append(header)
        if header.groupType == .top {
            switch String(data: header.label!, encoding: .ascii) {
            case "CELL", "WRLD": load() // "DIAL"
            default: return
            }
        }
    }

    @discardableResult
    public func load(loadAll: Bool = false) -> [Record] {
        guard _headerSkip != headers.count else { return records }
        for i in _headerSkip..<headers.endIndex {
            readGroup(header: headers[i], loadAll: loadAll)
        }
        _headerSkip = headers.count
        return records
    }

    static var _cellIdx = 0
    func readGroup(header: Header, loadAll: Bool) {
        _r.baseStream.position = header.position
        let endPosition = header.position + UInt64(header.dataSize)
        while _r.baseStream.position < endPosition {
            let recordHeader = Header(_r, for: _format, parent: header)
            guard recordHeader.type != "GRUP" else {
                let group = readGRUP(header: header, recordHeader: recordHeader)
                if loadAll {
                    group.load(loadAll: loadAll)
                }
                continue
            }
            if recordHeader.type == "CELL" { RecordGroup._cellIdx += 1 }
            // HACK to limit cells loading
            if (recordHeader.type == "CELL" && (RecordGroup._cellIdx < 20 || RecordGroup._cellIdx > 25)) {
                _r.baseStream.position += UInt64(recordHeader.dataSize)
                continue
            }
            guard let record = recordHeader.createRecord(at: _r.baseStream.position, recordLevel: _recordLevel) else {
                _r.baseStream.position += UInt64(recordHeader.dataSize)
                continue
            }
            readRecord(record, compressed: recordHeader.compressed)
            records.append(record)
        }
        groupsByLabel = groups != nil ? Dictionary(uniqueKeysWithValues:
            Dictionary(grouping: groups!) { x -> UInt32 in Utils.fromData(x.label!) }.map { x -> (key: UInt32, value: [RecordGroup]) in
                (key: x.key, value: x.value) }) : nil
    }

    func readGRUP(header: Header, recordHeader: Header) -> RecordGroup {
        let nextPosition = _r.baseStream.position + UInt64(recordHeader.dataSize)
        if groups == nil {
            groups = [RecordGroup]()
        }
        let group = RecordGroup(_r, _filePath, for: _format, recordLevel: _recordLevel)
        group.addHeader(recordHeader)
        groups!.append(group);
        _r.baseStream.position = nextPosition
        // print header path
        var headerPath = [String](); getHeaderPath(&headerPath, header: header)
        debugPrint("Grup: \(headerPath.joined(separator: "/"))/ \(header.groupType ?? .top)")
        return group
    }

    func getHeaderPath(_ b: inout [String], header: Header) {
        if header.parent != nil { getHeaderPath(&b, header: header.parent!) }
        b.append((header.groupType != .top ?
            String(data: header.label!, encoding: .ascii) :
            String(data: header.label!, encoding: .ascii))!)
    }

    func readRecord(_ record: Record, compressed: Bool) {
//        debugPrint("Recd: \(record.header.type)")
        guard compressed else {
            record.read(_r, _filePath, for: _format)
            return
        }
        let newDataSize = _r.readLEUInt32()
        let data = _r.readBytes(Int(record.header.dataSize - 4))
        let newData = data.inflate(size: Int(newDataSize))!
        // read record
        record.header.position = 0
        record.header.dataSize = newDataSize
        let r = BinaryReader(DataBaseStream(data: newData))
        defer { r.close() }
        record.read(r, _filePath, for: _format)
    }
}

public class Record: IRecord, CustomStringConvertible, CustomDebugStringConvertible {
    public var description: String { return "BASE" }
    public var debugDescription: String { return description }
    var header: Header
    var id: UInt32 { return header.formId }
    
    init(_ header: Header) {
        self.header = header
    }
    
    func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        preconditionFailure("This method must be overridden")
    }

    func read(_ r: BinaryReader, _ filePath: String, for format: GameFormatId) {
        let startPosition = r.baseStream.position
        let endPosition = startPosition + UInt64(header.dataSize)
        while r.baseStream.position < endPosition {
            let fieldHeader = FieldHeader(r, for: format)
            guard fieldHeader.type != "XXXX" else {
                if fieldHeader.dataSize != 4 {
                    fatalError("")
                }
                fieldHeader.dataSize = Int(r.readLEUInt32())
                continue
            }
            guard fieldHeader.type != "OFST" || header.type != "WRLD" else {
                r.baseStream.position = endPosition
                continue
            }
            let position = r.baseStream.position
            guard createField(r, for: format, type: fieldHeader.type, dataSize: fieldHeader.dataSize) else {
                debugPrint("Unsupported ESM record type: \(header.type):\(fieldHeader.type)")
                r.baseStream.position += UInt64(fieldHeader.dataSize)
                continue
            }
            // check full read
            guard r.baseStream.position == position + UInt64(fieldHeader.dataSize) else {
                let remains = r.baseStream.position - position - UInt64(fieldHeader.dataSize)
                fatalError("Failed reading \(header.type):\(fieldHeader.type) field data at offset \(position) in \(filePath) of \(remains)")
            }
        }
        // check full read
        guard r.baseStream.position == endPosition else {
            fatalError("Failed reading \(header.type) record data at offset \(startPosition) in \(filePath)")
        }
    }
}

public class FieldHeader: CustomStringConvertible {
    public var description: String { return type }
    public let type: String // 4 bytes
    public var dataSize: Int

    init(_ r: BinaryReader, for format: GameFormatId) {
        type = r.readASCIIString(4)
        dataSize = format == .TES3 ? Int(r.readLEUInt32()) : Int(r.readLEUInt16())
    }
}
