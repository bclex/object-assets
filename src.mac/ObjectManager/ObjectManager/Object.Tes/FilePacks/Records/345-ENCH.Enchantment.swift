//
//  ENCHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ENCHRecord: Record {
    // TESX
    public struct ENITField
    {
        public int Type; // TES3: 0 = Cast Once, 1 = Cast Strikes, 2 = Cast when Used, 3 = Constant Effect
                            // TES4: 0 = Scroll, 1 = Staff, 2 = Weapon, 3 = Apparel
        public int EnchantCost;
        public int ChargeAmount; //: Charge
        public int Flags; //: AutoCalc

        public ENITField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Type = r.ReadLEInt32();
            if (formatId == GameFormatId.TES3)
            {
                EnchantCost = r.ReadLEInt32();
                ChargeAmount = r.ReadLEInt32();
            }
            else
            {
                ChargeAmount = r.ReadLEInt32();
                EnchantCost = r.ReadLEInt32();
            }
            Flags = r.ReadLEInt32();
        }
    }

    public class EFITField
    {
        public string EffectId;
        public int Type; //:RangeType - 0 = Self, 1 = Touch, 2 = Target
        public int Area;
        public int Duration;
        public int MagnitudeMin;
        // TES3
        public byte SkillId; // (-1 if NA)
        public byte AttributeId; // (-1 if NA)
        public int MagnitudeMax;
        // TES4
        public int ActorValue;

        public EFITField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                EffectId = r.ReadASCIIString(2);
                SkillId = r.ReadByte();
                AttributeId = r.ReadByte();
                Type = r.ReadLEInt32();
                Area = r.ReadLEInt32();
                Duration = r.ReadLEInt32();
                MagnitudeMin = r.ReadLEInt32();
                MagnitudeMax = r.ReadLEInt32();
                return;
            }
            EffectId = r.ReadASCIIString(4);
            MagnitudeMin = r.ReadLEInt32();
            Area = r.ReadLEInt32();
            Duration = r.ReadLEInt32();
            Type = r.ReadLEInt32();
            ActorValue = r.ReadLEInt32();
        }
    }

    // TES4
    public class SCITField
    {
        public string Name;
        public int ScriptFormId;
        public int School; // 0 = Alteration, 1 = Conjuration, 2 = Destruction, 3 = Illusion, 4 = Mysticism, 5 = Restoration
        public string VisualEffect;
        public uint Flags;

        public SCITField(UnityBinaryReader r, uint dataSize)
        {
            Name = "Script Effect";
            ScriptFormId = r.ReadLEInt32();
            if (dataSize == 4)
                return;
            School = r.ReadLEInt32();
            VisualEffect = r.ReadASCIIString(4);
            Flags = dataSize > 12 ? r.ReadLEUInt32() : 0;
        }

        public void FULLField(UnityBinaryReader r, uint dataSize)
        {
            Name = r.ReadASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }
    }

    public var description: String { return "ENCH: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FULL; // Enchant name
    public ENITField ENIT; // Enchant Data
    public List<EFITField> EFITs = new List<EFITField>(); // Effect Data
    // TES4
    public List<SCITField> SCITs = new List<SCITField>(); // Script effect data

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FULL": if SCITs.count == 0 { FULL = STRVField(r, dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "ENIT",
             "ENDT": ENIT = ENITField(r, dataSize, format)
        case "EFID": r.skipBytes(dataSize)
        case "EFIT",
             "ENAM": EFITs.append(EFITField(r, dataSize, format))
        case "SCIT": SCITs.append(SCITField(r, dataSize))
        default: return false
        }
        return true
    }
}
