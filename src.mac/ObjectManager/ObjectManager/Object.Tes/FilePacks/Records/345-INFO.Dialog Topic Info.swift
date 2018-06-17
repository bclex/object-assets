//
//  INFORecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class INFORecord: Record {
    // TES3
    public struct DATA3Field {
        public let unknown1: Int32
        public let disposition: Int32
        public let rank: UInt8 // (0-10)
        public let gender: UInt8 // 0xFF = None, 0x00 = Male, 0x01 = Female
        public let pcRank: UInt8 // (0-10)
        public let unknown2: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            unknown1 = r.readLEInt32()
            disposition = r.readLEInt32()
            rank = r.readByte()
            gender = r.readByte()
            pcRank = r.readByte()
            unknown2 = r.readByte()
        }
    }

    public class TES3Group {
        public var NNAM: STRVField  // Next info ID (form a linked list of INFOs for the DIAL). First INFO has an empty PNAM, last has an empty NNAM.
        public var DATA: DATA3Field // Info data
        public var ONAM: STRVField  // Actor
        public var RNAM: STRVField  // Race
        public var CNAM: STRVField  // Class
        public var FNAM: STRVField  // Faction 
        public var ANAM: STRVField  // Cell
        public var DNAM: STRVField  // PC Faction
        public var NAME: STRVField  // The info response string (512 max)
        public var SNAM: FILEField  // Sound
        public var QSTN: BYTEField  // Journal Name
        public var QSTF: BYTEField  // Journal Finished
        public var QSTR: BYTEField  // Journal Restart
        public var SCVR: SCPTRecord.CTDAField // String for the function/variable choice
        public var INTV: UNKNField //
        public var FLTV: UNKNField // The function/variable result for the previous SCVR
        public var BNAM: STRVField // Result text (not compiled)
        
        init() {
        }
    }

    // TES4
    public struct DATA4Field {
        public let type: UInt8
        public let nextSpeaker: UInt8
        public let flags: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            type = r.readByte()
            nextSpeaker = r.readByte()
            flags = dataSize == 3 ? r.readByte() : 0
        }
    }

    public class TRDTField {
        public let emotionType: UInt32
        public let emotionValue: Int32
        public let responseNumber: UInt8
        public var responseText: String
        public var actorNotes: String

        init(_ r: BinaryReader, _ dataSize: Int) {
            emotionType = r.readLEUInt32()
            emotionValue = r.readLEInt32()
            r.skipBytes(4) // Unused
            responseNumber = r.readByte()
            r.skipBytes(3) // Unused
        }

        func NAM1Field(_ r: BinaryReader, _ dataSize: Int) {
            responseText = r.readASCIIString(dataSize, format: .possibleNullTerminated)
        }

        func NAM2Field(_ r: BinaryReader, _ dataSize: Int) {
            sctorNotes = r.readASCIIString(dataSize, format: .possibleNullTerminated)
        }
    }

    public class TES4Group {
        public var DATA: DATA4Field // Info data
        public var QSTI: FMIDField<QUSTRecord> // Quest
        public var TPIC: FMIDField<DIALRecord> // Topic
        public var NAMEs = [FMIDField<DIALRecord>]() // Topics
        public var RDTs = [TRDTField]() // Responses
        public var CTDAs = [SCPTRecord.CTDAField]() // Conditions
        public var TCLTs = [FMIDField<DIALRecord>]() // Choices
        public var TCLFs = [FMIDField<DIALRecord>]() // Link From Topics
        public var SCHR: SCPTRecord.SCHRField // Script Data
        public var SCDA: BYTVField // Compiled Script
        public var SCTX: STRVField // Script Source
        public var SCROs = [FMIDField<Record>]() // Global variable reference
        
        init() {
        }
    }

    public override var description: String { return "INFO: \(EDID!)" }
    public var EDID: STRVField! // Editor ID - Info name string (unique sequence of #'s), ID
    public var PNAM: FMIDField<INFORecord>! // Previous info ID
    public var TES3 = TES3Group()
    public var TES4 = TES4Group()

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "INAM": EDID = STRVField(r, dataSize); DIALRecord.LastRecord?.INFOs.append(self)
            case "PNAM": PNAM = FMIDField<INFORecord>(r, dataSize)
            case "NNAM": TES3.NNAM = STRVField(r, dataSize)
            case "DATA": TES3.DATA = DATA3Field(r, dataSize)
            case "ONAM": TES3.ONAM = STRVField(r, dataSize)
            case "RNAM": TES3.RNAM = STRVField(r, dataSize)
            case "CNAM": TES3.CNAM = STRVField(r, dataSize)
            case "FNAM": TES3.FNAM = STRVField(r, dataSize)
            case "ANAM": TES3.ANAM = STRVField(r, dataSize)
            case "DNAM": TES3.DNAM = STRVField(r, dataSize)
            case "NAME": TES3.NAME = STRVField(r, dataSize)
            case "SNAM": TES3.SNAM = FILEField(r, dataSize)
            case "QSTN": TES3.QSTN = BYTEField(r, dataSize)
            case "QSTF": TES3.QSTF = BYTEField(r, dataSize)
            case "QSTR": TES3.QSTR = BYTEField(r, dataSize)
            case "SCVR": TES3.SCVR = SCPTRecord.CTDAField(r, dataSize, format)
            case "INTV": TES3.INTV = UNKNField(r, dataSize)
            case "FLTV": TES3.FLTV = UNKNField(r, dataSize)
            case "BNAM": TES3.BNAM = STRVField(r, dataSize)
            default: return false
            }
            return true
        }
        switch type {
        case "DATA": TES4.DATA = DATA4Field(r, dataSize)
        case "QSTI": TES4.QSTI = FMIDField<QUSTRecord>(r, dataSize)
        case "TPIC": TES4.TPIC = FMIDField<DIALRecord>(r, dataSize)
        case "NAME": TES4.NAMEs.append(FMIDField<DIALRecord>(r, dataSize))
        case "TRDT": TES4.TRDTs.append(TRDTField(r, dataSize))
        case "NAM1": TES4.TRDTs.last!.NAM1Field(r, dataSize)
        case "NAM2": TES4.TRDTs.last!.NAM2Field(r, dataSize)
        case "CTDA",
             "CTDT": TES4.CTDAs.append(SCPTRecord.CTDAField(r, dataSize, format))
        case "TCLT": TES4.TCLTs.append(FMIDField<DIALRecord>(r, dataSize))
        case "TCLF": TES4.TCLFs.append(FMIDField<DIALRecord>(r, dataSize))
        case "SCHR",
             "SCHD": TES4.SCHR = SCPTRecord.SCHRField(r, dataSize)
        case "SCDA": TES4.SCDA = BYTVField(r, dataSize)
        case "SCTX": TES4.SCTX = STRVField(r, dataSize)
        case "SCRO": TES4.SCROs.append(FMIDField<Record>(r, dataSize))
        default: return false
        }
        return true
    }
}
