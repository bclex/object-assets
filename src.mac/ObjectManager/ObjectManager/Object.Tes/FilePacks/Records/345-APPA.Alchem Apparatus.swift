//
//  APPARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class APPARecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField
    {
        public byte Type; // 0 = Mortar and Pestle, 1 = Albemic, 2 = Calcinator, 3 = Retort
        public float Quality;
        public float Weight;
        public int Value;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Type = (byte)r.ReadLEInt32();
                Quality = r.ReadLESingle();
                Weight = r.ReadLESingle();
                Value = r.ReadLEInt32();
                return;
            }
            Type = r.ReadByte();
            Value = r.ReadLEInt32();
            Weight = r.ReadLESingle();
            Quality = r.ReadLESingle();
        }
    }

    public var description: String { return "APPA: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Model Name
    public STRVField FULL; // Item Name
    public DATAField DATA; // Alchemy Data
    public FILEField ICON; // Inventory Icon
    public FMIDField<SCPTRecord> SCRI; // Script Name

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
        case "AADT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}