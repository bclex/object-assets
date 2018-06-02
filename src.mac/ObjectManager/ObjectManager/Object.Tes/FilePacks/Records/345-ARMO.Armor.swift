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

    public override string ToString() => $"ARMO: {EDID.Value}";
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

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID":
            case "NAME": EDID = new STRVField(r, dataSize); return true;
            case "MODL": MODL = new MODLGroup(r, dataSize); return true;
            case "MODB": MODL.MODBField(r, dataSize); return true;
            case "MODT": MODL.MODTField(r, dataSize); return true;
            case "FULL":
            case "FNAM": FULL = new STRVField(r, dataSize); return true;
            case "DATA":
            case "AODT": DATA = new DATAField(r, dataSize, formatId); return true;
            case "ICON":
            case "ITEX": ICON = new FILEField(r, dataSize); return true;
            case "INDX": INDXs.Add(new CLOTRecord.INDXFieldGroup { INDX = new INTVField(r, dataSize) }); return true;
            case "BNAM": ArrayUtils.Last(INDXs).BNAM = new STRVField(r, dataSize); return true;
            case "CNAM": ArrayUtils.Last(INDXs).CNAM = new STRVField(r, dataSize); return true;
            case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
            case "ENAM": ENAM = new FMIDField<ENCHRecord>(r, dataSize); return true;
            case "BMDT": BMDT = new UI32Field(r, dataSize); return true;
            case "MOD2": MOD2 = new MODLGroup(r, dataSize); return true;
            case "MO2B": MOD2.MODBField(r, dataSize); return true;
            case "MO2T": MOD2.MODTField(r, dataSize); return true;
            case "MOD3": MOD3 = new MODLGroup(r, dataSize); return true;
            case "MO3B": MOD3.MODBField(r, dataSize); return true;
            case "MO3T": MOD3.MODTField(r, dataSize); return true;
            case "MOD4": MOD4 = new MODLGroup(r, dataSize); return true;
            case "MO4B": MOD4.MODBField(r, dataSize); return true;
            case "MO4T": MOD4.MODTField(r, dataSize); return true;
            case "ICO2": ICO2 = new FILEField(r, dataSize); return true;
            case "ANAM": ANAM = new IN16Field(r, dataSize); return true;
            default: return false;
        }
    }
}