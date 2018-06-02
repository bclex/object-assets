//
//  CLASRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CLASRecord: Record {
    public struct DATAField
    {
        //wbArrayS('Primary Attributes', wbInteger('Primary Attribute', itS32, wbActorValueEnum), 2),
        //wbInteger('Specialization', itU32, wbSpecializationEnum),
        //wbArrayS('Major Skills', wbInteger('Major Skill', itS32, wbActorValueEnum), 7),
        //wbInteger('Flags', itU32, wbFlags(['Playable', 'Guard'])),
        //wbInteger('Buys/Sells and Services', itU32, wbServiceFlags),
        //wbInteger('Teaches', itS8, wbSkillEnum),
        //wbInteger('Maximum training level', itU8),
        //wbInteger('Unused', itU16)

        public DATAField(UnityBinaryReader r, uint dataSize)
        {
            r.ReadBytes((int)dataSize);
        }
    }

    public override string ToString() => $"CLAS: {EDID.Value}";
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FULL; // Name
    public STRVField DESC; // Description
    public STRVField? ICON; // Icon (Optional)
    public DATAField DATA; // Data

    public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
    {
        if (formatId == GameFormatId.TES3)
            switch (type)
            {
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "FNAM": FULL = new STRVField(r, dataSize); return true;
                case "CLDT": r.ReadBytes((int)dataSize); return true;
                case "DESC": DESC = new STRVField(r, dataSize); return true;
                default: return false;
            }
        switch (type)
        {
            case "EDID": EDID = new STRVField(r, dataSize); return true;
            case "FULL": FULL = new STRVField(r, dataSize); return true;
            case "DESC": DESC = new STRVField(r, dataSize); return true;
            case "ICON": ICON = new STRVField(r, dataSize); return true;
            case "DATA": DATA = new DATAField(r, dataSize); return true;
            default: return false;
        }
    }
}
