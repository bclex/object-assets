//
//  INGRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class INGRRecord: Record, IHaveEDID, IHaveMODL {
    // TES3
    public struct IRDTField
    {
        public float Weight;
        public int Value;
        public int[] EffectId // 0 or -1 means no effect
        public int[] SkillId // only for Skill related effects, 0 or -1 otherwise
        public int[] AttributeId // only for Attribute related effects, 0 or -1 otherwise

        public IRDTField(UnityBinaryReader r, uint dataSize)
        {
            Weight = r.readLESingle();
            Value = r.readLEInt32();
            EffectId = int[4];
            for (var i = 0; i < EffectId.Length; i++)
                EffectId[i] = r.readLEInt32();
            SkillId = int[4];
            for (var i = 0; i < SkillId.Length; i++)
                SkillId[i] = r.readLEInt32();
            AttributeId = int[4];
            for (var i = 0; i < AttributeId.Length; i++)
                AttributeId[i] = r.readLEInt32();
        }
    }

    // TES4
    public class DATAField
    {
        public float Weight;
        public int Value;
        public uint Flags;

        public DATAField(UnityBinaryReader r, uint dataSize)
        {
            Weight = r.readLESingle();
        }

        public void ENITField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.readLEInt32();
            Flags = r.readLEUInt32();
        }
    }

    public var description: String { return "INGR: \(EDID)" }
    public STRVField EDID  // Editor ID
    public MODLGroup MODL  // Model Name
    public STRVField FULL // Item Name
    public IRDTField IRDT // Ingrediant Data //: TES3
    public DATAField DATA // Ingrediant Data //: TES4
    public FILEField ICON // Inventory Icon
    public FMIDField<SCPTRecord> SCRI // Script Name
    // TES4
    public List<ENCHRecord.EFITField> EFITs = List<ENCHRecord.EFITField>() // Effect Data
    public List<ENCHRecord.SCITField> SCITs = List<ENCHRecord.SCITField>() // Script effect data

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
                "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL": if SCITs.count == 0 { FULL = STRVField(r, dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "FNAM": FULL = STRVField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "IRDT": IRDT = IRDTField(r, dataSize)
        case "ICON",
                "ITEX": ICON = FILEField(r, dataSize)
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
