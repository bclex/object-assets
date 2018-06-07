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
        public enum RaceFlag: UInt32 {
            playable = 0x00000001
            faceGenHead = 0x00000002
            child = 0x00000004
            tiltFrontBack = 0x00000008
            tiltLeftRight = 0x00000010
            noShadow = 0x00000020
            swims = 0x00000040
            flies = 0x00000080
            walks = 0x00000100
            immobile = 0x00000200
            notPushable = 0x00000400
            noCombatInWater = 0x00000800
            noRotatingToHeadTrack = 0x00001000
            dontShowBloodSpray = 0x00002000
            dontShowBloodDecal = 0x00004000
            usesHeadTrackAnims = 0x00008000
            spellsAlignWMagicNode = 0x00010000
            useWorldRaycastsForFootIK = 0x00020000
            allowRagdollCollision = 0x00040000
            regenHPInCombat = 0x00080000
            cantOpenDoors = 0x00100000
            allowPCDialogue = 0x00200000
            noKnockdowns = 0x00400000
            allowPickpocket = 0x00800000
            alwaysUseProxyController = 0x01000000
            dontShowWeaponBlood = 0x02000000
            overlayHeadPartList = 0x04000000 //{> Only one can be active <}
            overrideHeadPartList = 0x08000000 //{> Only one can be active <}
            canPickupItems = 0x10000000
            allowMultipleMembraneShaders = 0x20000000
            canDualWield = 0x40000000
            avoidsRoads = 0x80000000
        }

        public struct SkillBoost {
            public let skillId: UInt8
            public let bonus: Int8

            init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
                guard format != .TES3 else {
                    skillId = UInt8(r.readLEInt32())
                    bonus = Int8(r.readLEInt32())
                    return
                }
                skillId = r.readByte()
                bonus = r.readSByte()
            }
        }

        public class RaceStats {
            public let height: Float
            public let weight: Float
            // Attributes
            public let strength: UInt8
            public let intelligence: UInt8
            public let willpower: UInt8
            public let agility: UInt8
            public let speed: UInt8
            public let endurance: UInt8
            public let personality: UInt8
            public let luck: UInt8
        }

        public let skillBoosts = [SkillBoost]() // Skill Boosts
        public let male = RaceStats()
        public let female = RaceStats()
        public let flags: UInt32 // 1 = Playable 2 = Beast Race

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            skillBoosts.reserveCapacity(7)
            guard format != .TES3 {
                for i in 0..<skillBoosts.capacity {
                    skillBoosts[i] = SkillBoost(r, 8, format)
                }
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
            for i in 0..<skillBoosts.capacity {
                skillBoosts[i] = skillBoost(r, 2, format)
            }
            r.readLEInt16() // padding
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
    public class FacePartGroup
    {
        public enum Indx: UInt32 {
            case head, ear_male, ear_female, mouth, teeth_lower, teeth_upper, tongue, eye_left, eye_right
        }

        public var INDX: UI32Field
        public var MODL: MODLGroup
        public var ICON: FILEField
    }

    public class BodyPartGroup
    {
        public enum Indx: UInt32 {
            upperBody, lowerBody, hand, foot, tail
        }

        public var INDX: UI32Field
        public var ICON: FILEField
    }

    public class BodyGroup {
        public var MODL: FILEField
        public var MODB: FLTVField 
        public var BodyParts: [BodyPartGroup]()
    }

    public var description: String { return "RACE: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var FULL: STRVField // Race name
    public var DESC: STRVField // Race description
    public var SPLOs = [STRVField]() // NPCs: Special power/ability name
    // TESX
    public var DATA: DATAField // RADT:DATA/ATTR: Race data/Base Attributes
    // TES4
    public var VNAM: FMID2Field<RACERecord> // Voice
    public var DNAM: FMID2Field<HAIRRecord> // Default Hair
    public var CNAM: BYTEField // Default Hair Color
    public var PNAM: FLTVField // FaceGen - Main clamp
    public var UNAM: FLTVField // FaceGen - Face clamp
    public var XNAM: UNKNField // Unknown
    //
    public var HNAMs = [FMIDField<HAIRRecord>]()
    public var ENAMs = [FMIDField<EYESRecord>]()
    public var FGGS: BYTVField // FaceGen Geometry-Symmetric
    public var FGGA: BYTVField // FaceGen Geometry-Asymmetric
    public var FGTS: BYTVField // FaceGen Texture-Symmetric
    public var SNAM: UNKNField // Unknown

    // Parts
    public var faceParts = [FacePartGroup]()
    public var bodys = [BodyGroup(), BodyGroup()]
    var _nameState: Int8
    var _genderState: Int8

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = STRVField(r, dataSize)
            case "FNAM": FULL = STRVField(r, dataSize)
            case "RADT": DATA = DATAField(r, dataSize, format)
            case "NPCS": SPLOs.append(STRVField(r, dataSize))
            case "DESC": DESC = STRVField(r, dataSize)
            default: return false
            }
            return true
        }
        else if format == .TES4 {
            switch _nameState {
            case 0:
                switch type {
                case "EDID": EDID = STRVField(r, dataSize)
                case "FULL": FULL = STRVField(r, dataSize)
                case "DESC": DESC = STRVField(r, dataSize)
                case "DATA": DATA = DATAField(r, dataSize, format)
                case "SPLO": SPLOs.append(STRVField(r, dataSize))
                case "VNAM": VNAM = FMID2Field<RACERecord>(r, dataSize)
                case "DNAM": DNAM = FMID2Field<HAIRRecord>(r, dataSize)
                case "CNAM": CNAM = BYTEField(r, dataSize)
                case "PNAM": PNAM = FLTVField(r, dataSize)
                case "UNAM": UNAM = FLTVField(r, dataSize)
                case "XNAM": XNAM = UNKNField(r, dataSize)
                case "ATTR": DATA.ATTRField(r, dataSize)
                case "NAM0": _nameState += 1
                default: return false
                }
                return true
            case 1: // Face Data
                switch type {
                case "INDX": FaceParts.append(FacePartGroup(INDX: UI32Field(r, dataSize)))
                case "MODL": FaceParts.last!.MODL = MODLGroup(r, dataSize)
                case "ICON": FaceParts.last!.ICON = FILEField(r, dataSize)
                case "MODB": FaceParts.last!.MODL.MODBField(r, dataSize)
                case "NAM1": _nameState += 1
                default: return false
                }
                return true
            case 2: // Body Data
                switch type {
                case "MNAM": _genderState = 0
                case "FNAM": _genderState = 1
                case "MODL": Bodys[_genderState].MODL = FILEField(r, dataSize)
                case "MODB": Bodys[_genderState].MODB = FLTVField(r, dataSize)
                case "INDX": Bodys[_genderState].BodyParts.append(BodyPartGroup(INDX: UI32Field(r, dataSize)))
                case "ICON": Bodys[_genderState].BodyParts.last!.ICON = FILEField(r, dataSize)
                case "HNAM": _nameState += 1
                default: return false
                }
                guard _nameState != 2 else {
                    return true    
                }
                fallthrough
            case 3: // Postamble
                switch type {
                case "HNAM": for i in 0..<(dataSize >> 2) { HNAMs.append(FMIDField<HAIRRecord>(r, 4)) }
                case "ENAM": for i in 0..<(dataSize >> 2) { ENAMs.append(FMIDField<EYESRecord>(r, 4)) }
                case "FGGS": FGGS = BYTVField(r, dataSize)
                case "FGGA": FGGA = BYTVField(r, dataSize)
                case "FGTS": FGTS = BYTVField(r, dataSize)
                case "SNAM": SNAM = UNKNField(r, dataSize)
                default: return false
                }
                return true
            default: return false
            }
            return true
        }
        fatalError("Error")
    }
}
