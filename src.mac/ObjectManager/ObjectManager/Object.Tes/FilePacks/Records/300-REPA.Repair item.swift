//
//  REPARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class REPARecord: Record, IHaveEDID, IHaveMODL {
    public struct RIDTField
    {
        public float Weight;
        public int Value;
        public int Uses;
        public float Quality;

        public RIDTField(UnityBinaryReader r, uint dataSize)
        {
            Weight = r.ReadLESingle();
            Value = r.ReadLEInt32();
            Uses = r.ReadLEInt32();
            Quality = r.ReadLESingle();
        }
    }

    public var description: String { return "REPA: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Model Name
    public STRVField FNAM; // Item Name
    public RIDTField RIDT; // Repair Data
    public FILEField ICON; // Inventory Icon
    public FMIDField<SCPTRecord> SCRI; // Script Name

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = STRVField(r, dataSize)
        case "RIDT": RIDT = RIDTField(r, dataSize)
        case "ITEX": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}