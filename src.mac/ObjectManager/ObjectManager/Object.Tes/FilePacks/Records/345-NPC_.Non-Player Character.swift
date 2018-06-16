//
//  NPC_Record.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class NPC_Record: Record, IHaveEDID, IHaveMODL {
    public struct NPC_Flags: OptionSet {
        let rawValue: UInt32
        public static let female = NPC_Flags(rawValue: 0x0001)
        public static let essential = NPC_Flags(rawValue: 0x0002)
        public static let respawn = NPC_Flags(rawValue: 0x0004)
        public static let none = NPC_Flags(rawValue: 0x0008)
        public static let autocalc = NPC_Flags(rawValue: 0x0010)
        public static let bloodSkel = NPC_Flags(rawValue: 0x0400)
        public static let bloodMetal = NPC_Flags(rawValue: 0x0800)
    }

    public class NPDTField {
        public let level: Int16
        public let strength: UInt8
        public let intelligence: UInt8
        public let willpower: UInt8
        public let agility: UInt8
        public let speed: UInt8
        public let endurance: UInt8
        public let personality: UInt8
        public let luck: UInt8
        public let skills: [UInt8]
        public let reputation: UInt8
        public let health: Int16
        public let spellPts: Int16
        public let fatigue: Int16
        public let disposition: UInt8
        public let factionId: UInt8
        public let rank: UInt8
        public let unknown1: UInt8
        public let gold: Int32

        // 12 byte version
        //public short Level;
        //public byte Disposition;
        //public byte FactionId;
        //public byte Rank;
        //public byte Unknown1;
        public let unknown2: UInt8
        public let unknown3: UInt8
        //public int Gold;

        init(_ r: BinaryReader, _ dataSize: Int) {
            if dataSize == 52 {
                level = r.readLEInt16()
                strength = r.readByte()
                intelligence = r.readByte()
                willpower = r.readByte()
                agility = r.readByte()
                speed = r.readByte()
                endurance = r.readByte()
                personality = r.readByte()
                luck = r.readByte()
                skills = r.readBytes(27)
                reputation = r.readByte()
                health = r.readLEInt16()
                spellPts = r.readLEInt16()
                fatigue = r.readLEInt16()
                disposition = r.readByte()
                factionId = r.readByte()
                rank = r.readByte()
                unknown1 = r.readByte()
                gold = r.readLEInt32()
            }
            else {
                level = r.readLEInt16()
                disposition = r.readByte()
                factionId = r.readByte()
                rank = r.readByte()
                unknown1 = r.readByte()
                unknown2 = r.readByte()
                unknown3 = r.readByte()
                gold = r.readLEInt32()
            }
        }
    }

    public struct DODTField {
        public let xpos: Float
        public let ypos: Float
        public let zpos: Float
        public let xrot: Float
        public let yrot: Float
        public let zrot: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            xpos = r.readLESingle()
            ypos = r.readLESingle()
            zpos = r.readLESingle()
            xrot = r.readLESingle()
            yrot = r.readLESingle()
            zrot = r.readLESingle()
        }
    }

    public var description: String { return "NPC_: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var FULL: STRVField // NPC name
    public var MODL: MODLGroup  // Animation
    public var RNAM: STRVField // Race Name
    public var ANAM: STRVField // Faction name
    public var BNAM: STRVField // Head model
    public var CNAM: STRVField // Class name
    public var KNAM: STRVField // Hair model
    public var NPDT: NPDTField // NPC Data
    public var FLAG: INTVField // NPC Flags
    public var NPCOs = [CNTOField]() // NPC item
    public var NPCSs = [STRVField]() // NPC spell
    public var AIDT: CREARecord.AIDTField  // AI data
    public var AI_W: CREARecord.AI_WField? // AI
    public var AI_T: CREARecord.AI_TField? // AI Travel
    public var AI_F: CREARecord.AI_FField? // AI Follow
    public var AI_E: CREARecord.AI_FField? // AI Escort
    public var CNDT: STRVField? // Cell escort/follow to string (optional)
    public var AI_A: CREARecord.AI_AField? // AI Activate
    public var DODT: DODTField // Cell Travel Destination
    public var DNAM: STRVField // Cell name for previous DODT, if interior
    public var XSCL: FLTVField? // Scale (optional) Only present if the scale is not 1.0
    public var SCRI: FMIDField<SCPTRecord>? // Unknown

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "RNAM": RNAM = STRVField(r, dataSize)
        case "ANAM": ANAM = STRVField(r, dataSize)
        case "BNAM": BNAM = STRVField(r, dataSize)
        case "CNAM": CNAM = STRVField(r, dataSize)
        case "KNAM": KNAM = STRVField(r, dataSize)
        case "NPDT": NPDT = NPDTField(r, dataSize)
        case "FLAG": FLAG = INTVField(r, dataSize)
        case "NPCO": NPCOs.append(CNTOField(r, dataSize, format))
        case "NPCS": NPCSs.append(STRVField(r, dataSize, .zeroPadded))
        case "AIDT": AIDT = CREARecord.AIDTField(r, dataSize)
        case "AI_W": AI_W = CREARecord.AI_WField(r, dataSize, 1)
        case "AI_T": AI_T = CREARecord.AI_TField(r, dataSize)
        case "AI_F": AI_F = CREARecord.AI_FField(r, dataSize)
        case "AI_E": AI_E = CREARecord.AI_FField(r, dataSize)
        case "CNDT": CNDT = STRVField(r, dataSize)
        case "AI_A": AI_A = CREARecord.AI_AField(r, dataSize)
        case "DODT": DODT = DODTField(r, dataSize)
        case "DNAM": DNAM = STRVField(r, dataSize)
        case "XSCL": XSCL = FLTVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
