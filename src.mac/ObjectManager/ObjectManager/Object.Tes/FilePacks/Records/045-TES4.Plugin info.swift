//
//  TES4Record.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TES4Record: Record {
    public struct HEDRField
    {
        public float Version;
        public int NumRecords; // Number of records and groups (not including TES4 record itself).
        public uint NextObjectId; // Next available object ID.

        public HEDRField(UnityBinaryReader r, uint dataSize)
        {
            Version = r.ReadLESingle();
            NumRecords = r.ReadLEInt32();
            NextObjectId = r.ReadLEUInt32();
        }
    }

    public HEDRField HEDR;
    public STRVField? CNAM; // author (Optional)
    public STRVField? SNAM; // description (Optional)
    public List<STRVField> MASTs; // master
    public List<INTVField> DATAs; // fileSize
    public UNKNField? ONAM; // overrides (Optional)
    public IN32Field INTV; // unknown
    public IN32Field? INCC; // unknown (Optional)

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "HEDR": HEDR = HEDRField(r, dataSize)
        case "OFST": r.skipBytes(dataSize)
        case "DELE": r.skipBytes(dataSize)
        case "CNAM": CNAM = STRVField(r, dataSize)
        case "SNAM": SNAM = STRVField(r, dataSize)
        case "MAST": if MASTs == nil { MASTs = [STRVField]() } MASTs.append(STRVField(r, dataSize))
        case "DATA": if DATAs == nil { DATAs = [INTVField]() } DATAs.append(INTVField(r, dataSize))
        case "ONAM": ONAM = UNKNField(r, dataSize)
        case "INTV": INTV = IN32Field(r, dataSize)
        case "INCC": INCC = IN32Field(r, dataSize)
        default: return false
        }
        return true
    }
}
