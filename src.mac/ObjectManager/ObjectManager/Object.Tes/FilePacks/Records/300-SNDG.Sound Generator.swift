//
//  SNDGRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SNDGRecord: Record {
    public enum SNDGType : uint
    {
        LeftFoot = 0,
        RightFoot = 1,
        SwimLeft = 2,
        SwimRight = 3,
        Moan = 4,
        Roar = 5,
        Scream = 6,
        Land = 7,
    }

    public override string ToString() => $"SNDG: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public IN32Field DATA; // Sound Type Data
    public STRVField SNAM; // Sound ID
    public STRVField? CNAM; // Creature name (optional)

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        if (formatId == GameFormatId.TES3)
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new IN32Field(r, dataSize); return true;
                case "SNAM": SNAM = new STRVField(r, dataSize); return true;
                case "CNAM": CNAM = new STRVField(r, dataSize); return true;
                default: return false;
            }
        return false;
    }
}