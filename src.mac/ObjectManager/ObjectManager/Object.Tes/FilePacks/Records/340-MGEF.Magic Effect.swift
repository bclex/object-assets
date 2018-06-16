//
//  MGEFRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class MGEFRecord: Record {
    // TES3
    public struct MEDTField {
        public let SpellSchool: Int32 // 0 = Alteration, 1 = Conjuration, 2 = Destruction, 3 = Illusion, 4 = Mysticism, 5 = Restoration
        public let BaseCost: Float
        public let Flags: Int // 0x0200 = Spellmaking, 0x0400 = Enchanting, 0x0800 = Negative
        public let Color: ColorRef
        public let SpeedX: Float
        public let SizeX: Float
        public let SizeCap: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            spellSchool = r.readLEInt32()
            baseCost = r.readLESingle()
            flags = r.readLEInt32()
            color = ColorRef(UInt8(r.readLEInt32()), UInt8(r.readLEInt32()), UInt8(r.readLEInt32()))
            speedX = r.readLESingle()
            sizeX = r.readLESingle()
            sizeCap = r.readLESingle()
        }
    }

    // TES4
    public struct MFEGFlag: OptionSet {
        let rawValue: UInt8
        public static let hostile = MFEGFlag(rawValue: 0x00000001)
        public static let recover = MFEGFlag(rawValue: 0x00000002)
        public static let detrimental = MFEGFlag(rawValue: 0x00000004)
        public static let magnitudePercent = MFEGFlag(rawValue: 0x00000008)
        public static let self_ = MFEGFlag(rawValue: 0x00000010)
        public static let touch = MFEGFlag(rawValue: 0x00000020)
        public static let target = MFEGFlag(rawValue: 0x00000040)
        public static let noDuration = MFEGFlag(rawValue: 0x00000080)
        public static let noMagnitude = MFEGFlag(rawValue: 0x00000100)
        public static let noArea = MFEGFlag(rawValue: 0x00000200)
        public static let fxPersist = MFEGFlag(rawValue: 0x00000400)
        public static let spellmaking = MFEGFlag(rawValue: 0x00000800)
        public static let enchanting = MFEGFlag(rawValue: 0x00001000)
        public static let noIngredient = MFEGFlag(rawValue: 0x00002000)
        public static let unknown14 = MFEGFlag(rawValue: 0x00004000)
        public static let unknown15 = MFEGFlag(rawValue: 0x00008000)
        public static let useWeapon = MFEGFlag(rawValue: 0x00010000)
        public static let useArmor = MFEGFlag(rawValue: 0x00020000)
        public static let useCreature = MFEGFlag(rawValue: 0x00040000)
        public static let useSkill = MFEGFlag(rawValue: 0x00080000)
        public static let useAttribute = MFEGFlag(rawValue: 0x00100000)
        public static let unknown21 = MFEGFlag(rawValue: 0x00200000)
        public static let unknown22 = MFEGFlag(rawValue: 0x00400000)
        public static let unknown23 = MFEGFlag(rawValue: 0x00800000)
        public static let useActorValue = MFEGFlag(rawValue: 0x01000000)
        public static let sprayProjectileType = MFEGFlag(rawValue: 0x02000000) // (Ball if Spray, Bolt or Fog is not specified)
        public static let boltProjectileType = MFEGFlag(rawValue: 0x04000000)
        public static let noHitEffect = MFEGFlag(rawValue: 0x08000000)
        public static let unknown28 = MFEGFlag(rawValue: 0x10000000)
        public static let unknown29 = MFEGFlag(rawValue: 0x20000000)
        public static let unknown30 = MFEGFlag(rawValue: 0x40000000)
        public static let unknown31 = MFEGFlag(rawValue: 0x80000000)
    }

    public class DATAField {
        public let flags: UInt32
        public let baseCost: Float
        public let assocItem: Int32
        public let magicSchool: Int32
        public let resistValue: Int32
        public let counterEffectCount: UInt32 // Must be updated automatically when ESCE length changes!
        public let light: FormId<LIGHRecord>
        public let projectileSpeed: Float
        public let effectShader: FormId<EFSHRecord>
        public let enchantEffect: FormId<EFSHRecord>
        public let castingSound: FormId<SOUNRecord>
        public let boltSound: FormId<SOUNRecord>
        public let hitSound: FormId<SOUNRecord>
        public let areaSound: FormId<SOUNRecord>
        public let constantEffectEnchantmentFactor: Float
        public let constantEffectBarterFactor: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            flags = r.readLEUInt32()
            baseCost = r.readLESingle()
            assocItem = r.readLEInt32()
            //wbUnion('Assoc. Item', wbMGEFFAssocItemDecider, [
            //  wbFormIDCk('Unused', [NULL]),
            //  wbFormIDCk('Assoc. Weapon', [WEAP]),
            //  wbFormIDCk('Assoc. Armor', [ARMO, NULL{?}]),
            //  wbFormIDCk('Assoc. Creature', [CREA, LVLC, NPC_]),
            //  wbInteger('Assoc. Actor Value', itS32, wbActorValueEnum)
            magicSchool = r.readLEInt32()
            resistValue = r.readLEInt32()
            counterEffectCount = r.readLEUInt16()
            r.skipBytes(2) // Unused
            light = FormId<LIGHRecord>(r.readLEUInt32())
            projectileSpeed = r.readLESingle()
            effectShader = FormId<EFSHRecord>(r.readLEUInt32())
            guard dataSize != 36 else {
                return
            }
            enchantEffect = FormId<EFSHRecord>(r.readLEUInt32())
            castingSound = FormId<SOUNRecord>(r.readLEUInt32())
            boltSound = FormId<SOUNRecord>(r.readLEUInt32())
            hitSound = FormId<SOUNRecord>(r.readLEUInt32())
            areaSound = FormId<SOUNRecord>(r.readLEUInt32())
            constantEffectEnchantmentFactor = r.readLESingle()
            constantEffectBarterFactor = r.readLESingle()
        }
    }

    public var description: String { return "MGEF: \(INDX):\(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var DESC: STRVField // Description
    // TES3
    public var INDX: INTVField // The Effect ID (0 to 137)
    public var MEDT: MEDTField // Effect Data
    public var ICON: FILEField // Effect Icon
    public var PTEX: STRVField // Particle texture
    public var CVFX: STRVField // Casting visual
    public var BVFX: STRVField // Bolt visual
    public var HVFX: STRVField // Hit visual
    public var AVFX: STRVField // Area visual
    public var CSND: STRVField? // Cast sound (optional)
    public var BSND: STRVField? // Bolt sound (optional)
    public var HSND: STRVField? // Hit sound (optional)
    public var ASND: STRVField? // Area sound (optional)
    // TES4
    public var FULL: STRVField
    public var MODL: MODLGroup
    public var DATA: DATAField
    public var ESCEs: [STRVField]

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "INDX": INDX = INTVField(r, dataSize)
            case "MEDT": MEDT = MEDTField(r, dataSize)
            case "ITEX": ICON = FILEField(r, dataSize)
            case "PTEX": PTEX = STRVField(r, dataSize)
            case "CVFX": CVFX = STRVField(r, dataSize)
            case "BVFX": BVFX = STRVField(r, dataSize)
            case "HVFX": HVFX = STRVField(r, dataSize)
            case "AVFX": AVFX = STRVField(r, dataSize)
            case "DESC": DESC = STRVField(r, dataSize)
            case "CSND": CSND = STRVField(r, dataSize)
            case "BSND": BSND = STRVField(r, dataSize)
            case "HSND": HSND = STRVField(r, dataSize)
            case "ASND": ASND = STRVField(r, dataSize)
            default: return false
            }
            return true
        }
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "DESC": DESC = STRVField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "ESCE": ESCEs = [STRVField](); ESCEs.reserveCapacity(dataSize >> 2) for i in 0..<ESCEs.capacity { ESCEs.append(STRVField(r, 4)) }
        default: return false
        }
        return true
    }
}
