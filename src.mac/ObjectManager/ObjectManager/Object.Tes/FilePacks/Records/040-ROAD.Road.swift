//
//  ROADRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ROADRecord: Record {
    public override string ToString() => $"ROAD:";
    public PGRDRecord.PGRPField[] PGRPs { get; set; }
    public UNKNField PGRR { get; set; }

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "PGRP": PGRPs = new PGRDRecord.PGRPField[dataSize >> 4]; for (var i = 0; i < PGRPs.Length; i++) PGRPs[i] = new PGRDRecord.PGRPField(r, dataSize); return true;
            case "PGRR": PGRR = new UNKNField(r, dataSize); return true;
            default: return false;
        }
    }
}
