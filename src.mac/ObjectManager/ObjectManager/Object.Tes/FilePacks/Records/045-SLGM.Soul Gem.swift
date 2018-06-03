//
//  SLGMRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SLGMRecord: Record {
    public struct DATAField
    {
        public int Value;
        public float Weight;

        public DATAField(UnityBinaryReader r, uint dataSize)
        {
            Value = r.ReadLEInt32();
            Weight = r.ReadLESingle();
        }
    }

    public var description: String { return "SLGM: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL; // Model
    public STRVField FULL; // Item Name
    public FMIDField<SCPTRecord> SCRI; // Script (optional)
    public DATAField DATA; // Type of soul contained in the gem
    public FILEField ICON; // Icon (optional)
    public BYTEField SOUL; // Type of soul contained in the gem
    public BYTEField SLCP; // Soul gem maximum capacity

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "SOUL": SOUL = BYTEField(r, dataSize)
        case "SLCP": SLCP = BYTEField(r, dataSize)
        default: return false
        }
        return true
    }
}
