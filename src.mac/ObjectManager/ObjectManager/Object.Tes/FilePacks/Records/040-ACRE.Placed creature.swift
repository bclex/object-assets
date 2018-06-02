//
//  ACRERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ACRERecord: Record {
    public var description: String { return "GMST: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var NAME: FMIDField<Record> // Base
    public var REFRRecord.DATAField DATA; // Position/Rotation
    public var XOWNs:[CELLRecord.XOWNGroup] // Ownership (optional)
    public var XESP: REFRRecord.XESPField? // Enable Parent (optional)
    public var XSCL: FLTVField? // Scale (optional)
    public var XRGD: BYTVField? // Ragdoll Data (optional)

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "NAME": NAME = new FMIDField<Record>(r, dataSize); return true;
            case "DATA": DATA = new REFRRecord.DATAField(r, dataSize); return true;
            case "XOWN": if (XOWNs == null) XOWNs = new List<CELLRecord.XOWNGroup>(); XOWNs.Add(new CELLRecord.XOWNGroup { XOWN = new FMIDField<Record>(r, dataSize) }); return true;
            case "XRNK": ArrayUtils.Last(XOWNs).XRNK = new IN32Field(r, dataSize); return true;
            case "XGLB": ArrayUtils.Last(XOWNs).XGLB = new FMIDField<Record>(r, dataSize); return true;
            case "XESP": XESP = new REFRRecord.XESPField(r, dataSize); return true;
            case "XSCL": XSCL = new FLTVField(r, dataSize); return true;
            case "XRGD": XRGD = new BYTVField(r, dataSize); return true;
            default: return false;
        }
    }
}
