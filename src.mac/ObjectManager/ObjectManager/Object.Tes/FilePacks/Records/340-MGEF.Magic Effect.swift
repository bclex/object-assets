//
//  MGEFRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class MGEFRecord: Record {
    // TES3
    public struct MEDTField
    {
        public int SpellSchool; // 0 = Alteration, 1 = Conjuration, 2 = Destruction, 3 = Illusion, 4 = Mysticism, 5 = Restoration
        public float BaseCost;
        public int Flags; // 0x0200 = Spellmaking, 0x0400 = Enchanting, 0x0800 = Negative
        public ColorRef Color;
        public float SpeedX;
        public float SizeX;
        public float SizeCap;

        public MEDTField(UnityBinaryReader r, uint dataSize)
        {
            SpellSchool = r.ReadLEInt32();
            BaseCost = r.ReadLESingle();
            Flags = r.ReadLEInt32();
            Color = new ColorRef((byte)r.ReadLEInt32(), (byte)r.ReadLEInt32(), (byte)r.ReadLEInt32());
            SpeedX = r.ReadLESingle();
            SizeX = r.ReadLESingle();
            SizeCap = r.ReadLESingle();
        }
    }

    // TES4
    [Flags]
    public enum MFEGFlag : uint
    {
        Hostile = 0x00000001,
        Recover = 0x00000002,
        Detrimental = 0x00000004,
        MagnitudePercent = 0x00000008,
        Self = 0x00000010,
        Touch = 0x00000020,
        Target = 0x00000040,
        NoDuration = 0x00000080,
        NoMagnitude = 0x00000100,
        NoArea = 0x00000200,
        FXPersist = 0x00000400,
        Spellmaking = 0x00000800,
        Enchanting = 0x00001000,
        NoIngredient = 0x00002000,
        Unknown14 = 0x00004000,
        Unknown15 = 0x00008000,
        UseWeapon = 0x00010000,
        UseArmor = 0x00020000,
        UseCreature = 0x00040000,
        UseSkill = 0x00080000,
        UseAttribute = 0x00100000,
        Unknown21 = 0x00200000,
        Unknown22 = 0x00400000,
        Unknown23 = 0x00800000,
        UseActorValue = 0x01000000,
        SprayProjectileType = 0x02000000, // (Ball if Spray, Bolt or Fog is not specified)
        BoltProjectileType = 0x04000000,
        NoHitEffect = 0x08000000,
        Unknown28 = 0x10000000,
        Unknown29 = 0x20000000,
        Unknown30 = 0x40000000,
        Unknown31 = 0x80000000,
    }

    public class DATAField
    {
        public uint Flags;
        public float BaseCost;
        public int AssocItem;
        public int MagicSchool;
        public int ResistValue;
        public uint CounterEffectCount; // Must be updated automatically when ESCE length changes!
        public FormId<LIGHRecord> Light;
        public float ProjectileSpeed;
        public FormId<EFSHRecord> EffectShader;
        public FormId<EFSHRecord> EnchantEffect;
        public FormId<SOUNRecord> CastingSound;
        public FormId<SOUNRecord> BoltSound;
        public FormId<SOUNRecord> HitSound;
        public FormId<SOUNRecord> AreaSound;
        public float ConstantEffectEnchantmentFactor;
        public float ConstantEffectBarterFactor;

        public DATAField(UnityBinaryReader r, uint dataSize)
        {
            Flags = r.ReadLEUInt32();
            BaseCost = r.ReadLESingle();
            AssocItem = r.ReadLEInt32();
            //wbUnion('Assoc. Item', wbMGEFFAssocItemDecider, [
            //  wbFormIDCk('Unused', [NULL]),
            //  wbFormIDCk('Assoc. Weapon', [WEAP]),
            //  wbFormIDCk('Assoc. Armor', [ARMO, NULL{?}]),
            //  wbFormIDCk('Assoc. Creature', [CREA, LVLC, NPC_]),
            //  wbInteger('Assoc. Actor Value', itS32, wbActorValueEnum)
            MagicSchool = r.ReadLEInt32();
            ResistValue = r.ReadLEInt32();
            CounterEffectCount = r.ReadLEUInt16();
            r.ReadLEInt16(); // Unused
            Light = new FormId<LIGHRecord>(r.ReadLEUInt32());
            ProjectileSpeed = r.ReadLESingle();
            EffectShader = new FormId<EFSHRecord>(r.ReadLEUInt32());
            if (dataSize == 36)
                return;
            EnchantEffect = new FormId<EFSHRecord>(r.ReadLEUInt32());
            CastingSound = new FormId<SOUNRecord>(r.ReadLEUInt32());
            BoltSound = new FormId<SOUNRecord>(r.ReadLEUInt32());
            HitSound = new FormId<SOUNRecord>(r.ReadLEUInt32());
            AreaSound = new FormId<SOUNRecord>(r.ReadLEUInt32());
            ConstantEffectEnchantmentFactor = r.ReadLESingle();
            ConstantEffectBarterFactor = r.ReadLESingle();
        }
    }

    public var description: String { return "MGEF: \(INDX):\(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public STRVField DESC; // Description
    // TES3
    public INTVField INDX; // The Effect ID (0 to 137)
    public MEDTField MEDT; // Effect Data
    public FILEField ICON; // Effect Icon
    public STRVField PTEX; // Particle texture
    public STRVField CVFX; // Casting visual
    public STRVField BVFX; // Bolt visual
    public STRVField HVFX; // Hit visual
    public STRVField AVFX; // Area visual
    public STRVField? CSND; // Cast sound (optional)
    public STRVField? BSND; // Bolt sound (optional)
    public STRVField? HSND; // Hit sound (optional)
    public STRVField? ASND; // Area sound (optional)
    // TES4
    public STRVField FULL;
    public MODLGroup MODL;
    public DATAField DATA;
    public List<STRVField> ESCEs;

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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
        case "ESCE": ESCEs = [STRVField](); for i in 0..<(dataSize >> 2) { ESCEs.append(STRVField(r, 4)) }
        default: return false
        }
        return true
    }
}