//
//  AACTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class AACTRecord: Record {
    public var description: String { return "AACT: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var CNAME: CREFField // RGB color

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "CNAME": CNAME = new CREFField(r, dataSize); return true;
            default: return false;
        }
    }
}
