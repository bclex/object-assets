//
//  SGSTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SGSTRecord: Record {
    public class DATAField
    {
        public byte Uses;
        public int Value;
        public float Weight;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Uses = r.ReadByte();
            Value = r.ReadLEInt32();
            Weight = r.ReadLESingle();
        }
    }

    public override string ToString() => $"SGST: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Model
    public STRVField FULL; // Item Name
    public DATAField DATA; // Sigil Stone Data
    public FILEField ICON; // Icon
    public FMIDField<SCPTRecord>? SCRI; // Script (optional)
    public List<ENCHRecord.EFITField> EFITs = new List<ENCHRecord.EFITField>(); // Effect Data
    public List<ENCHRecord.SCITField> SCITs = new List<ENCHRecord.SCITField>(); // Script Effect Data

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "MODL": MODL = new MODLGroup(r, dataSize); return true;
            case "MODB": MODL.MODBField(r, dataSize); return true;
            case "MODT": MODL.MODTField(r, dataSize); return true;
            case "FULL": if (SCITs.Count == 0) FULL = new STRVField(r, dataSize); else ArrayUtils.Last(SCITs).FULLField(r, dataSize); return true;
            case "DATA": DATA = new DATAField(r, dataSize, formatId); return true;
            case "ICON": ICON = new FILEField(r, dataSize); return true;
            case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
            case "EFID": r.ReadBytes((int)dataSize); return true;
            case "EFIT": EFITs.Add(new ENCHRecord.EFITField(r, dataSize, formatId)); return true;
            case "SCIT": SCITs.Add(new ENCHRecord.SCITField(r, dataSize)); return true;
            default: return false;
        }
    }
}