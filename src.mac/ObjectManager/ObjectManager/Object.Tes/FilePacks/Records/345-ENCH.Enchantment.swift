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
        public int Type // TES3: 0 = Cast Once, 1 = Cast Strikes, 2 = Cast when Used, 3 = Constant Effect
                            // TES4: 0 = Scroll, 1 = Staff, 2 = Weapon, 3 = Apparel
        public int EnchantCost;
        public int ChargeAmount //: Charge
        public int Flags //: AutoCalc

        public ENITField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Type = r.readLEInt32();
            if (formatId == GameFormatId.TES3)
            {
                EnchantCost = r.readLEInt32();
                ChargeAmount = r.readLEInt32();
            }
            else
            {
                ChargeAmount = r.readLEInt32();
                EnchantCost = r.readLEInt32();
            }
            Flags = r.readLEInt32();
        }
    }

    public class EFITField
    {
        public string EffectId;
        public int Type //:RangeType - 0 = Self, 1 = Touch, 2 = Target
        public int Area;
        public int Duration;
        public int MagnitudeMin;
        // TES3
        public byte SkillId // (-1 if NA)
        public byte AttributeId // (-1 if NA)
        public int MagnitudeMax;
        // TES4
        public int ActorValue;

        public EFITField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                EffectId = r.readASCIIString(2);
                SkillId = r.readByte();
                AttributeId = r.readByte();
                Type = r.readLEInt32();
                Area = r.readLEInt32();
                Duration = r.readLEInt32();
                MagnitudeMin = r.readLEInt32();
                MagnitudeMax = r.readLEInt32();
                return;
            }
            EffectId = r.readASCIIString(4);
            MagnitudeMin = r.readLEInt32();
            Area = r.readLEInt32();
            Duration = r.readLEInt32();
            Type = r.readLEInt32();
            ActorValue = r.readLEInt32();
        }
    }

    // TES4
    public class SCITField
    {
        public string Name;
        public int ScriptFormId;
        public int School // 0 = Alteration, 1 = Conjuration, 2 = Destruction, 3 = Illusion, 4 = Mysticism, 5 = Restoration
        public string VisualEffect;
        public uint Flags;

        public SCITField(UnityBinaryReader r, uint dataSize)
        {
            Name = "Script Effect";
            ScriptFormId = r.readLEInt32();
            if (dataSize == 4)
                return;
            School = r.readLEInt32();
            VisualEffect = r.readASCIIString(4);
            Flags = dataSize > 12 ? r.readLEUInt32() : 0;
        }

        public void FULLField(UnityBinaryReader r, uint dataSize)
        {
            Name = r.readASCIIString((int)dataSize, ASCIIFormat.PossiblyNullTerminated);
        }
    }

    public var description: String { return "ENCH: \(EDID)" }
    public STRVField EDID  // Editor ID
    public STRVField FULL // Enchant name
    public ENITField ENIT // Enchant Data
    public List<EFITField> EFITs = List<EFITField>() // Effect Data
    // TES4
    public List<SCITField> SCITs = List<SCITField>() // Script effect data

    init() {
    }

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
