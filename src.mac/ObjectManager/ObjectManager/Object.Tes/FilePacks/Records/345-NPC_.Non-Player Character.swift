//
//  NPC_Record.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class NPC_Record: Record, IHaveEDID, IHaveMODL {
    public struct NPC_Flags: OptionSet {
        public let rawValue: UInt32
        public static let female = NPC_Flags(rawValue: 0x0001)
        public static let essential = NPC_Flags(rawValue: 0x0002)
        public static let respawn = NPC_Flags(rawValue: 0x0004)
        public static let none = NPC_Flags(rawValue: 0x0008)
        public static let autocalc = NPC_Flags(rawValue: 0x0010)
        public static let bloodSkel = NPC_Flags(rawValue: 0x0400)
        public static let bloodMetal = NPC_Flags(rawValue: 0x0800)
        
        public init(rawValue: UInt32) {
            self.rawValue = rawValue
        }
    }

    public class NPDTField {
        public let level: Int16
        public var strength: UInt8 = 0
        public var intelligence: UInt8 = 0
        public var willpower: UInt8 = 0
        public var agility: UInt8 = 0
        public var speed: UInt8 = 0
        public var endurance: UInt8 = 0
        public var personality: UInt8 = 0
        public var luck: UInt8 = 0
        public var skills: Data = Data()
        public var reputation: UInt8 = 0
        public var health: Int16 = 0
        public var spellPts: Int16 = 0
        public var fatigue: Int16 = 0
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
        public var unknown2: UInt8 = 0
        public var unknown3: UInt8 = 0
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

    public override var description: String { return "NPC_: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var FULL: STRVField! // NPC name
    public var MODL: MODLGroup? = nil // Animation
    public var RNAM: STRVField! // Race Name
    public var ANAM: STRVField! // Faction name
    public var BNAM: STRVField! // Head model
    public var CNAM: STRVField! // Class name
    public var KNAM: STRVField! // Hair model
    public var NPDT: NPDTField! // NPC Data
    public var FLAG: INTVField! // NPC Flags
    public var NPCOs = [CNTOField]() // NPC item
    public var NPCSs = [STRVField]() // NPC spell
    public var AIDT: CREARecord.AIDTField!  // AI data
    public var AI_W: CREARecord.AI_WField? = nil // AI
    public var AI_T: CREARecord.AI_TField? = nil // AI Travel
    public var AI_F: CREARecord.AI_FField? = nil // AI Follow
    public var AI_E: CREARecord.AI_FField? = nil // AI Escort
    public var CNDT: STRVField? = nil // Cell escort/follow to string (optional)
    public var AI_A: CREARecord.AI_AField? = nil // AI Activate
    public var DODT: DODTField! // Cell Travel Destination
    public var DNAM: STRVField! // Cell name for previous DODT, if interior
    public var XSCL: FLTVField? = nil // Scale (optional) Only present if the scale is not 1.0
    public var SCRI: FMIDField<SCPTRecord>? = nil // Unknown

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "FULL",
             "FNAM": FULL = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "RNAM": RNAM = r.readSTRV(dataSize)
        case "ANAM": ANAM = r.readSTRV(dataSize)
        case "BNAM": BNAM = r.readSTRV(dataSize)
        case "CNAM": CNAM = r.readSTRV(dataSize)
        case "KNAM": KNAM = r.readSTRV(dataSize)
        case "NPDT": NPDT = NPDTField(r, dataSize)
        case "FLAG": FLAG = r.readINTV(dataSize)
        case "NPCO": NPCOs.append(CNTOField(r, dataSize, for: format))
        case "NPCS": NPCSs.append(r.readSTRV(dataSize, format: .zeroPadded))
        case "AIDT": AIDT = CREARecord.AIDTField(r, dataSize)
        case "AI_W": AI_W = CREARecord.AI_WField(r, dataSize)
        case "AI_T": AI_T = CREARecord.AI_TField(r, dataSize)
        case "AI_F": AI_F = CREARecord.AI_FField(r, dataSize)
        case "AI_E": AI_E = CREARecord.AI_FField(r, dataSize)
        case "CNDT": CNDT = r.readSTRV(dataSize)
        case "AI_A": AI_A = CREARecord.AI_AField(r, dataSize)
        case "DODT": DODT = DODTField(r, dataSize)
        case "DNAM": DNAM = r.readSTRV(dataSize)
        case "XSCL": XSCL = r.readT(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
