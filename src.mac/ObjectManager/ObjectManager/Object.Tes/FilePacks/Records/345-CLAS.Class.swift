//
//  CLASRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CLASRecord: Record {
    public struct DATAField {
        //wbArrayS('Primary Attributes', wbInteger('Primary Attribute', itS32, wbActorValueEnum), 2),
        //wbInteger('Specialization', itU32, wbSpecializationEnum),
        //wbArrayS('Major Skills', wbInteger('Major Skill', itS32, wbActorValueEnum), 7),
        //wbInteger('Flags', itU32, wbFlags(['Playable', 'Guard'])),
        //wbInteger('Buys/Sells and Services', itU32, wbServiceFlags),
        //wbInteger('Teaches', itS8, wbSkillEnum),
        //wbInteger('Maximum training level', itU8),
        //wbInteger('Unused', itU16)

        init(_ r: BinaryReader, _ dataSize: Int) {
            r.skipBytes(dataSize)
        }
    }

    public override var description: String { return "CLAS: \(EDID)" }
    public var EDID: STRVField = STRVField.empty   // Editor ID
    public var FULL: STRVField!  // Name
    public var DESC: STRVField!  // Description
    public var ICON: STRVField? = nil // Icon (Optional)
    public var DATA: DATAField!  // Data

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
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
