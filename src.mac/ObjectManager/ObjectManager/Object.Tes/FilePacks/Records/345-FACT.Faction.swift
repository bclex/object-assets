//
//  FACTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class FACTRecord: Record {
    // TESX
    public class RNAMGroup
    {
        public override string ToString() => $"{RNAM.Value}:{MNAM.Value}";
        public IN32Field RNAM; // rank
        public STRVField MNAM; // male
        public STRVField FNAM; // female
        public STRVField INAM; // insignia
    }

    // TES3
    public struct FADTField
    {
        public FADTField(UnityBinaryReader r, uint dataSize)
        {
            r.skipBytes(dataSize);
        }
    }

    // TES4
    public struct XNAMField
    {
        public override string ToString() => $"{FormId}";
        public int FormId;
        public int Mod;
        public int Combat;

        public XNAMField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            FormId = r.ReadLEInt32();
            Mod = r.ReadLEInt32();
            Combat = formatId > GameFormatId.TES4 ? r.ReadLEInt32() : 0; // 0 - Neutral, 1 - Enemy, 2 - Ally, 3 - Friend
        }
    }

    public var description: String { return "FACT: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public STRVField FNAM; // Faction name
    public List<RNAMGroup> RNAMs = new List<RNAMGroup>(); // Rank Name
    public FADTField FADT; // Faction data
    public List<STRVField> ANAMs = new List<STRVField>(); // Faction name
    public List<INTVField> INTVs = new List<INTVField>(); // Faction reaction
    // TES4
    public XNAMField XNAM; // Interfaction Relations
    public INTVField DATA; // Flags (byte, uint32)
    public UI32Field CNAM;

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if format == .TES3 {
            switch type {
            case "NAME": EDID = STRVField(r, dataSize)
            case "FNAM": FNAM = STRVField(r, dataSize)
            case "RNAM": RNAMs.append(RNAMGroup(MNAM: STRVField(r, dataSize)))
            case "FADT": FADT = FADTField(r, dataSize)
            case "ANAM": ANAMs.append(STRVField(r, dataSize))
            case "INTV": INTVs.append(INTVField(r, dataSize))
            default: return false
            }
            return true
        }
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "FULL": FNAM = STRVField(r, dataSize)
        case "XNAM": XNAM = XNAMField(r, dataSize, format)
        case "DATA": DATA = INTVField(r, dataSize)
        case "CNAM": CNAM = UI32Field(r, dataSize)
        case "RNAM": RNAMs.append(RNAMGroup(RNAM: IN32Field(r, dataSize)))
        case "MNAM": RNAMs.last!.MNAM = STRVField(r, dataSize)
        case "FNAM": RNAMs.last!.FNAM = STRVField(r, dataSize)
        case "INAM": RNAMs.last!.INAM = STRVField(r, dataSize)
        default: return false
        }
        return true
    }
}
