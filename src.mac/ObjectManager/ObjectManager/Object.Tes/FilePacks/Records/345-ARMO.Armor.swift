//
//  ARMORecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ARMORecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField
    {
        public enum ARMOType
        {
            Helmet = 0,
            Cuirass = 1,
            L_Pauldron = 2,
            R_Pauldron = 3,
            Greaves = 4,
            Boots = 5,
            L_Gauntlet = 6,
            R_Gauntlet = 7,
            Shield = 8,
            L_Bracer = 9,
            R_Bracer = 10,
        }

        public short Armour;
        public int Value;
        public int Health;
        public float Weight;
        //
        public int Type;
        public int EnchantPts;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Type = r.ReadLEInt32();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                Health = r.ReadLEInt32();
                EnchantPts = r.ReadLEInt32();
                Armour = (short)r.ReadLEInt32();
                return;
            }
            Armour = r.ReadLEInt16();
            Value = r.ReadLEInt32();
            Health = r.ReadLEInt32();
            Weight = r.ReadLESingle();
            Type = 0;
            EnchantPts = 0;
        }
    }

    public var description: String { return "ARMO: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Male biped model
    public STRVField FULL; // Item Name
    public FILEField ICON; // Male icon
    public DATAField DATA; // Armour Data
    public FMIDField<SCPTRecord>? SCRI; // Script Name (optional)
    public FMIDField<ENCHRecord>? ENAM; // Enchantment FormId (optional)
    // TES3
    public List<CLOTRecord.INDXFieldGroup> INDXs = new List<CLOTRecord.INDXFieldGroup>(); // Body Part Index
    // TES4
    public UI32Field BMDT; // Flags
    public MODLGroup MOD2; // Male world model (optional)
    public MODLGroup MOD3; // Female biped (optional)
    public MODLGroup MOD4; // Female world model (optional)
    public FILEField? ICO2; // Female icon (optional)
    public IN16Field? ANAM; // Enchantment points (optional)

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "DATA":
        case "AODT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "INDX": INDXs.append(CLOTRecord.INDXFieldGroup(INDX: INTVField(r, dataSize)))
        case "BNAM": INDXs.last!.BNAM = STRVField(r, dataSize)
        case "CNAM": INDXs.last!.CNAM = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "BMDT": BMDT = UI32Field(r, dataSize)
        case "MOD2": MOD2 = MODLGroup(r, dataSize)
        case "MO2B": MOD2.MODBField(r, dataSize)
        case "MO2T": MOD2.MODTField(r, dataSize)
        case "MOD3": MOD3 = MODLGroup(r, dataSize)
        case "MO3B": MOD3.MODBField(r, dataSize)
        case "MO3T": MOD3.MODTField(r, dataSize)
        case "MOD4": MOD4 = MODLGroup(r, dataSize)
        case "MO4B": MOD4.MODBField(r, dataSize)
        case "MO4T": MOD4.MODTField(r, dataSize)
        case "ICO2": ICO2 = FILEField(r, dataSize)
        case "ANAM": ANAM = IN16Field(r, dataSize)
        default: return false
        }
        return true
    }
}