//
//  MGEFRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class MGEFRecord: Record, IHaveEDID, IHaveMODL {
    // TES3
    public struct MEDTField {
        public let spellSchool: Int32 // 0 = Alteration, 1 = Conjuration, 2 = Destruction, 3 = Illusion, 4 = Mysticism, 5 = Restoration
        public let baseCost: Float
        public let flags: Int32 // 0x0200 = Spellmaking, 0x0400 = Enchanting, 0x0800 = Negative
        public let color: ColorRef4
        public let speedX: Float
        public let sizeX: Float
        public let sizeCap: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            spellSchool = r.readLEInt32()
            baseCost = r.readLESingle()
            flags = r.readLEInt32()
            color = ColorRef4(red: UInt8(r.readLEInt32()), green: UInt8(r.readLEInt32()), blue: UInt8(r.readLEInt32()), null: 255)
            speedX = r.readLESingle()
            sizeX = r.readLESingle()
            sizeCap = r.readLESingle()
        }
    }

    // TES4
    public struct MFEGFlag: OptionSet {
        public let rawValue: UInt32
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
        
        public init(rawValue: UInt32) {
            self.rawValue = rawValue
        }
    }

    public typealias DATAField = (
        flags: UInt32,
        baseCost: Float,
        assocItem: Int32,
        magicSchool: Int32,
        resistValue: Int32,
        counterEffectCount: UInt16, // Must be updated automatically when ESCE length changes!
        light: FormId32<LIGHRecord>,
        projectileSpeed: Float,
        effectShader: FormId32<EFSHRecord>,
        enchantEffect: FormId32<EFSHRecord>,
        castingSound: FormId32<SOUNRecord>,
        boltSound: FormId32<SOUNRecord>,
        hitSound: FormId32<SOUNRecord>,
        areaSound: FormId32<SOUNRecord>,
        constantEffectEnchantmentFactor: Float,
        constantEffectBarterFactor: Float
    )

//        init(_ r: BinaryReader, _ dataSize: Int) {
//            flags = r.readLEUInt32()
//            baseCost = r.readLESingle()
//            assocItem = r.readLEInt32()
//            //wbUnion('Assoc. Item', wbMGEFFAssocItemDecider, [
//            //  wbFormIDCk('Unused', [NULL]),
//            //  wbFormIDCk('Assoc. Weapon', [WEAP]),
//            //  wbFormIDCk('Assoc. Armor', [ARMO, NULL{?}]),
//            //  wbFormIDCk('Assoc. Creature', [CREA, LVLC, NPC_]),
//            //  wbInteger('Assoc. Actor Value', itS32, wbActorValueEnum)
//            magicSchool = r.readLEInt32()
//            resistValue = r.readLEInt32()
//            counterEffectCount = r.readLEUInt16()
//            r.skipBytes(2) // Unused
//            light = FormId<LIGHRecord>(r.readLEUInt32())
//            projectileSpeed = r.readLESingle()
//            effectShader = FormId<EFSHRecord>(r.readLEUInt32())
//            guard dataSize != 36 else {
//                return
//            }
//            enchantEffect = FormId<EFSHRecord>(r.readLEUInt32())
//            castingSound = FormId<SOUNRecord>(r.readLEUInt32())
//            boltSound = FormId<SOUNRecord>(r.readLEUInt32())
//            hitSound = FormId<SOUNRecord>(r.readLEUInt32())
//            areaSound = FormId<SOUNRecord>(r.readLEUInt32())
//            constantEffectEnchantmentFactor = r.readLESingle()
//            constantEffectBarterFactor = r.readLESingle()
//        }

    public override var description: String { return "MGEF: \(INDX!):\(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var DESC: STRVField! // Description
    // TES3
    public var INDX: INTVField! // The Effect ID (0 to 137)
    public var MEDT: MEDTField! // Effect Data
    public var ICON: FILEField! // Effect Icon
    public var PTEX: STRVField! // Particle texture
    public var CVFX: STRVField! // Casting visual
    public var BVFX: STRVField! // Bolt visual
    public var HVFX: STRVField! // Hit visual
    public var AVFX: STRVField! // Area visual
    public var CSND: STRVField? = nil // Cast sound (optional)
    public var BSND: STRVField? = nil// Bolt sound (optional)
    public var HSND: STRVField? = nil// Hit sound (optional)
    public var ASND: STRVField? = nil// Area sound (optional)
    // TES4
    public var FULL: STRVField!
    public var MODL: MODLGroup? = nil
    public var DATA: DATAField!
    public var ESCEs: [STRVField]!

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "INDX": INDX = r.readINTV(dataSize)
            case "MEDT": MEDT = MEDTField(r, dataSize)
            case "ITEX": ICON = r.readSTRV(dataSize)
            case "PTEX": PTEX = r.readSTRV(dataSize)
            case "CVFX": CVFX = r.readSTRV(dataSize)
            case "BVFX": BVFX = r.readSTRV(dataSize)
            case "HVFX": HVFX = r.readSTRV(dataSize)
            case "AVFX": AVFX = r.readSTRV(dataSize)
            case "DESC": DESC = r.readSTRV(dataSize)
            case "CSND": CSND = r.readSTRV(dataSize)
            case "BSND": BSND = r.readSTRV(dataSize)
            case "HSND": HSND = r.readSTRV(dataSize)
            case "ASND": ASND = r.readSTRV(dataSize)
            default: return false
            }
            return true
        }
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "DESC": DESC = r.readSTRV(dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "DATA": DATA = r.readT(dataSize)
        case "ESCE":
            ESCEs = [STRVField](); let capacity = dataSize >> 2; ESCEs.reserveCapacity(capacity)
            for _ in 0..<capacity { ESCEs.append(r.readSTRV(4)) }
        default: return false
        }
        return true
    }
}
