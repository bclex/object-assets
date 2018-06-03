//
//  CLOTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CLOTRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField
    {
        public enum CLOTType
        {
            Pants = 0,
            Shoes = 1,
            Shirt = 2,
            Belt = 3,
            Robe = 4,
            R_Glove = 5,
            L_Glove = 6,
            Skirt = 7,
            Ring = 8,
            Amulet = 9,
        }

        public int Value;
        public float Weight;
        //
        public int Type;
        public short EnchantPts;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Type = r.readLEInt32();
                Weight = r.readLESingle();
                Value = r.readLEInt16();
                EnchantPts = r.readLEInt16();
                return;
            }
            Value = r.readLEInt32();
            Weight = r.readLESingle();
            Type = 0;
            EnchantPts = 0;
        }
    }

    public class INDXFieldGroup
    {
        public override string ToString() => $"{INDX.Value}: {BNAM.Value}";
        public INTVField INDX;
        public STRVField BNAM;
        public STRVField CNAM;
    }

    public var description: String { return "CLOT: \(EDID)" }
    public STRVField EDID  // Editor ID
    public MODLGroup MODL  // Model Name
    public STRVField FULL // Item Name
    public DATAField DATA // Clothing Data
    public FILEField ICON // Male Icon
    public STRVField ENAM // Enchantment Name
    public FMIDField<SCPTRecord> SCRI // Script Name
    // TES3
    public List<INDXFieldGroup> INDXs = List<INDXFieldGroup>() // Body Part Index (Moved to Race)
    // TES4
    public UI32Field BMDT // Clothing Flags
    public MODLGroup MOD2 // Male world model (optional)
    public MODLGroup MOD3 // Female biped (optional)
    public MODLGroup MOD4 // Female world model (optional)
    public FILEField? ICO2 // Female icon (optional)
    public IN16Field? ANAM // Enchantment points (optional)

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "DATA",
             "CTDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "INDX": INDXs.append(INDXFieldGroup(INDX: INTVField(r, dataSize)))
        case "BNAM": INDXs.last!.BNAM = STRVField(r, dataSize)
        case "CNAM": INDXs.last!.CNAM = STRVField(r, dataSize)
        case "ENAM": ENAM = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "BMDT": BMDT = UI32Field(r, dataSize)
        case "MOD2": MOD2 = MODLGroup(r, dataSize)
        case "MO2B": MOD2.MODBField(r, dataSize)
        case "MO2T": MOD2.MODTField(r, dataSize)
        case "MOD3": MOD3 = MODLGroup(r, dataSize)
        case "MO3B": MOD3.MODBField(r, dataSize)
        case "MO3T": MOD3.MODTField(r, dataSize)
        case "MOD4": MOD4 = MODLGroup(r, dataSize)
        case "MO4B": MOD4.MODBField(r, dataSize)
 
        case "ICO2": ICO2 = FILEField(r, dataSize)
        case "ANAM": ANAM = IN16Field(r, dataSize)
        default: return false
        }
        return true
    }
}