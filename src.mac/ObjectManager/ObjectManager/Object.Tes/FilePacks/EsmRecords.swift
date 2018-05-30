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
    public enum Flags: UInt32 {
        case esmFile = 0x00000001               // ESM file. (TES4.HEDR record only.)
        case deleted = 0x00000020               // Deleted
        case r00 = 0x00000040                   // Constant / (REFR) Hidden From Local Map (Needs Confirmation: Related to shields)
        case r01 = 0x00000100                   // Must Update Anims / (REFR) Inaccessible
        case r02 = 0x00000200                   // (REFR) Hidden from local map / (ACHR) Starts dead / (REFR) MotionBlurCastsShadows
        case r03 = 0x00000400                   // Quest item / Persistent reference / (LSCR) Displays in Main Menu
        case initiallyDisabled = 0x00000800     // Initially disabled
        case ignored = 0x00001000               // Ignored
        case visibleWhenDistant = 0x00008000    // Visible when distant
        case r04 = 0x00010000                   // (ACTI) Random Animation Start
        case r05 = 0x00020000                   // (ACTI) Dangerous / Off limits (Interior cell) Dangerous Can't be set withough Ignore Object Interaction
        case compressed = 0x00040000            // Data is compressed
        case cantWait = 0x00080000              // Can't wait
                                                // tes5
        case r06 = 0x00100000                   // (ACTI) Ignore Object Interaction Ignore Object Interaction Sets Dangerous Automatically
        case isMarker = 0x00800000              // Is Marker
        case r07 = 0x02000000                   // (ACTI) Obstacle / (REFR) No AI Acquire
        case navMesh01 = 0x04000000             // NavMesh Gen - Filter
        case navMesh02 = 0x08000000             // NavMesh Gen - Bounding Box
        case r08 = 0x10000000                   // (FURN) Must Exit to Talk / (REFR) Reflected By Auto Water
        case r09 = 0x20000000                   // (FURN/IDLM) Child Can Use / (REFR) Don't Havok Settle
        case r10 = 0x40000000                   // NavMesh Gen - Ground / (REFR) NoRespawn
        case r11 = 0x80000000                   // (REFR) MultiBound
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

    public var description: String { return "\(type):\(groupType)" }
    public let type: String // 4 bytes
    public let dataSize: UInt32
    public let flags: Flags
    public var compressed: Bool { return (flags.rawValue & Flags.compressed.rawValue) != 0 }
    public let formId: UInt32
    public let position: UInt64
    // group
    public let label: String
    public let groupType: GroupType

    init() { }
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
            position = r.baseStream.offsetInFile;
            return;
        }
        dataSize = r.readLEUInt32()
        if format == .TES3 {
            _ = r.readLEUInt32() // Unknown
        }
        flags = Flags(rawValue: r.readLEUInt32())!
        if format == .TES3 {
            position = r.baseStream.offsetInFile
            return
        }
        // tes4
        formId = r.readLEUInt32()
        _ = r.readLEUInt32()
        if format == .TES4 {
            position = r.baseStream.offsetInFile
            return
        }
        // tes5
        _ = r.readLEUInt32()
        position = r.baseStream.offsetInFile
    }

    static let create:[String : (f: ()->Record, l: (Int)->Bool)] = [
        "TES3" : ({return TES3Record()}, {x in return true})
         /*
        "TES4" : ({return TES4Record()}, {x in return true}),
        // 0
        "LTEX" : ({return LTEXRecord()}, {x in return x > 0}),
        "STAT" : ({return STATRecord()}, {x in return x > 0}),
        "CELL" : ({return CELLRecord()}, {x in return x > 0}),
        "LAND" : ({return LANDRecord()}, {x in return x > 0}),
        // 1
        "DOOR" : ({return DOORRecord()}, {x in return x > 1}),
        "MISC" : ({return MISCRecord()}, {x in return x > 1}),
        "WEAP" : ({return WEAPRecord()}, {x in return x > 1}),
        "CONT" : ({return CONTRecord()}, {x in return x > 1}),
        "LIGH" : ({return LIGHRecord()}, {x in return x > 1}),
        "ARMO" : ({return ARMORecord()}, {x in return x > 1}),
        "CLOT" : ({return CLOTRecord()}, {x in return x > 1}),
        "REPA" : ({return REPARecord()}, {x in return x > 1}),
        "ACTI" : ({return ACTIRecord()}, {x in return x > 1}),
        "APPA" : ({return APPARecord()}, {x in return x > 1}),
        "LOCK" : ({return LOCKRecord()}, {x in return x > 1}),
        "PROB" : ({return PROBRecord()}, {x in return x > 1}),
        "INGR" : ({return INGRRecord()}, {x in return x > 1}),
        "BOOK" : ({return BOOKRecord()}, {x in return x > 1}),
        "ALCH" : ({return ALCHRecord()}, {x in return x > 1}),
        "CREA" : ({return CREARecord()}, {x in return x > 1}), // && BaseSettings.Game.CreaturesEnabled}),
        "NPC_" : ({return NPC_Record()}, {x in return x > 1}), // && BaseSettings.Game.NpcsEnabled}),
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

    func createRecord(at position: UInt64) -> Record {
        guard let recordType = Header.create[type] else {
            fatalError("Unsupported ESM record type: \(type)")
        }
        let record = recordType.f()
        record.header = self;
        return record;
    }
}

public class RecordGroup: CustomStringConvertible {
    public var description: String { return "\(headers.first ?? "none")" }
    //public string Label => Headers.First.Value.Label
    public var headers = [Header]()
    public var records = [Record]()
    public var groups: [RecordGroup]?
    let _r: BinaryReader
    let _filePath: String
    let _formatId: GameFormatId
    let _level: UInt8
    var _headerSkip = 0

    init(_ r: BinaryReader, filePath: String, for format: GameFormatId, level: Int) {
        _r = r
        _filePath = filePath
        _formatId = formatId
        _level = level
    }

    func add(header: Header, mode: Int = 0) {
        headers.append(header)
        var grup = _r.readASCIIString(4)
        _r.baseStream.offsetInFile -= 4
        guard grup != "GRUP" else {
            return
        }
        let recordHeader = Header(_r, forFormat: _formatId)
        readGrup(header, recordHeader)
    }

    func readGrup(_ header: Header, _ recordHeader: Header, mode: Int = 0) {
        let nextPosition = _r.baseStream.offsetInFile + recordHeader.dataSize
        if groups == nil {
            groups = [RecordGroup]()
        }
        let group = RecordGroup(_r, _filePath, forFormat: _formatId, level: _level)
        group.add(header: recordHeader)
        groups.append(group) ; debugPrint("Grup: \(header.Label)/\(group)")
        _r.baseStream.offsetInFile = nextPosition
    }

    public func load() {
        guard _headerSkip != headers.count else {
            return
        }
        for header in _headerSkip..<headers.endIndex {
            readGroup(header)
        }
        _headerSkip = headers.count
    }

    func readGroup(_ header: Header) {
        _r.baseStream.offsetInFile = header.position
        let endPosition = header.position + header.dataSize
        while _r.baseStream.offsetInFile < endPosition {
            let recordHeader = Header(_r, for: _formatId)
            guard recordHeader.type != "GRUP" else {
                readGrup(header, recordHeader)
                //group.load()
                continue
            }
            guard record = recordHeader.createRecord(at: _r.baseStream.offsetInFile) else {
                _r.baseStream.offsetInFile += recordHeader.dataSize
                continue
            }
            readRecord(record, compressed: recordHeader.compressed)
            records.append(record)
        }
    }

    func readRecord(_ record: Record, compressed: Bool) {
        guard compressed else {
            record.read(_r, _filePath, _formatId)
            return
        }
        let newDataSize = _r.readLEUInt32()
        let data = _r.readBytes(Int(record.header.dataSize - 4))
        let newData = data.inflate()
        // read record
        record.header.Position = 0
        record.header.dataSize = newDataSize
        let r = BinaryReader(s)
        defer { r.close() }
        record.read(r, _filePath, _formatId)
    }
}

public class Record: IRecord, CustomStringConvertible {
    public var description: String { return "BASE" }
    internal var header: Header

    func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) {
    }

    func read(_ r: BinaryReader, filePath: String, for format: GameFormatId) {
        let startPosition = r.baseStream.Position
        let endPosition = startPosition + header.dataSize
        while r.baseStream.offsetInFile < endPosition {
            let fieldHeader = FieldHeader(r, for: formatId)
            guard fieldHeader.type != "XXXX") else {
                if fieldHeader.dataSize != 4 {
                    fatalError("")
                }
                fieldHeader.dataSize = Int(r.readLEUInt32())
                continue
            }
            guard fieldHeader.type != "OFST" || header.type != "WRLD" else {
                r.baseStream.offsetInFile = endPosition
                continue
                //header.DataSize = UInt(endPosition - r.baseStream.offsetInFile)
            }
            let position = r.baseStream.offsetInFile
            guard createField(r, for: formatId, type: fieldHeader.type, dataSize: fieldHeader.dataSize)) else {
                fatalError("Unsupported ESM record type: \(header.type):\(fieldHeader.type)")
                r.baseStream.offsetInFile += fieldHeader.dataSize
                continue
            }
            // check full read
            guard r.baseStream.offsetInFile == position + fieldHeader.dataSize else {
                fatalError("Failed reading \(header.type):\(fieldHeader.type) field data at offset \(position) in \(filePath) of \(r.baseStream.offsetInFile - position - fieldHeader.dataSize)")
            }
        }
        // check full read
        guard r.baseStream.offsetInFile == endPosition else {
            fatalError("Failed reading \(header.type) record data at offset \(startPosition) in \(filePath)")
        }
    }
}

public class FieldHeader, CustomStringConvertible {
    public var description: String { return type }
    public let type: String // 4 bytes
    public let dataSize: Int

    init(_ r: BinaryReader, for format: GameFormatId) {
        type = r.readASCIIString(4)
        dataSize = Int(format == .TES3 ? r.readLEUInt32() : r.readLEUInt16())
    }
}
