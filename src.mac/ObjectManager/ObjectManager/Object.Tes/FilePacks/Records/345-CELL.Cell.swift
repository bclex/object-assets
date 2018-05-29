using OA.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    public class CELLRecord : Record, ICellRecord
    {
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
                GridX = r.ReadLEInt32();
                GridY = r.ReadLEInt32();
                Flags = formatId == GameFormatId.TES5 ? r.ReadLEUInt32() : 0;
            }
        }

        public struct XCLLField
        {
            public ColorRef AmbientColor;
            public ColorRef DirectionalColor; //: SunlightColor
            public ColorRef FogColor;
            public float FogNear; //: FogDensity
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
                AmbientColor = new ColorRef(r);
                DirectionalColor = new ColorRef(r);
                FogColor = new ColorRef(r);
                FogNear = r.ReadLESingle();
                if (formatId == GameFormatId.TES3)
                {
                    FogFar = DirectionalFade = FogClipDist = DirectionalRotationXY = DirectionalRotationZ = 0;
                    FogPow = 0;
                    return;
                }
                FogFar = r.ReadLESingle();
                DirectionalRotationXY = r.ReadLEInt32();
                DirectionalRotationZ = r.ReadLEInt32();
                DirectionalFade = r.ReadLESingle();
                FogClipDist = r.ReadLESingle();
                if (formatId == GameFormatId.TES4)
                {
                    FogPow = 0;
                    return;
                }
                FogPow = r.ReadLESingle();
            }
        }

        public class XOWNGroup
        {
            public FMIDField<Record> XOWN;
            public IN32Field XRNK; // Faction rank
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
                    Position = r.ReadLEVector3();
                    EulerAngles = r.ReadLEVector3();
                }
            }

            public UI32Field? FRMR; // Object Index (starts at 1)
                                    // This is used to uniquely identify objects in the cell. For new files the index starts at 1 and is incremented for each new object added. For modified
                                    // objects the index is kept the same.
            public override string ToString() => $"CREF: {EDID.Value}";
            public STRVField EDID; // Object ID
            public FLTVField? XSCL; // Scale (Static)
            public IN32Field? DELE; // Indicates that the reference is deleted.
            public XYZAField? DODT; // XYZ Pos, XYZ Rotation of exit
            public STRVField DNAM; // Door exit name (Door objects)
            public FLTVField? FLTV; // Follows the DNAM optionally, lock level
            public STRVField KNAM; // Door key
            public STRVField TNAM; // Trap name
            public BYTEField? UNAM; // Reference Blocked (only occurs once in MORROWIND.ESM)
            public STRVField ANAM; // Owner ID string
            public STRVField BNAM; // Global variable/rank ID
            public IN32Field? INTV; // Number of uses, occurs even for objects that don't use it
            public UI32Field? NAM9; // Unknown
            public STRVField XSOL; // Soul Extra Data (ID string of creature)
            public XYZAField DATA; // Ref Position Data
            //
            public STRVField CNAM; // Unknown
            public UI32Field? NAM0; // Unknown
            public IN32Field? XCHG; // Unknown
            public IN32Field? INDX; // Unknown
        }

        public override string ToString() => $"CELL: {FULL.Value}";
        public STRVField EDID { get; set; } // Editor ID. Can be an empty string for exterior cells in which case the region name is used instead.
        public STRVField FULL; // Full Name / TES3:RGNN - Region name
        public UI16Field DATA; // Flags
        public XCLCField? XCLC; // Cell Data (only used for exterior cells)
        public XCLLField? XCLL; // Lighting (only used for interior cells)
        public FLTVField? XCLW; // Water Height
        // TES3
        public UI32Field? NAM0; // Number of objects in cell in current file (Optional)
        public INTVField INTV; // Unknown
        public CREFField? NAM5; // Map Color (COLORREF)
        // TES4
        public FMIDField<REGNRecord>[] XCLRs; // Regions
        public BYTEField? XCMT; // Music (optional)
        public FMIDField<CLMTRecord>? XCCM; // Climate
        public FMIDField<WATRRecord>? XCWT; // Water
        public List<XOWNGroup> XOWNs = new List<XOWNGroup>(); // Ownership

        // Referenced Object Data Grouping
        public bool InFRMR = false;
        public List<RefObj> RefObjs = new List<RefObj>();

        public bool IsInterior => Utils.ContainsBitFlags(DATA.Value, 0x01);
        public Vector2i GridCoords => new Vector2i(XCLC.Value.GridX, XCLC.Value.GridY);
        public Color? AmbientLight => XCLL != null ? (Color?)XCLL.Value.AmbientColor.ToColor32() : null;

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (!InFRMR && type == "FRMR")
                InFRMR = true;
            if (!InFRMR)
                switch (type)
                {
                    case "EDID":
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "FULL":
                    case "RGNN": FULL = new STRVField(r, dataSize); return true;
                    case "DATA": DATA = new INTVField(r, formatId == GameFormatId.TES3 ? 4 : dataSize).ToUI16Field(); if (formatId == GameFormatId.TES3) goto case "XCLC"; return true;
                    case "XCLC": XCLC = new XCLCField(r, dataSize, formatId); return true;
                    case "XCLL":
                    case "AMBI": XCLL = new XCLLField(r, dataSize, formatId); return true;
                    case "XCLW":
                    case "WHGT": XCLW = new FLTVField(r, dataSize); return true;
                    // TES3
                    case "NAM0": NAM0 = new UI32Field(r, dataSize); return true;
                    case "INTV": INTV = new INTVField(r, dataSize); return true;
                    case "NAM5": NAM5 = new CREFField(r, dataSize); return true;
                    // TES4
                    case "XCLR": XCLRs = new FMIDField<REGNRecord>[dataSize >> 2]; for (var i = 0; i < XCLRs.Length; i++) XCLRs[i] = new FMIDField<REGNRecord>(r, 4); return true;
                    case "XCMT": XCMT = new BYTEField(r, dataSize); return true;
                    case "XCCM": XCCM = new FMIDField<CLMTRecord>(r, dataSize); return true;
                    case "XCWT": XCWT = new FMIDField<WATRRecord>(r, dataSize); return true;
                    case "XOWN": XOWNs.Add(new XOWNGroup { XOWN = new FMIDField<Record>(r, dataSize) }); return true;
                    case "XRNK": ArrayUtils.Last(XOWNs).XRNK = new IN32Field(r, dataSize); return true;
                    case "XGLB": ArrayUtils.Last(XOWNs).XGLB = new FMIDField<Record>(r, dataSize); return true;
                    default: return false;
                }
            // Referenced Object Data Grouping
            else switch (type)
                {
                    // RefObjDataGroup sub-records
                    case "FRMR": RefObjs.Add(new RefObj()); ArrayUtils.Last(RefObjs).FRMR = new UI32Field(r, dataSize); return true;
                    case "NAME": ArrayUtils.Last(RefObjs).EDID = new STRVField(r, dataSize); return true;
                    case "XSCL": ArrayUtils.Last(RefObjs).XSCL = new FLTVField(r, dataSize); return true;
                    case "DODT": ArrayUtils.Last(RefObjs).DODT = new RefObj.XYZAField(r, dataSize); return true;
                    case "DNAM": ArrayUtils.Last(RefObjs).DNAM = new STRVField(r, dataSize); return true;
                    case "FLTV": ArrayUtils.Last(RefObjs).FLTV = new FLTVField(r, dataSize); return true;
                    case "KNAM": ArrayUtils.Last(RefObjs).KNAM = new STRVField(r, dataSize); return true;
                    case "TNAM": ArrayUtils.Last(RefObjs).TNAM = new STRVField(r, dataSize); return true;
                    case "UNAM": ArrayUtils.Last(RefObjs).UNAM = new BYTEField(r, dataSize); return true;
                    case "ANAM": ArrayUtils.Last(RefObjs).ANAM = new STRVField(r, dataSize); return true;
                    case "BNAM": ArrayUtils.Last(RefObjs).BNAM = new STRVField(r, dataSize); return true;
                    case "INTV": ArrayUtils.Last(RefObjs).INTV = new IN32Field(r, dataSize); return true;
                    case "NAM9": ArrayUtils.Last(RefObjs).NAM9 = new UI32Field(r, dataSize); return true;
                    case "XSOL": ArrayUtils.Last(RefObjs).XSOL = new STRVField(r, dataSize); return true;
                    case "DATA": ArrayUtils.Last(RefObjs).DATA = new RefObj.XYZAField(r, dataSize); return true;
                    //
                    case "CNAM": ArrayUtils.Last(RefObjs).CNAM = new STRVField(r, dataSize); return true;
                    case "NAM0": ArrayUtils.Last(RefObjs).NAM0 = new UI32Field(r, dataSize); return true;
                    case "XCHG": ArrayUtils.Last(RefObjs).XCHG = new IN32Field(r, dataSize); return true;
                    case "INDX": ArrayUtils.Last(RefObjs).INDX = new IN32Field(r, dataSize); return true;
                    default: return false;
                }
        }
    }
}