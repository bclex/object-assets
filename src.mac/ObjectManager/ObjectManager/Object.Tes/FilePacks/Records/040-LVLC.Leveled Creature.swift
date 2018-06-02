//
//  LVLCRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LVLCRecord: Record {
    public var description: String { return "LVLC: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var LVLD: BYTEField // Chance
    public var LVLF: BYTEField // Flags - 0x01 = Calculate from all levels <= player's level, 0x02 = Calculate for each item in count
    public var SCRI: FMIDField<SCPTRecord> // Script (optional)
    public var TNAM: FMIDField<CREARecord> // Creature Template (optional)
    public var LVLOs = [LVLIRecord.LVLOField]()

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "LVLD": LVLD = new BYTEField(r, dataSize); return true;
            case "LVLF": LVLF = new BYTEField(r, dataSize); return true;
            case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
            case "TNAM": TNAM = new FMIDField<CREARecord>(r, dataSize); return true;
            case "LVLO": LVLOs.Add(new LVLIRecord.LVLOField(r, dataSize)); return true;
            default: return false;
        }
    }
}
