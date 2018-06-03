//
//  BOOKRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BOOKRecord: Record, IHaveEDID, IHaveMODL {
    public struct DATAField
    {
        public byte Flags //: Scroll - (1 is scroll, 0 not)
        public byte Teaches //: SkillId - (-1 is no skill)
        public int Value;
        public float Weight;
        //
        public int EnchantPts;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Weight = r.readLESingle();
                Value = r.readLEInt32();
                Flags = (byte)r.readLEInt32();
                Teaches = (byte)r.readLEInt32();
                EnchantPts = r.readLEInt32();
                return;
            }
            Flags = r.readByte();
            Teaches = r.readByte();
            Value = r.readLEInt32();
            Weight = r.readLESingle();
            EnchantPts = 0;
        }
    }

    public var description: String { return "BOOK: \(EDID)" }
    public STRVField EDID  // Editor ID
    public MODLGroup MODL  // Model (optional)
    public STRVField FULL // Item Name
    public DATAField DATA // Book Data
    public STRVField DESC // Book Text
    public FILEField ICON // Inventory Icon (optional)
    public FMIDField<SCPTRecord> SCRI // Script Name (optional)
    public FMIDField<ENCHRecord> ENAM // Enchantment FormId (optional)
    // TES4
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
        case "FULL":
        case "FNAM": FULL = STRVField(r, dataSize)
        case "DATA",
                "BKDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
                "ITEX": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "DESC",
                "TEXT": DESC = STRVField(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "ANAM": ANAM = IN16Field(r, dataSize)
        default: return false
        }
        return true
    }
}
