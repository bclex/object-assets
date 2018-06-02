//
//  HAIRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class HAIRRecord: Record {
    public var description: String { return "HAIR: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var FULL: STRVField
    public var MODL: MODLGroup
    public var ICON: FILEField
    public var DATA: BYTEField // Playable, Not Male, Not Female, Fixed

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "FULL": FULL = new STRVField(r, dataSize); return true;
            case "MODL": MODL = new MODLGroup(r, dataSize); return true;
            case "MODB": MODL.MODBField(r, dataSize); return true;
            case "ICON": ICON = new FILEField(r, dataSize); return true;
            case "DATA": DATA = new BYTEField(r, dataSize); return true;
            default: return false;
        }
    }
}
