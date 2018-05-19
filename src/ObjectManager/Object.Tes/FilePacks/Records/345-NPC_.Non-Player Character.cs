﻿using OA.Core;
using System;
using System.Collections.Generic;

namespace OA.Tes.FilePacks.Records
{
    public class NPC_Record : Record, IHaveEDID, IHaveMODL
    {
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
                    Level = r.ReadLEInt16();
                    Strength = r.ReadByte();
                    Intelligence = r.ReadByte();
                    Willpower = r.ReadByte();
                    Agility = r.ReadByte();
                    Speed = r.ReadByte();
                    Endurance = r.ReadByte();
                    Personality = r.ReadByte();
                    Luck = r.ReadByte();
                    Skills = r.ReadBytes(27);
                    Reputation = r.ReadByte();
                    Health = r.ReadLEInt16();
                    SpellPts = r.ReadLEInt16();
                    Fatigue = r.ReadLEInt16();
                    Disposition = r.ReadByte();
                    FactionId = r.ReadByte();
                    Rank = r.ReadByte();
                    Unknown1 = r.ReadByte();
                    Gold = r.ReadLEInt32();
                }
                else
                {
                    Level = r.ReadLEInt16();
                    Disposition = r.ReadByte();
                    FactionId = r.ReadByte();
                    Rank = r.ReadByte();
                    Unknown1 = r.ReadByte();
                    Unknown2 = r.ReadByte();
                    Unknown3 = r.ReadByte();
                    Gold = r.ReadLEInt32();
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
                XPos = r.ReadLESingle();
                YPos = r.ReadLESingle();
                ZPos = r.ReadLESingle();
                XRot = r.ReadLESingle();
                YRot = r.ReadLESingle();
                ZRot = r.ReadLESingle();
            }
        }

        public override string ToString() => $"NPC_: {EDID.Value}";
        public STRVField EDID { get; set; } // NPC ID
        public STRVField FNAM; // NPC name
        public FILEField MODL { get; set; } // Animation
        public STRVField RNAM; // Race Name
        public STRVField ANAM; // Faction name
        public STRVField BNAM; // Head model
        public STRVField CNAM; // Class name
        public STRVField KNAM; // Hair model
        public NPDTField NPDT; // NPC Data
        public INTVField FLAG; // NPC Flags
        public List<NPCOField> NPCOs = new List<NPCOField>(); // NPC item
        public List<STRVField> NPCSs = new List<STRVField>(); // NPC spell
        public CREARecord.AIDTField AIDT; // AI data
        public CREARecord.AI_WField? AI_W; // AI
        public CREARecord.AI_TField? AI_T; // AI Travel
        public CREARecord.AI_FField? AI_F; // AI Follow
        public CREARecord.AI_FField? AI_E; // AI Escort
        public STRVField? CNDT; // Cell escort/follow to string (optional)
        public CREARecord.AI_AField? AI_A; // AI Activate
        public DODTField DODT; // Cell Travel Destination
        public STRVField DNAM; // Cell name for previous DODT, if interior
        public FLTVField? XSCL; // Scale (optional) Only present if the scale is not 1.0
        public STRVField? SCRI; // Unknown

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (formatId == GameFormatId.Tes3)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                    case "MODL": MODL = new FILEField(r, dataSize); return true;
                    case "RNAM": RNAM = new STRVField(r, dataSize); return true;
                    case "ANAM": ANAM = new STRVField(r, dataSize); return true;
                    case "BNAM": BNAM = new STRVField(r, dataSize); return true;
                    case "CNAM": CNAM = new STRVField(r, dataSize); return true;
                    case "KNAM": KNAM = new STRVField(r, dataSize); return true;
                    case "NPDT": NPDT = new NPDTField(r, dataSize); return true;
                    case "FLAG": FLAG = new INTVField(r, dataSize); return true;
                    case "NPCO": NPCOs.Add(new NPCOField(r, dataSize)); return true;
                    case "NPCS": NPCSs.Add(new STRVField(r, dataSize, ASCIIFormat.ZeroPadded)); return true;
                    case "AIDT": AIDT = new CREARecord.AIDTField(r, dataSize); return true;
                    case "AI_W": AI_W = new CREARecord.AI_WField(r, dataSize, 1); return true;
                    case "AI_T": AI_T = new CREARecord.AI_TField(r, dataSize); return true;
                    case "AI_F": AI_F = new CREARecord.AI_FField(r, dataSize); return true;
                    case "AI_E": AI_E = new CREARecord.AI_FField(r, dataSize); return true;
                    case "CNDT": CNDT = new STRVField(r, dataSize); return true;
                    case "AI_A": AI_A = new CREARecord.AI_AField(r, dataSize); return true;
                    case "DODT": DODT = new DODTField(r, dataSize); return true;
                    case "DNAM": DNAM = new STRVField(r, dataSize); return true;
                    case "XSCL": XSCL = new FLTVField(r, dataSize); return true;
                    case "SCRI": SCRI = new STRVField(r, dataSize); return true;
                    default: return false;
                }
            return false;
        }
    }
}