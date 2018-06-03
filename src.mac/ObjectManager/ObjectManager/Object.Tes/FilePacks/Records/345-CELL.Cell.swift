//
//  CELLRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CELLRecord: Record, ICellRecord {
    [Flags]
    public enum CELLFlags : ushort
    {
        Interior = 0x0001,
        HasWater = 0x0002,
        InvertFastTravel = 0x0004, //: IllegalToSleepHere
        BehaveLikeExterior = 0x0008, //: BehaveLikeExterior (Tribunal), Force hide land (exterior cell) / Oblivion interior (interior cell)
        Unknown1 = 0x0010,
        PublicArea = 0x0020, // Public place
        HandChanged = 0x0040,
        ShowSky = 0x0080, // Behave like exterior
        UseSkyLighting = 0x0100,
    }

    public struct XCLCField
    {
        public int GridX;
        public int GridY;
        public uint Flags;

        public XCLCField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            GridX = r.readLEInt32();
            GridY = r.readLEInt32();
            Flags = formatId == GameFormatId.TES5 ? r.readLEUInt32() : 0;
        }
    }

    public struct XCLLField
    {
        public ColorRef AmbientColor;
        public ColorRef DirectionalColor //: SunlightColor
        public ColorRef FogColor;
        public float FogNear //: FogDensity
        // TES4
        public float FogFar;
        public int DirectionalRotationXY;
        public int DirectionalRotationZ;
        public float DirectionalFade;
        public float FogClipDist;
        // TES5
        public float FogPow;

        public XCLLField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
        {
            AmbientColor = ColorRef(r);
            DirectionalColor = ColorRef(r);
            FogColor = ColorRef(r);
            FogNear = r.readLESingle();
            if (formatId == GameFormatId.TES3)
            {
                FogFar = DirectionalFade = FogClipDist = DirectionalRotationXY = DirectionalRotationZ = 0;
                FogPow = 0;
                return;
            }
            FogFar = r.readLESingle();
            DirectionalRotationXY = r.readLEInt32();
            DirectionalRotationZ = r.readLEInt32();
            DirectionalFade = r.readLESingle();
            FogClipDist = r.readLESingle();
            if (formatId == GameFormatId.TES4)
            {
                FogPow = 0;
                return;
            }
            FogPow = r.readLESingle();
        }
    }

    public class XOWNGroup
    {
        public FMIDField<Record> XOWN;
        public IN32Field XRNK // Faction rank
        public FMIDField<Record> XGLB;
    }

    public class RefObj
    {
        public struct XYZAField
        {
            public Vector3 Position;
            public Vector3 EulerAngles;

            public XYZAField(UnityBinaryReader r, uint dataSize)
            {
                Position = r.readLEVector3();
                EulerAngles = r.readLEVector3();
            }
        }

        public UI32Field? FRMR // Object Index (starts at 1)
                                // This is used to uniquely identify objects in the cell. For files the index starts at 1 and is incremented for each object added. For modified
                                // objects the index is kept the same.
        public override string ToString() => $"CREF: {EDID.Value}";
        public STRVField EDID // Object ID
        public FLTVField? XSCL // Scale (Static)
        public IN32Field? DELE // Indicates that the reference is deleted.
        public XYZAField? DODT // XYZ Pos, XYZ Rotation of exit
        public STRVField DNAM // Door exit name (Door objects)
        public FLTVField? FLTV // Follows the DNAM optionally, lock level
        public STRVField KNAM // Door key
        public STRVField TNAM // Trap name
        public BYTEField? UNAM // Reference Blocked (only occurs once in MORROWIND.ESM)
        public STRVField ANAM // Owner ID string
        public STRVField BNAM // Global variable/rank ID
        public IN32Field? INTV // Number of uses, occurs even for objects that don't use it
        public UI32Field? NAM9 // Unknown
        public STRVField XSOL // Soul Extra Data (ID string of creature)
        public XYZAField DATA // Ref Position Data
        //
        public STRVField CNAM // Unknown
        public UI32Field? NAM0 // Unknown
        public IN32Field? XCHG // Unknown
        public IN32Field? INDX // Unknown
    }

    public var description: String { return "CELL: \(FULL)" }
    public STRVField EDID  // Editor ID. Can be an empty string for exterior cells in which case the region name is used instead.
    public STRVField FULL // Full Name / TES3:RGNN - Region name
    public UI16Field DATA // Flags
    public XCLCField? XCLC // Cell Data (only used for exterior cells)
    public XCLLField? XCLL // Lighting (only used for interior cells)
    public FLTVField? XCLW // Water Height
    // TES3
    public UI32Field? NAM0 // Number of objects in cell in current file (Optional)
    public INTVField INTV // Unknown
    public CREFField? NAM5 // Map Color (COLORREF)
    // TES4
    public FMIDField<REGNRecord>[] XCLRs // Regions
    public BYTEField? XCMT // Music (optional)
    public FMIDField<CLMTRecord>? XCCM // Climate
    public FMIDField<WATRRecord>? XCWT // Water
    public List<XOWNGroup> XOWNs = List<XOWNGroup>() // Ownership

    // Referenced Object Data Grouping
    public bool InFRMR = false;
    public List<RefObj> RefObjs = List<RefObj>();

    public bool IsInterior => Utils.ContainsBitFlags(DATA.Value, 0x01);
    public Vector2i GridCoords => Vector2i(XCLC.Value.GridX, XCLC.Value.GridY);
    public Color? AmbientLight => XCLL != null ? (Color?)XCLL.Value.AmbientColor.ToColor32() : null;

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        if !InFRMR && type == "FRMR" {
            InFRMR = true
        }
        if !InFRMR {
            switch type {
            case "EDID",
                 "NAME": EDID = STRVField(r, dataSize)
            case "FULL":
            case "RGNN": FULL = STRVField(r, dataSize)
            case "DATA": DATA = INTVField(r, format == .TES3 ? 4 : dataSize).ToUI16Field(); if format == .TES3 { fallthrough }
            case "XCLC": XCLC = XCLCField(r, dataSize, format)
            case "XCLL":
            case "AMBI": XCLL = XCLLField(r, dataSize, format)
            case "XCLW":
            case "WHGT": XCLW = FLTVField(r, dataSize)
            // TES3
            case "NAM0": NAM0 = UI32Field(r, dataSize)
            case "INTV": INTV = INTVField(r, dataSize)
            case "NAM5": NAM5 = CREFField(r, dataSize)
            // TES4
            case "XCLR": XCLRs = (FMIDField<REGNRecord>](); XCLRs.reserveCapacity(dataSize >> 2); for i in 0..<XCLRs.capactiy { XCLRs[i] = FMIDField<REGNRecord>(r, 4) }
            case "XCMT": XCMT = BYTEField(r, dataSize)
            case "XCCM": XCCM = FMIDField<CLMTRecord>(r, dataSize)
            case "XCWT": XCWT = FMIDField<WATRRecord>(r, dataSize)
            case "XOWN": XOWNs.Add(XOWNGroup(XOWN: FMIDField<Record>(r, dataSize)))
            case "XRNK": XOWNs.last!.XRNK = IN32Field(r, dataSize)
            case "XGLB": XOWNs.last!.XGLB = FMIDField<Record>(r, dataSize)
            default: return false
            }
            return true
        }
        // Referenced Object Data Grouping
        else {
            switch type {
            // RefObjDataGroup sub-records
            case "FRMR": RefObjs.append(RefObj()); RefObjs.last!.FRMR = UI32Field(r, dataSize)
            case "NAME": RefObjs.last!.EDID = STRVField(r, dataSize)
            case "XSCL": RefObjs.last!.XSCL = FLTVField(r, dataSize)
            case "DODT": RefObjs.last!.DODT = RefObj.XYZAField(r, dataSize)
            case "DNAM": RefObjs.last!.DNAM = STRVField(r, dataSize)
            case "FLTV": RefObjs.last!.FLTV = FLTVField(r, dataSize)
            case "KNAM": RefObjs.last!.KNAM = STRVField(r, dataSize)
            case "TNAM": RefObjs.last!.TNAM = STRVField(r, dataSize)
            case "UNAM": RefObjs.last!.UNAM = BYTEField(r, dataSize)
            case "ANAM": RefObjs.last!.ANAM = STRVField(r, dataSize)
            case "BNAM": RefObjs.last!.BNAM = STRVField(r, dataSize)
            case "INTV": RefObjs.last!.INTV = IN32Field(r, dataSize)
            case "NAM9": RefObjs.last!.NAM9 = UI32Field(r, dataSize)
            case "XSOL": RefObjs.last!.XSOL = STRVField(r, dataSize)
            case "DATA": RefObjs.last!.DATA = RefObj.XYZAField(r, dataSize)
            //
            case "CNAM": RefObjs.last!.CNAM = STRVField(r, dataSize)
            case "NAM0": RefObjs.last!.NAM0 = UI32Field(r, dataSize)
            case "XCHG": RefObjs.last!.XCHG = IN32Field(r, dataSize)
            case "INDX": RefObjs.last!.INDX = IN32Field(r, dataSize)
            default: return false
            }
            return true
        }
    }
}