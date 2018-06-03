//
//  MISCRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class MISCRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField
    {
        public float Weight;
        public uint Value;
        public uint Unknown;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Weight = r.readLESingle();
                Value = r.readLEUInt32();
                Unknown = r.readLEUInt32();
                return;
            }
            Value = r.readLEUInt32();
            Weight = r.readLESingle();
            Unknown = 0;
        }
    }

    public var description: String { return "MISC: \(EDID)" }
    public STRVField EDID  // Editor ID
    public MODLGroup MODL  // Model
    public STRVField FULL // Item Name
    public DATAField DATA // Misc Item Data
    public FILEField ICON // Icon (optional)
    public FMIDField<SCPTRecord> SCRI // Script FormID (optional)
    // TES3
    public FMIDField<ENCHRecord> ENAM // enchantment ID

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
             "MCDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
