//
//  CREARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CREARecord: Record, IHaveEDID, IHaveMODL {
    [Flags]
    public enum CREAFlags : uint
    {
        Biped = 0x0001,
        Respawn = 0x0002,
        WeaponAndShield = 0x0004,
        None = 0x0008,
        Swims = 0x0010,
        Flies = 0x0020,
        Walks = 0x0040,
        DefaultFlags = 0x0048,
        Essential = 0x0080,
        SkeletonBlood = 0x0400,
        MetalBlood = 0x0800
    }

    public struct NPDTField
    {
        public int Type // 0 = Creature, 1 = Daedra, 2 = Undead, 3 = Humanoid
        public int Level;
        public int Strength;
        public int Intelligence;
        public int Willpower;
        public int Agility;
        public int Speed;
        public int Endurance;
        public int Personality;
        public int Luck;
        public int Health;
        public int SpellPts;
        public int Fatigue;
        public int Soul;
        public int Combat;
        public int Magic;
        public int Stealth;
        public int AttackMin1;
        public int AttackMax1;
        public int AttackMin2;
        public int AttackMax2;
        public int AttackMin3;
        public int AttackMax3;
        public int Gold;

        public NPDTField(UnityBinaryReader r, uint dataSize)
        {
            Type = r.readLEInt32();
            Level = r.readLEInt32();
            Strength = r.readLEInt32();
            Intelligence = r.readLEInt32();
            Willpower = r.readLEInt32();
            Agility = r.readLEInt32();
            Speed = r.readLEInt32();
            Endurance = r.readLEInt32();
            Personality = r.readLEInt32();
            Luck = r.readLEInt32();
            Health = r.readLEInt32();
            SpellPts = r.readLEInt32();
            Fatigue = r.readLEInt32();
            Soul = r.readLEInt32();
            Combat = r.readLEInt32();
            Magic = r.readLEInt32();
            Stealth = r.readLEInt32();
            AttackMin1 = r.readLEInt32();
            AttackMax1 = r.readLEInt32();
            AttackMin2 = r.readLEInt32();
            AttackMax2 = r.readLEInt32();
            AttackMin3 = r.readLEInt32();
            AttackMax3 = r.readLEInt32();
            Gold = r.readLEInt32();
        }
    }

    public struct AIDTField
    {
        public enum AIFlags : uint
        {
            Weapon = 0x00001,
            Armor = 0x00002,
            Clothing = 0x00004,
            Books = 0x00008,
            Ingrediant = 0x00010,
            Picks = 0x00020,
            Probes = 0x00040,
            Lights = 0x00080,
            Apparatus = 0x00100,
            Repair = 0x00200,
            Misc = 0x00400,
            Spells = 0x00800,
            MagicItems = 0x01000,
            Potions = 0x02000,
            Training = 0x04000,
            Spellmaking = 0x08000,
            Enchanting = 0x10000,
            RepairItem = 0x20000
        }

        public byte Hello;
        public byte Unknown1;
        public byte Fight;
        public byte Flee;
        public byte Alarm;
        public byte Unknown2;
        public byte Unknown3;
        public byte Unknown4;
        public uint Flags;

        public AIDTField(UnityBinaryReader r, uint dataSize)
        {
            Hello = r.readByte();
            Unknown1 = r.readByte();
            Fight = r.readByte();
            Flee = r.readByte();
            Alarm = r.readByte();
            Unknown2 = r.readByte();
            Unknown3 = r.readByte();
            Unknown4 = r.readByte();
            Flags = r.readLEUInt32();
        }
    }

    public struct AI_WField
    {
        public short Distance;
        public short Duration;
        public byte TimeOfDay;
        public byte[] Idle;
        public byte Unknown;

        public AI_WField(UnityBinaryReader r, uint dataSize, byte mode)
        {
            Distance = r.readLEInt16();
            Duration = r.readLEInt16();
            TimeOfDay = r.readByte();
            Idle = r.readBytes(8);
            Unknown = r.readByte();
        }
    }

    public struct AI_TField
    {
        public float X;
        public float Y;
        public float Z;
        public float Unknown;

        public AI_TField(UnityBinaryReader r, uint dataSize)
        {
            X = r.readLESingle();
            Y = r.readLESingle();
            Z = r.readLESingle();
            Unknown = r.readLESingle();
        }
    }

    public struct AI_FField
    {
        public float X;
        public float Y;
        public float Z;
        public short Duration;
        public string Id;
        public short Unknown;

        public AI_FField(UnityBinaryReader r, uint dataSize)
        {
            X = r.readLESingle();
            Y = r.readLESingle();
            Z = r.readLESingle();
            Duration = r.readLEInt16();
            Id = r.readASCIIString(32, ASCIIFormat.ZeroPadded);
            Unknown = r.readLEInt16();
        }
    }

    public struct AI_AField
    {
        public string Name;
        public byte Unknown;

        public AI_AField(UnityBinaryReader r, uint dataSize)
        {
            Name = r.readASCIIString(32, ASCIIFormat.ZeroPadded);
            Unknown = r.readByte();
        }
    }

    public var description: String { return "CREA: \(EDID)" }
    public STRVField EDID  // Editor ID
    public MODLGroup MODL  // NIF Model
    public STRVField FNAM // Creature name
    public NPDTField NPDT // Creature data
    public IN32Field FLAG // Creature Flags
    public FMIDField<SCPTRecord> SCRI // Script
    public CNTOField NPCO // Item record
    public AIDTField AIDT // AI data
    public AI_WField AI_W // AI Wander
    public AI_TField? AI_T // AI Travel
    public AI_FField? AI_F // AI Follow
    public AI_FField? AI_E // AI Escort
    public AI_AField? AI_A // AI Activate
    public FLTVField? XSCL // Scale (optional), Only present if the scale is not 1.0
    public STRVField? CNAM;
    public List<STRVField> NPCSs = List<STRVField>();

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3) else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = STRVField(r, dataSize)
        case "NPDT": NPDT = NPDTField(r, dataSize)
        case "FLAG": FLAG = IN32Field(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "NPCO": NPCO = CNTOField(r, dataSize, formatId)
        case "AIDT": AIDT = AIDTField(r, dataSize)
        case "AI_W": AI_W = AI_WField(r, dataSize, 0)
        case "AI_T": AI_T = AI_TField(r, dataSize)
        case "AI_F": AI_F = AI_FField(r, dataSize)
        case "AI_E": AI_E = AI_FField(r, dataSize)
        case "AI_A": AI_A = AI_AField(r, dataSize)
        case "XSCL": XSCL = FLTVField(r, dataSize)
        case "CNAM": CNAM = STRVField(r, dataSize)
        case "NPCS": NPCSs.append(STRVField(r, dataSize, ASCIIFormat.ZeroPadded))
        default: return false
        }
        return true
    }
}