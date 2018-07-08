//
//  RACERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class RACERecord: Record {
    // TESX
    public class DATAField {
        public struct RaceFlag: OptionSet {
            public let rawValue: UInt32
            public static let playable = 0x00000001
            public static let faceGenHead = 0x00000002
            public static let child = 0x00000004
            public static let tiltFrontBack = 0x00000008
            public static let tiltLeftRight = 0x00000010
            public static let noShadow = 0x00000020
            public static let swims = 0x00000040
            public static let flies = 0x00000080
            public static let walks = 0x00000100
            public static let immobile = 0x00000200
            public static let notPushable = 0x00000400
            public static let noCombatInWater = 0x00000800
            public static let noRotatingToHeadTrack = 0x00001000
            public static let dontShowBloodSpray = 0x00002000
            public static let dontShowBloodDecal = 0x00004000
            public static let usesHeadTrackAnims = 0x00008000
            public static let spellsAlignWMagicNode = 0x00010000
            public static let useWorldRaycastsForFootIK = 0x00020000
            public static let allowRagdollCollision = 0x00040000
            public static let regenHPInCombat = 0x00080000
            public static let cantOpenDoors = 0x00100000
            public static let allowPCDialogue = 0x00200000
            public static let noKnockdowns = 0x00400000
            public static let allowPickpocket = 0x00800000
            public static let alwaysUseProxyController = 0x01000000
            public static let dontShowWeaponBlood = 0x02000000
            public static let overlayHeadPartList = 0x04000000 //{> Only one can be active <}
            public static let overrideHeadPartList = 0x08000000 //{> Only one can be active <}
            public static let canPickupItems = 0x10000000
            public static let allowMultipleMembraneShaders = 0x20000000
            public static let canDualWield = 0x40000000
            public static let avoidsRoads = 0x80000000
            
            public init(rawValue: UInt32) {
                self.rawValue = rawValue
            }
        }

        public struct SkillBoost {
            public let skillId: UInt8
            public let bonus: Int8

            init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
                guard format != .TES3 else {
                    skillId = UInt8(checkMax: r.readLEInt32())
                    bonus = Int8(r.readLEInt32())
                    return
                }
                skillId = r.readByte()
                bonus = r.readSByte()
            }
        }

        public class RaceStats {
            public var height: Float!
            public var weight: Float!
            // Attributes
            public var strength: UInt8!
            public var intelligence: UInt8!
            public var willpower: UInt8!
            public var agility: UInt8!
            public var speed: UInt8!
            public var endurance: UInt8!
            public var personality: UInt8!
            public var luck: UInt8!
        }

        public var skillBoosts: [SkillBoost] // Skill Boosts
        public let male = RaceStats()
        public let female = RaceStats()
        public let flags: UInt32 // 1 = Playable 2 = Beast Race

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            skillBoosts = [SkillBoost](); skillBoosts.reserveCapacity(7)
            guard format != .TES3 else {
                for _ in 0..<7 { skillBoosts.append(SkillBoost(r, 8, format)) }
                male.strength = UInt8(r.readLEInt32()); female.strength = UInt8(r.readLEInt32())
                male.intelligence = UInt8(r.readLEInt32()); female.intelligence = UInt8(r.readLEInt32())
                male.willpower = UInt8(r.readLEInt32()); female.willpower = UInt8(r.readLEInt32())
                male.agility = UInt8(r.readLEInt32()); female.agility = UInt8(r.readLEInt32())
                male.speed = UInt8(r.readLEInt32()); female.speed = UInt8(r.readLEInt32())
                male.endurance = UInt8(r.readLEInt32()); female.endurance = UInt8(r.readLEInt32())
                male.personality = UInt8(r.readLEInt32()); female.personality = UInt8(r.readLEInt32())
                male.luck = UInt8(r.readLEInt32()); female.luck = UInt8(r.readLEInt32())
                male.height = r.readLESingle(); female.height = r.readLESingle()
                male.weight = r.readLESingle(); female.weight = r.readLESingle()
                flags = r.readLEUInt32()
                return
            }
            for _ in 0..<7 { skillBoosts.append(SkillBoost(r, 2, format)) }
            r.skipBytes(2) // padding
            male.height = r.readLESingle(); female.height = r.readLESingle()
            male.weight = r.readLESingle(); female.weight = r.readLESingle()
            flags = r.readLEUInt32()
        }

        func ATTRField(_ r: BinaryReader, _ dataSize: Int) {
            male.strength = r.readByte()
            male.intelligence = r.readByte()
            male.willpower = r.readByte()
            male.agility = r.readByte()
            male.speed = r.readByte()
            male.endurance = r.readByte()
            male.personality = r.readByte()
            male.luck = r.readByte()
            female.strength = r.readByte()
            female.intelligence = r.readByte()
            female.willpower = r.readByte()
            female.agility = r.readByte()
            female.speed = r.readByte()
            female.endurance = r.readByte()
            female.personality = r.readByte()
            female.luck = r.readByte()
        }
    }

    // TES4
    public class FacePartGroup {
        public enum Indx: UInt32 {
            case head = 0, ear_male, ear_female, mouth, teeth_lower, teeth_upper, tongue, eye_left, eye_right
        }

        public let INDX: UI32Field
        public var MODL: MODLGroup!
        public var ICON: FILEField!
        
        init(INDX: UI32Field) {
            self.INDX = INDX
        }
    }

    public class BodyPartGroup {
        public enum Indx: UInt32 {
            case upperBody = 0, lowerBody, hand, foot, tail
        }

        public let INDX: UI32Field
        public var ICON: FILEField!
        
        init(INDX: UI32Field) {
            self.INDX = INDX
        }
    }

    public class BodyGroup {
        public var MODL: FILEField!
        public var MODB: FLTVField!
        public var bodyParts = [BodyPartGroup]()
    }

    public override var description: String { return "RACE: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var FULL: STRVField! // Race name
    public var DESC: STRVField! // Race description
    public var SPLOs = [STRVField]() // NPCs: Special power/ability name
    // TESX
    public var DATA: DATAField! // RADT:DATA/ATTR: Race data/Base Attributes
    // TES4
    public var VNAM: FMID2Field<RACERecord>! // Voice
    public var DNAM: FMID2Field<HAIRRecord>! // Default Hair
    public var CNAM: BYTEField! // Default Hair Color
    public var PNAM: FLTVField! // FaceGen - Main clamp
    public var UNAM: FLTVField! // FaceGen - Face clamp
    public var XNAM: UNKNField! // Unknown
    //
    public var HNAMs = [FMIDField<HAIRRecord>]()
    public var ENAMs = [FMIDField<EYESRecord>]()
    public var FGGS: BYTVField! // FaceGen Geometry-Symmetric
    public var FGGA: BYTVField! // FaceGen Geometry-Asymmetric
    public var FGTS: BYTVField! // FaceGen Texture-Symmetric
    public var SNAM: UNKNField! // Unknown

    // Parts
    public var faceParts = [FacePartGroup]()
    public var bodys = [BodyGroup(), BodyGroup()]
    var _nameState: Int8 = 0
    var _genderState: Int = 0

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = r.readSTRV(dataSize)
            case "FNAM": FULL = r.readSTRV(dataSize)
            case "RADT": DATA = DATAField(r, dataSize, format)
            case "NPCS": SPLOs.append(r.readSTRV(dataSize))
            case "DESC": DESC = r.readSTRV(dataSize)
            default: return false
            }
            return true
        }
        else if format == .TES4 {
            switch _nameState {
            case 0:
                switch type {
                case "EDID": EDID = r.readSTRV(dataSize)
                case "FULL": FULL = r.readSTRV(dataSize)
                case "DESC": DESC = r.readSTRV(dataSize)
                case "DATA": DATA = DATAField(r, dataSize, format)
                case "SPLO": SPLOs.append(r.readSTRV(dataSize))
                case "VNAM": VNAM = FMID2Field<RACERecord>(r, dataSize)
                case "DNAM": DNAM = FMID2Field<HAIRRecord>(r, dataSize)
                case "CNAM": CNAM = r.readT(dataSize)
                case "PNAM": PNAM = r.readT(dataSize)
                case "UNAM": UNAM = r.readT(dataSize)
                case "XNAM": XNAM = r.readBYTV(dataSize)
                case "ATTR": DATA.ATTRField(r, dataSize)
                case "NAM0": _nameState += 1
                default: return false
                }
            case 1: // Face Data
                switch type {
                case "INDX": faceParts.append(FacePartGroup(INDX: r.readT(dataSize)))
                case "MODL": faceParts.last!.MODL = MODLGroup(r, dataSize)
                case "ICON": faceParts.last!.ICON = r.readSTRV(dataSize)
                case "MODB": faceParts.last!.MODL.MODBField(r, dataSize)
                case "NAM1": _nameState += 1
                default: return false
                }
            case 2: // Body Data
                switch type {
                case "MNAM": _genderState = 0
                case "FNAM": _genderState = 1
                case "MODL": bodys[_genderState].MODL = r.readSTRV(dataSize)
                case "MODB": bodys[_genderState].MODB = r.readT(dataSize)
                case "INDX": bodys[_genderState].bodyParts.append(BodyPartGroup(INDX: r.readT(dataSize)))
                case "ICON": bodys[_genderState].bodyParts.last!.ICON = r.readSTRV(dataSize)
                case "HNAM": _nameState += 1
                default: return false
                }
                guard _nameState != 2 else {
                    return true    
                }
                fallthrough
            case 3: // Postamble
                switch type {
                case "HNAM": for _ in 0..<(dataSize >> 2) { HNAMs.append(FMIDField<HAIRRecord>(r, 4)) }
                case "ENAM": for _ in 0..<(dataSize >> 2) { ENAMs.append(FMIDField<EYESRecord>(r, 4)) }
                case "FGGS": FGGS = r.readBYTV(dataSize)
                case "FGGA": FGGA = r.readBYTV(dataSize)
                case "FGTS": FGTS = r.readBYTV(dataSize)
                case "SNAM": SNAM = r.readBYTV(dataSize)
                default: return false
                }
            default: return false
            }
            return true
        }
        fatalError("Error")
    }
}
