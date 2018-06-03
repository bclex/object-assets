//
//  CONTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CONTRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public class DATAField
    {
        public byte Flags; // flags 0x0001 = Organic, 0x0002 = Respawns, organic only, 0x0008 = Default, unknown
        public float Weight;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            if (formatId == GameFormatId.TES3)
            {
                Weight = r.ReadLESingle();
                return;
            }
            Flags = r.ReadByte();
            Weight = r.ReadLESingle();
        }

        public void FLAGField(UnityBinaryReader r, uint dataSize)
        {
            Flags = (byte)r.ReadLEUInt32();
        }
    }

    public var description: String { return "CONT: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public MODLGroup MODL { get; set; } // Model
    public STRVField FULL; // Container Name
    public DATAField DATA; // Container Data
    public FMIDField<SCPTRecord>? SCRI;
    public List<CNTOField> CNTOs = new List<CNTOField>();
    // TES4
    public FMIDField<SOUNRecord> SNAM; // Open sound
    public FMIDField<SOUNRecord> QNAM; // Close sound

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
                "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
                "FNAM": FULL = STRVField(r, dataSize)
        case "DATA",
                "CNDT": DATA = DATAField(r, dataSize, format)
        case "FLAG": DATA.FLAGField(r, dataSize)
        case "CNTO",
                "NPCO": CNTOs.append(CNTOField(r, dataSize, format))
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
        case "QNAM": QNAM = FMIDField<SOUNRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}