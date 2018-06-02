//
//  FLORRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class FLORRecord: Record {
    public override string ToString() => $"FLOR: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL; // Model
    public STRVField FULL; // Plant Name
    public FMIDField<SCPTRecord> SCRI; // Script (optional)
    public FMIDField<INGRRecord> PFIG; // The ingredient the plant produces (optional)
    public BYTVField PFPC; // Spring, Summer, Fall, Winter Ingredient Production (byte)

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
            case "PFIG": PFIG = new FMIDField<INGRRecord>(r, dataSize); return true;
            case "PFPC": PFPC = new BYTVField(r, dataSize); return true;
            default: return false;
        }
    }
}
