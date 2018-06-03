//
//  NPC_Record.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class NPC_Record: Record, IHaveEDID, IHaveMODL {
    [Flags]
    public enum NPC_Flags : uint
    {
        Female = 0x0001,
        Essential = 0x0002,
        Respawn = 0x0004,
        None = 0x0008,
        Autocalc = 0x0010,
        BloodSkel = 0x0400,
        BloodMetal = 0x0800,
    }

    public class NPDTField
    {
        public short Level;
        public byte Strength;
        public byte Intelligence;
        public byte Willpower;
        public byte Agility;
        public byte Speed;
        public byte Endurance;
        public byte Personality;
        public byte Luck;
        public byte[] Skills;
        public byte Reputation;
        public short Health;
        public short SpellPts;
        public short Fatigue;
        public byte Disposition;
        public byte FactionId;
        public byte Rank;
        public byte Unknown1;
        public int Gold;

        // 12 byte version
        //public short Level;
        //public byte Disposition;
        //public byte FactionId;
        //public byte Rank;
        //public byte Unknown1;
        public byte Unknown2;
        public byte Unknown3;
        //public int Gold;

        public NPDTField(UnityBinaryReader r, uint dataSize)
        {
            if (dataSize == 52)
            {
                Level = r.readLEInt16();
                Strength = r.readByte();
                Intelligence = r.readByte();
                Willpower = r.readByte();
                Agility = r.readByte();
                Speed = r.readByte();
                Endurance = r.readByte();
                Personality = r.readByte();
                Luck = r.readByte();
                Skills = r.readBytes(27);
                Reputation = r.readByte();
                Health = r.readLEInt16();
                SpellPts = r.readLEInt16();
                Fatigue = r.readLEInt16();
                Disposition = r.readByte();
                FactionId = r.readByte();
                Rank = r.readByte();
                Unknown1 = r.readByte();
                Gold = r.readLEInt32();
            }
            else
            {
                Level = r.readLEInt16();
                Disposition = r.readByte();
                FactionId = r.readByte();
                Rank = r.readByte();
                Unknown1 = r.readByte();
                Unknown2 = r.readByte();
                Unknown3 = r.readByte();
                Gold = r.readLEInt32();
            }
        }
    }

    public struct DODTField
    {
        public float XPos;
        public float YPos;
        public float ZPos;
        public float XRot;
        public float YRot;
        public float ZRot;

        public DODTField(UnityBinaryReader r, uint dataSize)
        {
            XPos = r.readLESingle();
            YPos = r.readLESingle();
            ZPos = r.readLESingle();
            XRot = r.readLESingle();
            YRot = r.readLESingle();
            ZRot = r.readLESingle();
        }
    }

    public var description: String { return "NPC_: \(EDID)" }
    public STRVField EDID  // Editor ID
    public STRVField FULL // NPC name
    public MODLGroup MODL  // Animation
    public STRVField RNAM // Race Name
    public STRVField ANAM // Faction name
    public STRVField BNAM // Head model
    public STRVField CNAM // Class name
    public STRVField KNAM // Hair model
    public NPDTField NPDT // NPC Data
    public INTVField FLAG // NPC Flags
    public List<CNTOField> NPCOs = List<CNTOField>() // NPC item
    public List<STRVField> NPCSs = List<STRVField>() // NPC spell
    public CREARecord.AIDTField AIDT // AI data
    public CREARecord.AI_WField? AI_W // AI
    public CREARecord.AI_TField? AI_T // AI Travel
    public CREARecord.AI_FField? AI_F // AI Follow
    public CREARecord.AI_FField? AI_E // AI Escort
    public STRVField? CNDT // Cell escort/follow to string (optional)
    public CREARecord.AI_AField? AI_A // AI Activate
    public DODTField DODT // Cell Travel Destination
    public STRVField DNAM // Cell name for previous DODT, if interior
    public FLTVField? XSCL // Scale (optional) Only present if the scale is not 1.0
    public FMIDField<SCPTRecord>? SCRI // Unknown

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "RNAM": RNAM = STRVField(r, dataSize)
        case "ANAM": ANAM = STRVField(r, dataSize)
        case "BNAM": BNAM = STRVField(r, dataSize)
        case "CNAM": CNAM = STRVField(r, dataSize)
        case "KNAM": KNAM = STRVField(r, dataSize)
        case "NPDT": NPDT = NPDTField(r, dataSize)
        case "FLAG": FLAG = INTVField(r, dataSize)
        case "NPCO": NPCOs.append(CNTOField(r, dataSize, format))
        case "NPCS": NPCSs.append(STRVField(r, dataSize, .zeroPadded))
        case "AIDT": AIDT = CREARecord.AIDTField(r, dataSize)
        case "AI_W": AI_W = CREARecord.AI_WField(r, dataSize, 1)
        case "AI_T": AI_T = CREARecord.AI_TField(r, dataSize)
        case "AI_F": AI_F = CREARecord.AI_FField(r, dataSize)
        case "AI_E": AI_E = CREARecord.AI_FField(r, dataSize)
        case "CNDT": CNDT = STRVField(r, dataSize)
        case "AI_A": AI_A = CREARecord.AI_AField(r, dataSize)
        case "DODT": DODT = DODTField(r, dataSize)
        case "DNAM": DNAM = STRVField(r, dataSize)
        case "XSCL": XSCL = FLTVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
