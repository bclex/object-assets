//
//  EsmRecords.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

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
    public let type: String // 4 bytes
    public var dataSize: UInt32
    public let flags: Flags
    public var compressed: Bool { return flags.contains(.compressed) }
    public let formId: UInt32
    public var position: UInt64
    // group
    public let label: String?
    public let groupType: GroupType?

    init(label: String, dataSize: UInt32, position: UInt64) {
        type = ""
        self.dataSize = dataSize
        flags = Flags(rawValue: 0)
        formId = 0
        self.position = position
        self.label = label
        groupType = nil
    }
    init(_ r: BinaryReader, for format: GameFormatId) {
        type = r.readASCIIString(4)
        if type == "GRUP" {
            dataSize = r.readLEUInt32() - (format == .TES4 ? 20 : 24)
            label = r.readASCIIString(4)
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

    static let create:[String : (f: (Header) -> Record, l: (Int) -> Bool)] = [
        "TES3" : ({x in return TES3Record(x)}, {x in return true})
         /*
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
        "GMST" : ({return GMSTRecord()}, {x in return x > 2}),
        "GLOB" : ({return GLOBRecord()}, {x in return x > 2}),
        "SOUN" : ({return SOUNRecord()}, {x in return x > 2}),
        "REGN" : ({return REGNRecord()}, {x in return x > 2}),
        // 3
        "CLAS" : ({return CLASRecord()}, {x in return x > 3}),
        "SPEL" : ({return SPELRecord()}, {x in return x > 3}),
        "BODY" : ({return BODYRecord()}, {x in return x > 3}),
        "PGRD" : ({return PGRDRecord()}, {x in return x > 3}),
        "INFO" : ({return INFORecord()}, {x in return x > 3}),
        "DIAL" : ({return DIALRecord()}, {x in return x > 3}),
        "SNDG" : ({return SNDGRecord()}, {x in return x > 3}),
        "ENCH" : ({return ENCHRecord()}, {x in return x > 3}),
        "SCPT" : ({return SCPTRecord()}, {x in return x > 3}),
        "SKIL" : ({return SKILRecord()}, {x in return x > 3}),
        "RACE" : ({return RACERecord()}, {x in return x > 3}),
        "MGEF" : ({return MGEFRecord()}, {x in return x > 3}),
        "LEVI" : ({return LEVIRecord()}, {x in return x > 3}),
        "LEVC" : ({return LEVCRecord()}, {x in return x > 3}),
        "BSGN" : ({return BSGNRecord()}, {x in return x > 3}),
        "FACT" : ({return FACTRecord()}, {x in return x > 3}),
        "SSCR" : ({return SSCRRecord()}, {x in return x > 3}),
        // 4 - Oblivion
        "ACRE" : ({return ACRERecord()}, {x in return x > 4}),
        "ACHR" : ({return ACHRRecord()}, {x in return x > 4}),
        "AMMO" : ({return AMMORecord()}, {x in return x > 4}),
        "ANIO" : ({return ANIORecord()}, {x in return x > 4}),
        "CLMT" : ({return CLMTRecord()}, {x in return x > 4}),
        "CSTY" : ({return CSTYRecord()}, {x in return x > 4}),
        "EFSH" : ({return EFSHRecord()}, {x in return x > 4}),
        "EYES" : ({return EYESRecord()}, {x in return x > 4}),
        "FLOR" : ({return FLORRecord()}, {x in return x > 4}),
        "FURN" : ({return FURNRecord()}, {x in return x > 4}),
        "GRAS" : ({return GRASRecord()}, {x in return x > 4}),
        "HAIR" : ({return HAIRRecord()}, {x in return x > 4}),
        "IDLE" : ({return IDLERecord()}, {x in return x > 4}),
        "KEYM" : ({return KEYMRecord()}, {x in return x > 4}),
        "LSCR" : ({return LSCRRecord()}, {x in return x > 4}),
        "LVLC" : ({return LVLCRecord()}, {x in return x > 4}),
        "LVLI" : ({return LVLIRecord()}, {x in return x > 4}),
        "LVSP" : ({return LVSPRecord()}, {x in return x > 4}),
        "PACK" : ({return PACKRecord()}, {x in return x > 4}),
        "QUST" : ({return QUSTRecord()}, {x in return x > 4}),
        "REFR" : ({return REFRRecord()}, {x in return x > 4}),
        "ROAD" : ({return ROADRecord()}, {x in return x > 4}),
        "SBSP" : ({return SBSPRecord()}, {x in return x > 4}),
        "SGST" : ({return SGSTRecord()}, {x in return x > 4}),
        "SLGM" : ({return SLGMRecord()}, {x in return x > 4}),
        "TREE" : ({return TREERecord()}, {x in return x > 4}),
        "WATR" : ({return WATRRecord()}, {x in return x > 4}),
        "WRLD" : ({return WRLDRecord()}, {x in return x > 4}),
        "WTHR" : ({return WTHRRecord()}, {x in return x > 4}),
        // 5 - Skyrim
        "AACT" : ({return AACTRecord()}, {x in return x > 5}),
        "ADDN" : ({return ADDNRecord()}, {x in return x > 5}),
        "ARMA" : ({return ARMARecord()}, {x in return x > 5}),
        "ARTO" : ({return ARTORecord()}, {x in return x > 5}),
        "ASPC" : ({return ASPCRecord()}, {x in return x > 5}),
        "ASTP" : ({return ASTPRecord()}, {x in return x > 5}),
        "AVIF" : ({return AVIFRecord()}, {x in return x > 5}),
        "DLBR" : ({return DLBRRecord()}, {x in return x > 5}),
        "DLVW" : ({return DLVWRecord()}, {x in return x > 5}),
        "SNDR" : ({return SNDRRecord()}, {x in return x > 5}),
        */
    ]

    // public static bool LoadRecord(string type, byte level) => Create.TryGetValue(type, out RecordType recordType) ? recordType.Load(level) : false;

    func createRecord(at position: UInt64) -> Record? {
        guard let recordType = Header.create[type] else {
            debugPrint("Unsupported ESM record type: \(type)")
            return nil
        }
        let record = recordType.f(self)
        record.header = self;
        return record;
    }
}

public class RecordGroup: CustomStringConvertible {
    public var description: String { return "\(headers.first?.description ?? "none")" }
    //public string Label => Headers.First.Value.Label
    public var headers = [Header]()
    public var records = [Record]()
    public var groups: [RecordGroup]?
    let _r: BinaryReader
    let _filePath: String
    let _format: GameFormatId
    let _level: Int
    var _headerSkip = 0

    init(_ r: BinaryReader, _ filePath: String, for format: GameFormatId, level: Int) {
        _r = r
        _filePath = filePath
        _format = format
        _level = level
    }

    func addHeader(_ header: Header, mode: Int = 0) {
        headers.append(header)
        let grup = _r.readASCIIString(4)
        _r.baseStream.position -= 4
        guard grup != "GRUP" else {
            return
        }
        let recordHeader = Header(_r, for: _format)
        readGrup(header, recordHeader)
    }

    func readGrup(_ header: Header, _ recordHeader: Header, mode: Int = 0) {
        let nextPosition = _r.baseStream.position + UInt64(recordHeader.dataSize)
        if groups == nil {
            groups = [RecordGroup]()
        }
        let group = RecordGroup(_r, _filePath, for: _format, level: _level)
        group.addHeader(recordHeader)
        groups!.append(group); debugPrint("Grup: \(header.label ?? "unk")/\(group)")
        _r.baseStream.position = nextPosition
    }

    public func load() {
        guard _headerSkip != headers.count else {
            return
        }
        for i in _headerSkip..<headers.endIndex {
            readGroup(headers[i])
        }
        _headerSkip = headers.count
    }

    func readGroup(_ header: Header) {
        _r.baseStream.position = header.position
        let endPosition = header.position + UInt64(header.dataSize)
        while _r.baseStream.position < endPosition {
            let recordHeader = Header(_r, for: _format)
            guard recordHeader.type != "GRUP" else {
                readGrup(header, recordHeader)
                //group.load()
                continue
            }
            guard let record = recordHeader.createRecord(at: _r.baseStream.position) else {
                _r.baseStream.position += UInt64(recordHeader.dataSize)
                continue
            }
            readRecord(record, compressed: recordHeader.compressed)
            records.append(record)
        }
    }

    func readRecord(_ record: Record, compressed: Bool) {
        guard compressed else {
            record.read(_r, _filePath, for: _format)
            return
        }
        let newDataSize = _r.readLEUInt32()
        let data = _r.readBytes(Int(record.header.dataSize - 4))
        let newData = data.inflate()!
        // read record
        record.header.position = 0
        record.header.dataSize = newDataSize
        let r = BinaryReader(DataBaseStream(data: newData))
        defer { r.close() }
        record.read(r, _filePath, for: _format)
    }
}

public class Record: IRecord, CustomStringConvertible {
    public var description: String { return "BASE" }
    var header: Header
    
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
                //header.DataSize = UInt(endPosition - r.baseStream.position)
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
