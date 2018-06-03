//
//  REFRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class REFRRecord: Record {
    public struct XTELField
    {
        public FormId<REFRRecord> Door;
        public Vector3 Position;
        public Vector3 Rotation;

        public XTELField(UnityBinaryReader r, uint dataSize)
        {
            Door = new FormId<REFRRecord>(r.ReadLEUInt32());
            Position = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
            Rotation = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
        }
    }

    public struct DATAField
    {
        public Vector3 Position;
        public Vector3 Rotation;

        public DATAField(UnityBinaryReader r, uint dataSize)
        {
            Position = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
            Rotation = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
        }
    }

    public struct XLOCField
    {
        public var description: String { return "\(Key)" }
        public byte LockLevel;
        public FormId<KEYMRecord> Key;
        public byte Flags;

        public XLOCField(UnityBinaryReader r, uint dataSize)
        {
            LockLevel = r.ReadByte();
            r.skipBytes(3); // Unused
            Key = new FormId<KEYMRecord>(r.ReadLEUInt32());
            if (dataSize == 16)
                r.skipBytes(4); // Unused
            Flags = r.ReadByte();
            r.skipBytes(3); // Unused
        }
    }

    public struct XESPField
    {
        public var description: String { return "\(Reference)" }
        public FormId<Record> Reference;
        public byte Flags;

        public XESPField(UnityBinaryReader r, uint dataSize)
        {
            Reference = new FormId<Record>(r.ReadLEUInt32());
            Flags = r.ReadByte();
            r.skipBytes(3); // Unused
        }
    }

    public struct XSEDField
    {
        public var description: String { return "\(Seed)" }
        public byte Seed;

        public XSEDField(UnityBinaryReader r, uint dataSize)
        {
            Seed = r.ReadByte();
            if (dataSize == 4)
                r.skipBytes(3); // Unused
        }
    }

    public class XMRKGroup
    {
        public var description: String { return "\(FULL)" }
        public BYTEField FNAM; // Map Flags
        public STRVField FULL; // Name
        public BYTEField TNAM; // Type
    }

    public var description: String { return "REFR: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public FMIDField<Record> NAME; // Base
    public XTELField? XTEL; // Teleport Destination (optional)
    public DATAField DATA; // Position/Rotation
    public XLOCField? XLOC; // Lock information (optional)
    public List<CELLRecord.XOWNGroup> XOWNs; // Ownership (optional)
    public XESPField? XESP; // Enable Parent (optional)
    public FMIDField<Record>? XTRG; // Target (optional)
    public XSEDField? XSED; // SpeedTree (optional)
    public BYTVField? XLOD; // Distant LOD Data (optional)
    public FLTVField? XCHG; // Charge (optional)
    public FLTVField? XHLT; // Health (optional)
    public FMIDField<CELLRecord>? XPCI; // Unused (optional)
    public IN32Field? XLCM; // Level Modifier (optional)
    public FMIDField<REFRRecord>? XRTM; // Unknown (optional)
    public UI32Field? XACT; // Action Flag (optional)
    public IN32Field? XCNT; // Count (optional)
    public List<XMRKGroup> XMRKs; // Ownership (optional)
    //public bool? ONAM; // Open by Default
    public BYTVField? XRGD; // Ragdoll Data (optional)
    public FLTVField? XSCL; // Scale (optional)
    public BYTEField? XSOL; // Contained Soul (optional)
    var _nextFull: Int

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "NAME": NAME = FMIDField<Record>(r, dataSize)
        case "XTEL": XTEL = XTELField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "XLOC": XLOC = XLOCField(r, dataSize)
        case "XOWN": if XOWNs == nil { XOWNs = [CELLRecord.XOWNGroup]() } XOWNs.append(CELLRecord.XOWNGroup(XOWN: FMIDField<Record>(r, dataSize)))
        case "XRNK": XOWNs.last!.XRNK = IN32Field(r, dataSize)
        case "XGLB": XOWNs.last!.XGLB = FMIDField<Record>(r, dataSize)
        case "XESP": XESP = XESPField(r, dataSize)
        case "XTRG": XTRG = FMIDField<Record>(r, dataSize)
        case "XSED": XSED = XSEDField(r, dataSize)
        case "XLOD": XLOD = BYTVField(r, dataSize)
        case "XCHG": XCHG = FLTVField(r, dataSize)
        case "XHLT": XCHG = FLTVField(r, dataSize)
        case "XPCI": XPCI = FMIDField<CELLRecord>(r, dataSize); NextFull = 1
        case "FULL":
            if _nextFull == 1 { XPCI.addName(r.readASCIIString(dataSize)) }
            else if _nextFull == 2 { XMRKs.last!.FULL = STRVField(r, dataSize) }
            _nextFull = 0
        case "XLCM": XLCM = IN32Field(r, dataSize)
        case "XRTM": XRTM = FMIDField<REFRRecord>(r, dataSize)
        case "XACT": XACT = UI32Field(r, dataSize)
        case "XCNT": XCNT = IN32Field(r, dataSize)
        case "XMRK": if XMRKs == nil { XMRKs = [XMRKGroup]() } XMRKs.append(XMRKGroup()); _nextFull = 2
        case "FNAM": XMRKs.last!.FNAM = BYTEField(r, dataSize)
        case "TNAM": XMRKs.last!.TNAM = BYTEField(r, dataSize); _ = r.ReadByte()
        case "ONAM": break
        case "XRGD": XRGD = BYTVField(r, dataSize)
        case "XSCL": XSCL = FLTVField(r, dataSize)
        case "XSOL": XSOL = BYTEField(r, dataSize)
        default: return false
        }
        return true
    }
}
