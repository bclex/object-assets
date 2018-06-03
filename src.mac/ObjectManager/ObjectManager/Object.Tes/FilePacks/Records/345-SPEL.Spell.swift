//
//  SPELRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SPELRecord: Record {
    // TESX
    public struct SPITField
    {
        public override string ToString() => $"{Type}";
        public uint Type; // TES3: 0 = Spell, 1 = Ability, 2 = Blight, 3 = Disease, 4 = Curse, 5 = Power
                            // TES4: 0 = Spell, 1 = Disease, 2 = Power, 3 = Lesser Power, 4 = Ability, 5 = Poison
        public int SpellCost;
        public uint Flags; // 0x0001 = AutoCalc, 0x0002 = PC Start, 0x0004 = Always Succeeds
        // TES4
        public int SpellLevel;

        public SPITField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Type = r.ReadLEUInt32();
            SpellCost = r.ReadLEInt32();
            SpellLevel = formatId != GameFormatId.TES3 ? r.ReadLEInt32() : 0;
            Flags = r.ReadLEUInt32();
        }
    }

    public var description: String { return "SPEL: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FULL; // Spell name
    public SPITField SPIT; // Spell data
    public List<ENCHRecord.EFITField> EFITs = new List<ENCHRecord.EFITField>(); // Effect Data
    // TES4
    public List<ENCHRecord.SCITField> SCITs = new List<ENCHRecord.SCITField>(); // Script effect data

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "FULL": if SCITs.count == 0 { FULL = STRVField(r, dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "FNAM": FULL = STRVField(r, dataSize)
        case "SPIT":
        case "SPDT": SPIT = SPITField(r, dataSize, format)
        case "EFID": r.skipBytes(dataSize)
        case "EFIT",
             "ENAM": EFITs.append(ENCHRecord.EFITField(r, dataSize, format))
        case "SCIT": SCITs.append(ENCHRecord.SCITField(r, dataSize))
        default: return false
        }
        return true
    }
}
