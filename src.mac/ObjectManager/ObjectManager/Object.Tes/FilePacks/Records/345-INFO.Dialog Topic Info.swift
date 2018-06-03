//
//  INFORecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class INFORecord: Record {
    // TES3
    public struct DATA3Field
    {
        public int Unknown1;
        public int Disposition;
        public byte Rank // (0-10)
        public byte Gender // 0xFF = None, 0x00 = Male, 0x01 = Female
        public byte PCRank // (0-10)
        public byte Unknown2;

        public DATA3Field(UnityBinaryReader r, uint dataSize)
        {
            Unknown1 = r.readLEInt32();
            Disposition = r.readLEInt32();
            Rank = r.readByte();
            Gender = r.readByte();
            PCRank = r.readByte();
            Unknown2 = r.readByte();
        }
    }

    public class TES3Group
    {
        public STRVField NNAM // Next info ID (form a linked list of INFOs for the DIAL). First INFO has an empty PNAM, last has an empty NNAM.
        public DATA3Field DATA // Info data
        public STRVField ONAM // Actor
        public STRVField RNAM // Race
        public STRVField CNAM // Class
        public STRVField FNAM // Faction 
        public STRVField ANAM // Cell
        public STRVField DNAM // PC Faction
        public STRVField NAME // The info response string (512 max)
        public FILEField SNAM // Sound
        public BYTEField QSTN // Journal Name
        public BYTEField QSTF // Journal Finished
        public BYTEField QSTR // Journal Restart
        public SCPTRecord.CTDAField SCVR // String for the function/variable choice
        public UNKNField INTV //
        public UNKNField FLTV // The function/variable result for the previous SCVR
        public STRVField BNAM // Result text (not compiled)
    }

    // TES4
    public struct DATA4Field
    {
        public byte Type;
        public byte NextSpeaker;
        public byte Flags;

        public DATA4Field(UnityBinaryReader r, uint dataSize)
        {
            Type = r.readByte();
            NextSpeaker = r.readByte();
            Flags = dataSize == 3 ? r.readByte() : (byte)0;
        }
    }

    public class TRDTField
    {
        public uint EmotionType;
        public int EmotionValue;
        public byte ResponseNumber;
        public string ResponseText;
        public string ActorNotes;

        public TRDTField(UnityBinaryReader r, uint dataSize)
        {
            EmotionType = r.readLEUInt32();
            EmotionValue = r.readLEInt32();
            r.skipBytes(4) // Unused
            ResponseNumber = r.readByte();
            r.skipBytes(3) // Unused
        }

        public void NAM1Field(UnityBinaryReader r, uint dataSize)
        {
            ResponseText = r.readASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }

        public void NAM2Field(UnityBinaryReader r, uint dataSize)
        {
            ActorNotes = r.readASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }
    }

    public class TES4Group
    {
        public var DATA: DATA4Field // Info data
        public var QSTI: FMIDField<QUSTRecord> // Quest
        public var TPIC: FMIDField<DIALRecord> // Topic
        public var NAMEs = [FMIDField<DIALRecord>]() // Topics
        public var RDTs = [TRDTField]() // Responses
        public var CTDAs = [SCPTRecord.CTDAField>]() // Conditions
        public var TCLTs = [FMIDField<DIALRecord>]() // Choices
        public var TCLFs = [FMIDField<DIALRecord] // Link From Topics
        public var SCHR: SCPTRecord.SCHRField // Script Data
        public var SCDA: BYTVField // Compiled Script
        public var SCTX: STRVField // Script Source
        public var SCROs = [FMIDField<Record>]() // Global variable reference
    }

    public var description: String { return "INFO: \(EDID)" }
    public var EDID: STRVField // Editor ID - Info name string (unique sequence of #'s), ID
    public var PNAM: FMIDField<INFORecord> // Previous info ID
    public var TES3 = TES3Group()
    public var TES4 = TES4Group()

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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
