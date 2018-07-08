//
//  CREARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class CREARecord: Record, IHaveEDID, IHaveMODL {
    public struct CREAFlags: OptionSet {
        public let rawValue: UInt32
        public static let biped = CREAFlags(rawValue: 0x0001)
        public static let respawn = CREAFlags(rawValue: 0x0002)
        public static let weaponAndShield = CREAFlags(rawValue: 0x0004)
        public static let none = CREAFlags(rawValue: 0x0008)
        public static let swims = CREAFlags(rawValue: 0x0010)
        public static let flies = CREAFlags(rawValue: 0x0020)
        public static let walks = CREAFlags(rawValue: 0x0040)
        public static let defaultFlags = CREAFlags(rawValue: 0x0048)
        public static let essential = CREAFlags(rawValue: 0x0080)
        public static let skeletonBlood = CREAFlags(rawValue: 0x0400)
        public static let metalBlood = CREAFlags(rawValue: 0x0800)
        
        public init(rawValue: UInt32) {
            self.rawValue = rawValue
        }
    }

    public struct NPDTField {
        public let type: Int32 // 0 = Creature, 1 = Daedra, 2 = Undead, 3 = Humanoid
        public let level: Int32
        public let strength: Int32
        public let intelligence: Int32
        public let willpower: Int32
        public let agility: Int32
        public let speed: Int32
        public let endurance: Int32
        public let personality: Int32
        public let luck: Int32
        public let health: Int32
        public let spellPts: Int32
        public let fatigue: Int32
        public let soul: Int32
        public let combat: Int32
        public let magic: Int32
        public let stealth: Int32
        public let attackMin1: Int32
        public let attackMax1: Int32
        public let attackMin2: Int32
        public let attackMax2: Int32
        public let attackMin3: Int32
        public let attackMax3: Int32
        public let gold: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            type = r.readLEInt32()
            level = r.readLEInt32()
            strength = r.readLEInt32()
            intelligence = r.readLEInt32()
            willpower = r.readLEInt32()
            agility = r.readLEInt32()
            speed = r.readLEInt32()
            endurance = r.readLEInt32()
            personality = r.readLEInt32()
            luck = r.readLEInt32()
            health = r.readLEInt32()
            spellPts = r.readLEInt32()
            fatigue = r.readLEInt32()
            soul = r.readLEInt32()
            combat = r.readLEInt32()
            magic = r.readLEInt32()
            stealth = r.readLEInt32()
            attackMin1 = r.readLEInt32()
            attackMax1 = r.readLEInt32()
            attackMin2 = r.readLEInt32()
            attackMax2 = r.readLEInt32()
            attackMin3 = r.readLEInt32()
            attackMax3 = r.readLEInt32()
            gold = r.readLEInt32()
        }
    }

    public struct AIDTField {
        public struct AIFlags: OptionSet {
            public let rawValue: UInt32
            public static let weapon = AIFlags(rawValue: 0x00001)
            public static let armor = AIFlags(rawValue: 0x00002)
            public static let clothing = AIFlags(rawValue: 0x00004)
            public static let books = AIFlags(rawValue: 0x00008)
            public static let ingrediant = AIFlags(rawValue: 0x00010)
            public static let picks = AIFlags(rawValue: 0x00020)
            public static let probes = AIFlags(rawValue: 0x00040)
            public static let lights = AIFlags(rawValue: 0x00080)
            public static let apparatus = AIFlags(rawValue: 0x00100)
            public static let repair = AIFlags(rawValue: 0x00200)
            public static let misc = AIFlags(rawValue: 0x00400)
            public static let spells = AIFlags(rawValue: 0x00800)
            public static let magicItems = AIFlags(rawValue: 0x01000)
            public static let potions = AIFlags(rawValue: 0x02000)
            public static let training = AIFlags(rawValue: 0x04000)
            public static let spellmaking = AIFlags(rawValue: 0x08000)
            public static let enchanting = AIFlags(rawValue: 0x10000)
            public static let repairItem = AIFlags(rawValue: 0x20000)
            
            public init(rawValue: UInt32) {
                self.rawValue = rawValue
            }
        }

        public let hello: UInt8
        public let unknown1: UInt8
        public let fight: UInt8
        public let flee: UInt8
        public let alarm: UInt8
        public let unknown2: UInt8
        public let unknown3: UInt8
        public let unknown4: UInt8
        public let flags: UInt32

        init(_ r: BinaryReader, _ dataSize: Int) {
            hello = r.readByte()
            unknown1 = r.readByte()
            fight = r.readByte()
            flee = r.readByte()
            alarm = r.readByte()
            unknown2 = r.readByte()
            unknown3 = r.readByte()
            unknown4 = r.readByte()
            flags = r.readLEUInt32()
        }
    }

    public struct AI_WField {
        public let distance: Int16
        public let duration: Int16
        public let timeOfDay: UInt8
        public let idle: Data
        public let unknown: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            distance = r.readLEInt16()
            duration = r.readLEInt16()
            timeOfDay = r.readByte()
            idle = r.readBytes(8)
            unknown = r.readByte()
        }
    }

    public struct AI_TField {
        public let x: Float
        public let y: Float
        public let z: Float
        public let unknown: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            x = r.readLESingle()
            y = r.readLESingle()
            z = r.readLESingle()
            unknown = r.readLESingle()
        }
    }

    public struct AI_FField {
        public let x: Float
        public let y: Float
        public let z: Float
        public let duration: Int16
        public let id: String
        public let unknown: Int16

        init(_ r: BinaryReader, _ dataSize: Int) {
            x = r.readLESingle()
            y = r.readLESingle()
            z = r.readLESingle()
            duration = r.readLEInt16()
            id = r.readASCIIString(32, format: .zeroPadded)
            unknown = r.readLEInt16()
        }
    }

    public struct AI_AField {
        public let name: String
        public let unknown: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            name = r.readASCIIString(32, format: .zeroPadded)
            unknown = r.readByte()
        }
    }

    public override var description: String { return "CREA: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // NIF Model
    public var FNAM: STRVField! // Creature name
    public var NPDT: NPDTField! // Creature data
    public var FLAG: IN32Field! // Creature Flags
    public var SCRI: FMIDField<SCPTRecord>! // Script
    public var NPCO: CNTOField! // Item record
    public var AIDT: AIDTField! // AI data
    public var AI_W: AI_WField! // AI Wander
    public var AI_T: AI_TField? = nil // AI Travel
    public var AI_F: AI_FField? = nil // AI Follow
    public var AI_E: AI_FField? = nil // AI Escort
    public var AI_A: AI_AField? = nil // AI Activate
    public var XSCL: FLTVField? = nil // Scale (optional), Only present if the scale is not 1.0
    public var CNAM: STRVField? = nil
    public var NPCSs = [STRVField]()

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = r.readSTRV(dataSize)
        case "NPDT": NPDT = NPDTField(r, dataSize)
        case "FLAG": FLAG = r.readT(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "NPCO": NPCO = CNTOField(r, dataSize, for: format)
        case "AIDT": AIDT = AIDTField(r, dataSize)
        case "AI_W": AI_W = AI_WField(r, dataSize)
        case "AI_T": AI_T = AI_TField(r, dataSize)
        case "AI_F": AI_F = AI_FField(r, dataSize)
        case "AI_E": AI_E = AI_FField(r, dataSize)
        case "AI_A": AI_A = AI_AField(r, dataSize)
        case "XSCL": XSCL = r.readT(dataSize)
        case "CNAM": CNAM = r.readSTRV(dataSize)
        case "NPCS": NPCSs.append(r.readSTRV(dataSize, format: .zeroPadded))
        default: return false
        }
        return true
    }
}
