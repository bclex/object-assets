//
//  SKILRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SKILRecord: Record {
    // TESX
    public struct DATAField
    {
        public int Action;
        public int Attribute;
        public uint Specialization // 0 = Combat, 1 = Magic, 2 = Stealth
        public float[] UseValue // The use types for each skill are hard-coded.

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            Action = formatId == GameFormatId.TES3 ? 0 : r.readLEInt32();
            Attribute = r.readLEInt32();
            Specialization = r.readLEUInt32();
            UseValue = float[formatId == GameFormatId.TES3 ? 4 : 2];
            for (var i = 0; i < UseValue.Length; i++)
                UseValue[i] = r.readLESingle();
        }
    }

    public var description: String { return "SKIL: \(INDX):\(EDID)" }
    public STRVField EDID  // Editor ID
    public IN32Field INDX // Skill ID
    public DATAField DATA // Skill Data
    public STRVField DESC // Skill description
    // TES4
    public FILEField ICON // Icon
    public STRVField ANAM // Apprentice Text
    public STRVField JNAM // Journeyman Text
    public STRVField ENAM // Expert Text
    public STRVField MNAM // Master Text

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "INDX": INDX = IN32Field(r, dataSize)
        case "DATA",
             "SKDT": DATA = DATAField(r, dataSize, format)
        case "DESC": DESC = STRVField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "ANAM": ANAM = STRVField(r, dataSize)
        case "JNAM": JNAM = STRVField(r, dataSize)
        case "ENAM": ENAM = STRVField(r, dataSize)
        case "MNAM": MNAM = STRVField(r, dataSize)
        default: return false
        }
        return true
    }
}