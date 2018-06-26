using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class MGEFRecord : Record
    {
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

            public MEDTField(UnityBinaryReader r, int dataSize)
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

            public DATAField(UnityBinaryReader r, int dataSize)
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
                r.SkipBytes(2); // Unused
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

        public override string ToString() => $"MGEF: {INDX.Value}:{EDID.Value}";
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
        public STRVField[] ESCEs;

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            if (format == GameFormatId.TES3)
                switch (type)
                {
                    case "INDX": INDX = new INTVField(r, dataSize); return true;
                    case "MEDT": MEDT = new MEDTField(r, dataSize); return true;
                    case "ITEX": ICON = new FILEField(r, dataSize); return true;
                    case "PTEX": PTEX = new STRVField(r, dataSize); return true;
                    case "CVFX": CVFX = new STRVField(r, dataSize); return true;
                    case "BVFX": BVFX = new STRVField(r, dataSize); return true;
                    case "HVFX": HVFX = new STRVField(r, dataSize); return true;
                    case "AVFX": AVFX = new STRVField(r, dataSize); return true;
                    case "DESC": DESC = new STRVField(r, dataSize); return true;
                    case "CSND": CSND = new STRVField(r, dataSize); return true;
                    case "BSND": BSND = new STRVField(r, dataSize); return true;
                    case "HSND": HSND = new STRVField(r, dataSize); return true;
                    case "ASND": ASND = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "FULL": FULL = new STRVField(r, dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                case "ICON": ICON = new FILEField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "MODB": MODL.MODBField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                case "ESCE":
                    ESCEs = new STRVField[dataSize >> 2];
                    for (var i = 0; i < ESCEs.Length; i++) ESCEs[i] = new STRVField(r, 4); return true;
                default: return false;
            }
        }
    }
}