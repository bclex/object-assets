//
//  PROBRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class PROBRecord: Record, IHaveEDID, IHaveMODL {
    public struct PBDTField
    {
        public float Weight;
        public int Value;
        public float Quality;
        public int Uses;

        public PBDTField(UnityBinaryReader r, uint dataSize)
        {
            Weight = r.ReadLESingle();
            Value = r.ReadLEInt32();
            Quality = r.ReadLESingle();
            Uses = r.ReadLEInt32();
        }
    }

    public override string ToString() => $"PROB: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Model Name
    public STRVField FNAM; // Item Name
    public PBDTField PBDT; // Probe Data
    public FILEField ICON; // Inventory Icon
    public FMIDField<SCPTRecord> SCRI; // Script Name

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        if (formatId == GameFormatId.TES3)
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "PBDT": PBDT = new PBDTField(r, dataSize); return true;
                case "ITEX": ICON = new FILEField(r, dataSize); return true;
                case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
                default: return false;
            }
        return false;
    }
}