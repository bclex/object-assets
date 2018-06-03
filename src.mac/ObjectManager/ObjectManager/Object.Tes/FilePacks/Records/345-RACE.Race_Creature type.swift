//
//  RACERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class RACERecord: Record {
    // TESX
    public class DATAField
    {
        public enum RaceFlag : uint
        {
            Playable = 0x00000001,
            FaceGenHead = 0x00000002,
            Child = 0x00000004,
            TiltFrontBack = 0x00000008,
            TiltLeftRight = 0x00000010,
            NoShadow = 0x00000020,
            Swims = 0x00000040,
            Flies = 0x00000080,
            Walks = 0x00000100,
            Immobile = 0x00000200,
            NotPushable = 0x00000400,
            NoCombatInWater = 0x00000800,
            NoRotatingToHeadTrack = 0x00001000,
            DontShowBloodSpray = 0x00002000,
            DontShowBloodDecal = 0x00004000,
            UsesHeadTrackAnims = 0x00008000,
            SpellsAlignWMagicNode = 0x00010000,
            UseWorldRaycastsForFootIK = 0x00020000,
            AllowRagdollCollision = 0x00040000,
            RegenHPInCombat = 0x00080000,
            CantOpenDoors = 0x00100000,
            AllowPCDialogue = 0x00200000,
            NoKnockdowns = 0x00400000,
            AllowPickpocket = 0x00800000,
            AlwaysUseProxyController = 0x01000000,
            DontShowWeaponBlood = 0x02000000,
            OverlayHeadPartList = 0x04000000, //{> Only one can be active <}
            OverrideHeadPartList = 0x08000000, //{> Only one can be active <}
            CanPickupItems = 0x10000000,
            AllowMultipleMembraneShaders = 0x20000000,
            CanDualWield = 0x40000000,
            AvoidsRoads = 0x80000000,
        }

        public struct SkillBoost
        {
            public byte SkillId;
            public sbyte Bonus;
        }

        public struct Gender
        {
            public float Height;
            public float Weight;
            // Attributes;
            public byte Strength;
            public byte Intelligence;
            public byte Willpower;
            public byte Agility;
            public byte Speed;
            public byte Endurance;
            public byte Personality;
            public byte Luck;
        }

        public SkillBoost[] SkillBoosts; // Skill Boosts
        public Gender Male;
        public Gender Female;
        public uint Flags; // 1 = Playable 2 = Beast Race

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Male = new Gender();
            Female = new Gender();
            SkillBoosts = new SkillBoost[7];
            if (formatId == GameFormatId.TES3)
            {
                for (var i = 0; i < SkillBoosts.Length; i++)
                    SkillBoosts[i] = new SkillBoost { SkillId = (byte)r.ReadLEInt32(), Bonus = (sbyte)r.ReadLEInt32() };
                Male.Strength = (byte)r.ReadLEInt32(); Female.Strength = (byte)r.ReadLEInt32();
                Male.Intelligence = (byte)r.ReadLEInt32(); Female.Intelligence = (byte)r.ReadLEInt32();
                Male.Willpower = (byte)r.ReadLEInt32(); Female.Willpower = (byte)r.ReadLEInt32();
                Male.Agility = (byte)r.ReadLEInt32(); Female.Agility = (byte)r.ReadLEInt32();
                Male.Speed = (byte)r.ReadLEInt32(); Female.Speed = (byte)r.ReadLEInt32();
                Male.Endurance = (byte)r.ReadLEInt32(); Female.Endurance = (byte)r.ReadLEInt32();
                Male.Personality = (byte)r.ReadLEInt32(); Female.Personality = (byte)r.ReadLEInt32();
                Male.Luck = (byte)r.ReadLEInt32(); Female.Luck = (byte)r.ReadLEInt32();
                Male.Height = r.ReadLESingle(); Female.Height = r.ReadLESingle();
                Male.Weight = r.ReadLESingle(); Female.Weight = r.ReadLESingle();
                Flags = r.ReadLEUInt32();
                return;
            }
            for (var i = 0; i < SkillBoosts.Length; i++)
                SkillBoosts[i] = new SkillBoost { SkillId = r.ReadByte(), Bonus = (sbyte)r.ReadByte() };
            r.ReadLEInt16(); // padding
            Male.Height = r.ReadLESingle();
            Female.Height = r.ReadLESingle();
            Male.Weight = r.ReadLESingle();
            Female.Weight = r.ReadLESingle();
            Flags = r.ReadLEUInt32();
        }

        public void ATTRField(UnityBinaryReader r, uint dataSize)
        {
            Male.Strength = r.ReadByte();
            Male.Intelligence = r.ReadByte();
            Male.Willpower = r.ReadByte();
            Male.Agility = r.ReadByte();
            Male.Speed = r.ReadByte();
            Male.Endurance = r.ReadByte();
            Male.Personality = r.ReadByte();
            Male.Luck = r.ReadByte();
            Female.Strength = r.ReadByte();
            Female.Intelligence = r.ReadByte();
            Female.Willpower = r.ReadByte();
            Female.Agility = r.ReadByte();
            Female.Speed = r.ReadByte();
            Female.Endurance = r.ReadByte();
            Female.Personality = r.ReadByte();
            Female.Luck = r.ReadByte();
        }
    }

    // TES4
    public class FacePartGroup
    {
        public enum Indx : uint
        {
            Head,
            Ear_Male,
            Ear_Female,
            Mouth,
            Teeth_Lower,
            Teeth_Upper,
            Tongue,
            Eye_Left,
            Eye_Right,
        }

        public UI32Field INDX;
        public MODLGroup MODL;
        public FILEField ICON;
    }

    public class BodyPartGroup
    {
        public enum Indx : uint
        {
            UpperBody,
            LowerBody,
            Hand,
            Foot,
            Tail
        }

        public UI32Field INDX;
        public FILEField ICON;
    }

    public class BodyGroup
    {
        public FILEField MODL;
        public FLTVField MODB;
        public List<BodyPartGroup> BodyParts = new List<BodyPartGroup>();
    }

    public var description: String { return "RACE: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FULL; // Race name
    public STRVField DESC; // Race description
    public List<STRVField> SPLOs = new List<STRVField>(); // NPCs: Special power/ability name
    // TESX
    public DATAField DATA; // RADT:DATA/ATTR: Race data/Base Attributes
    // TES4
    public FMID2Field<RACERecord> VNAM; // Voice
    public FMID2Field<HAIRRecord> DNAM; // Default Hair
    public BYTEField CNAM; // Default Hair Color
    public FLTVField PNAM; // FaceGen - Main clamp
    public FLTVField UNAM; // FaceGen - Face clamp
    public UNKNField XNAM; // Unknown
    //
    public List<FMIDField<HAIRRecord>> HNAMs = new List<FMIDField<HAIRRecord>>();
    public List<FMIDField<EYESRecord>> ENAMs = new List<FMIDField<EYESRecord>>();
    public BYTVField FGGS; // FaceGen Geometry-Symmetric
    public BYTVField FGGA; // FaceGen Geometry-Asymmetric
    public BYTVField FGTS; // FaceGen Texture-Symmetric
    public UNKNField SNAM; // Unknown

    // Parts
    public sbyte NameState;
    public sbyte GenderState;
    public List<FacePartGroup> FaceParts = new List<FacePartGroup>();
    public BodyGroup[] Bodys = new[] { new BodyGroup(), new BodyGroup() };

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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
            switch NameState {
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
                case "NAM0": NameState++
                default: return false
                }
                return true
            case 1: // Face ata
                switch type {
                case "INDX": FaceParts.append(FacePartGroup(INDX: UI32Field(r, dataSize)))
                case "MODL": FaceParts.last!.MODL = MODLGroup(r, dataSize)
                case "ICON": FaceParts.last!.ICON = FILEField(r, dataSize)
                case "MODB": FaceParts.last!.MODL.MODBField(r, dataSize)
                case "NAM1": NameState++
                default: return false
                }
                return true
            case 2: // Body Data
                switch type {
                case "MNAM": GenderState = 0
                case "FNAM": GenderState = 1
                case "MODL": Bodys[GenderState].MODL = FILEField(r, dataSize)
                case "MODB": Bodys[GenderState].MODB = FLTVField(r, dataSize)
                case "INDX": Bodys[GenderState].BodyParts.append(BodyPartGroup(INDX: UI32Field(r, dataSize)))
                case "ICON": Bodys[GenderState].BodyParts.last!.ICON = FILEField(r, dataSize)
                case "HNAM": NameState++
                default: return false
                }
                if NameState == 2 {
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
        fatalError("")
    }
}
