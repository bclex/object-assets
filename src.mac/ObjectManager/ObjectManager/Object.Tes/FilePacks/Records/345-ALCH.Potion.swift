//
//  ALCHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ALCHRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public class DATAField
    {
        public float Weight;
        public int Value;
        public int Flags; //: AutoCalc

        init(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Weight = r.ReadLESingle();
            if (formatId == GameFormatId.TES3)
            {
                Value = r.ReadLEInt32();
                Flags = r.ReadLEInt32();
            }
        }

        public void ENITField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt32();
            Flags = r.ReadByte();
            r.skipBytes(3); // Unknown
        }
    }

    // TES3
    public struct ENAMField
    {
        public short EffectId;
        public byte SkillId; // for skill related effects, -1/0 otherwise
        public byte AttributeId; // for attribute related effects, -1/0 otherwise
        public int Unknown1;
        public int Unknown2;
        public int Duration;
        public int Magnitude;
        public int Unknown4;

        public ENAMField(UnityBinaryReader r, uint dataSize)
        {
            EffectId = r.ReadLEInt16();
            SkillId = r.ReadByte();
            AttributeId = r.ReadByte();
            Unknown1 = r.ReadLEInt32();
            Unknown2 = r.ReadLEInt32();
            Duration = r.ReadLEInt32();
            Magnitude = r.ReadLEInt32();
            Unknown4 = r.ReadLEInt32();
        }
    }

    public var description: String { return "ALCH: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Model
    public STRVField FULL; // Item Name
    public DATAField DATA; // Alchemy Data
    public ENAMField? ENAM; // Enchantment
    public FILEField ICON; // Icon
    public FMIDField<SCPTRecord>? SCRI; // Script (optional)
    // TES4
    public List<ENCHRecord.EFITField> EFITs = new List<ENCHRecord.EFITField>(); // Effect Data
    public List<ENCHRecord.SCITField> SCITs = new List<ENCHRecord.SCITField>(); // Script Effect Data

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL": if SCITs.Count == 0 { FULL = STRVField(r, dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "FNAM": FULL = STRVField(r, dataSize)
        case "DATA":
        case "ALDT": DATA = DATAField(r, dataSize, format)
        case "ENAM": ENAM = ENAMField(r, dataSize)
        case "ICON":
        case "TEXT": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        //
        case "ENIT": DATA.ENITField(r, dataSize)
        case "EFID": r.skipBytes(dataSize)
        case "EFIT": EFITs.append(ENCHRecord.EFITField(r, dataSize, format))
        case "SCIT": SCITs.append(ENCHRecord.SCITField(r, dataSize))
        default: return false
        }
        return true
    }
}