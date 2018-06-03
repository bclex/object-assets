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
            r.skipBytes((int)dataSize);
        }
    }

    public var description: String { return "CLAS: \(EDID)" }
    public STRVField EDID  // Editor ID
    public STRVField FULL // Name
    public STRVField DESC // Description
    public STRVField? ICON // Icon (Optional)
    public DATAField DATA // Data

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
    {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = STRVField(r, dataSize)
            case "FNAM": FULL = STRVField(r, dataSize)
            case "CLDT": r.skipBytes(dataSize)
            case "DESC": DESC = STRVField(r, dataSize)
            default: return false
            }
            return true
        }
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "DESC": DESC = STRVField(r, dataSize)
        case "ICON": ICON = STRVField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        default: return false
        }
        return true
    }
}
