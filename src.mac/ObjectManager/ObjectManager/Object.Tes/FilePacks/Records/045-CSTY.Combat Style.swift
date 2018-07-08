//
//  CSTYRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CSTYRecord: Record {
    public class CSTDField {
        public let dodgePercentChance: UInt8
        public let leftRightPercentChance: UInt8
        public let dodgeLeftRightTimer_min: Float
        public let dodgeLeftRightTimer_max: Float
        public let dodgeForwardTimer_min: Float
        public let dodgeForwardTimer_max: Float
        public let dodgeBackTimer_min: Float
        public let dodgeBackTimer_max: Float
        public let idleTimer_min: Float
        public let idleTimer_max: Float
        public let blockPercentChance: UInt8
        public let attackPercentChance: UInt8
        public let recoilStaggerBonusToAttack: Float
        public let unconsciousBonusToAttack: Float
        public let handToHandBonusToAttack: Float
        public let powerAttackPercentChance: UInt8
        public let recoilStaggerBonusToPower: Float
        public let unconsciousBonusToPowerAttack: Float
        public let powerAttack_normal: UInt8
        public let powerAttack_forward: UInt8
        public let powerAttack_back: UInt8
        public let powerAttack_left: UInt8
        public let powerAttack_right: UInt8
        public let holdTimer_min: Float
        public let holdTimer_max: Float
        public let flags1: UInt8
        public let acrobaticDodgePercentChance: UInt8
        public let rangeMult_optimal: Float
        public let rangeMult_max: Float
        public var switchDistance_melee: Float? = nil
        public var switchDistance_ranged: Float? = nil
        public var buffStandoffDistance: Float? = nil
        public var rangedStandoffDistance: Float? = nil
        public var groupStandoffDistance: Float? = nil
        public var rushingAttackPercentChance: UInt8? = nil
        public var rushingAttackDistanceMult: Float? = nil
        public var flags2: UInt32? = nil

        init(_ r: BinaryReader, _ dataSize: Int) {
            dodgePercentChance = r.readByte()
            leftRightPercentChance = r.readByte()
            r.skipBytes(2) // Unused
            dodgeLeftRightTimer_min = r.readLESingle()
            dodgeLeftRightTimer_max = r.readLESingle()
            dodgeForwardTimer_min = r.readLESingle()
            dodgeForwardTimer_max = r.readLESingle()
            dodgeBackTimer_min = r.readLESingle()
            dodgeBackTimer_max = r.readLESingle()
            idleTimer_min = r.readLESingle()
            idleTimer_max = r.readLESingle()
            blockPercentChance = r.readByte()
            attackPercentChance = r.readByte()
            r.skipBytes(2) // Unused
            recoilStaggerBonusToAttack = r.readLESingle()
            unconsciousBonusToAttack = r.readLESingle()
            handToHandBonusToAttack = r.readLESingle()
            powerAttackPercentChance = r.readByte()
            r.skipBytes(3) // Unused
            recoilStaggerBonusToPower = r.readLESingle()
            unconsciousBonusToPowerAttack = r.readLESingle()
            powerAttack_normal = r.readByte()
            powerAttack_forward = r.readByte()
            powerAttack_back = r.readByte()
            powerAttack_left = r.readByte()
            powerAttack_right = r.readByte()
            r.skipBytes(3) // Unused
            holdTimer_min = r.readLESingle()
            holdTimer_max = r.readLESingle()
            flags1 = r.readByte()
            acrobaticDodgePercentChance = r.readByte()
            r.skipBytes(2) // Unused
            guard dataSize != 84 else {
                rangeMult_optimal = 0
                rangeMult_max = 0
                return
            }
            rangeMult_optimal = r.readLESingle()
            rangeMult_max = r.readLESingle()
            guard dataSize != 92 else {
                return
            }
            switchDistance_melee = r.readLESingle()
            switchDistance_ranged = r.readLESingle()
            buffStandoffDistance = r.readLESingle()
            guard dataSize != 104 else {
                return
            }
            rangedStandoffDistance = r.readLESingle()
            groupStandoffDistance = r.readLESingle()
            guard dataSize != 112 else {
                return
            }
            rushingAttackPercentChance = r.readByte()
            r.skipBytes(3) // Unused
            rushingAttackDistanceMult = r.readLESingle()
            guard dataSize != 120 else {
                return
            }
            flags2 = r.readLEUInt32()
        }
    }

    public struct CSADField {
        public let dodgeFatigueModMult: Float
        public let dodgeFatigueModBase: Float
        public let encumbSpeedModBase: Float
        public let encumbSpeedModMult: Float
        public let dodgeWhileUnderAttackMult: Float
        public let dodgeNotUnderAttackMult: Float
        public let dodgeBackWhileUnderAttackMult: Float
        public let dodgeBackNotUnderAttackMult: Float
        public let dodgeForwardWhileAttackingMult: Float
        public let dodgeForwardNotAttackingMult: Float
        public let blockSkillModifierMult: Float
        public let blockSkillModifierBase: Float
        public let blockWhileUnderAttackMult: Float
        public let blockNotUnderAttackMult: Float
        public let attackSkillModifierMult: Float
        public let attackSkillModifierBase: Float
        public let attackWhileUnderAttackMult: Float
        public let attackNotUnderAttackMult: Float
        public let attackDuringBlockMult: Float
        public let powerAttFatigueModBase: Float
        public let powerAttFatigueModMult: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            dodgeFatigueModMult = r.readLESingle()
            dodgeFatigueModBase = r.readLESingle()
            encumbSpeedModBase = r.readLESingle()
            encumbSpeedModMult = r.readLESingle()
            dodgeWhileUnderAttackMult = r.readLESingle()
            dodgeNotUnderAttackMult = r.readLESingle()
            dodgeBackWhileUnderAttackMult = r.readLESingle()
            dodgeBackNotUnderAttackMult = r.readLESingle()
            dodgeForwardWhileAttackingMult = r.readLESingle()
            dodgeForwardNotAttackingMult = r.readLESingle()
            blockSkillModifierMult = r.readLESingle()
            blockSkillModifierBase = r.readLESingle()
            blockWhileUnderAttackMult = r.readLESingle()
            blockNotUnderAttackMult = r.readLESingle()
            attackSkillModifierMult = r.readLESingle()
            attackSkillModifierBase = r.readLESingle()
            attackWhileUnderAttackMult = r.readLESingle()
            attackNotUnderAttackMult = r.readLESingle()
            attackDuringBlockMult = r.readLESingle()
            powerAttFatigueModBase = r.readLESingle()
            powerAttFatigueModMult = r.readLESingle()
        }
    }

    public override var description: String { return "CSTY: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var CSTD: CSTDField! // Standard
    public var CSAD: CSADField! // Advanced
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "CSTD": CSTD = CSTDField(r, dataSize)
        case "CSAD": CSAD = CSADField(r, dataSize)
        default: return false
        }
        return true
    }
}
