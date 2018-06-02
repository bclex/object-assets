//
//  KEYMRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class KEYMRecord: Record {
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

    public var description: String { return "KEYM: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var MODL: MODLGroup // Model
    public var FULL: STRVField // Item Name
    public var SCRI: FMIDField<SCPTRecord> // Script (optional)
    public var DATA: DATAField // Type of soul contained in the gem
    public var ICON: FILEField // Icon (optional)

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "MODL": MODL = new MODLGroup(r, dataSize); return true;
            case "MODB": MODL.MODBField(r, dataSize); return true;
            case "MODT": MODL.MODTField(r, dataSize); return true;
            case "FULL": FULL = new STRVField(r, dataSize); return true;
            case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
            case "DATA": DATA = new DATAField(r, dataSize); return true;
            case "ICON": ICON = new FILEField(r, dataSize); return true;
            default: return false;
        }
    }
}
