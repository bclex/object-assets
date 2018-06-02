//
//  QUSTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class QUSTRecord: Record {
    public struct DATAField
    {
        public byte Flags;
        public byte Priority;

        public DATAField(UnityBinaryReader r, uint dataSize)
        {
            Flags = r.ReadByte();
            Priority = r.ReadByte();
        }
    }

    public override string ToString() => $"QUST: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FULL; // Item Name
    public FILEField ICON; // Icon
    public DATAField DATA; // Icon
    public FMIDField<SCPTRecord> SCRI; // Script Name
    public SCPTRecord.SCHRField SCHR; // Script Data
    public BYTVField SCDA; // Compiled Script
    public STRVField SCTX; // Script Source
    public List<FMIDField<Record>> SCROs = new List<FMIDField<Record>>(); // Global variable reference

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "FULL": FULL = new STRVField(r, dataSize); return true;
            case "ICON": ICON = new FILEField(r, dataSize); return true;
            case "DATA": DATA = new DATAField(r, dataSize); return true;
            case "SCRI": SCRI = new FMIDField<SCPTRecord>(r, dataSize); return true;
            case "CTDA": r.ReadBytes((int)dataSize); return true;
            case "INDX": r.ReadBytes((int)dataSize); return true;
            case "QSDT": r.ReadBytes((int)dataSize); return true;
            case "CNAM": r.ReadBytes((int)dataSize); return true;
            case "QSTA": r.ReadBytes((int)dataSize); return true;
            case "SCHR": SCHR = new SCPTRecord.SCHRField(r, dataSize); return true;
            case "SCDA": SCDA = new BYTVField(r, dataSize); return true;
            case "SCTX": SCTX = new STRVField(r, dataSize); return true;
            case "SCRO": SCROs.Add(new FMIDField<Record>(r, dataSize)); return true;
            default: return false;
        }
    }
}
