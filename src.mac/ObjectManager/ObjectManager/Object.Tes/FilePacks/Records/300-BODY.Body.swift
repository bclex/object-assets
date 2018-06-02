//
//  BODYRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class BODYRecord: Record {
    public struct BYDTField
    {
        public byte Part;
        public byte Vampire;
        public byte Flags;
        public byte PartType;

        public BYDTField(UnityBinaryReader r, uint dataSize)
        {
            Part = r.ReadByte();
            Vampire = r.ReadByte();
            Flags = r.ReadByte();
            PartType = r.ReadByte();
        }
    }

    public override string ToString() => $"BODY: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // NIF Model
    public STRVField FNAM; // Body name
    public BYDTField BYDT;

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        if (formatId == GameFormatId.TES3)
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "MODL": MODL = new MODLGroup(r, dataSize); return true;
                case "FNAM": FNAM = new STRVField(r, dataSize); return true;
                case "BYDT": BYDT = new BYDTField(r, dataSize); return true;
                default: return false;
            }
        return false;
    }
}
