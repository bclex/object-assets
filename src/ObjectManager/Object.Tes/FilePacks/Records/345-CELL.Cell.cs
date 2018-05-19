using OA.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    // TODO: add support for strange INTV before object data?
    public class CELLRecord : Record, ICellRecord
    {
        public struct DATAField
        {
            [Flags]
            public enum CELLFlags : uint
            {
                Interior = 0x01,
                HasWater = 0x02,
                IllegalToSleepHere = 0x04,
                BehaveLikeExterior = 0x80, // (Tribunal)
            }
            public uint Flags;
            public int GridX;
            public int GridY;

            public DATAField(UnityBinaryReader r, uint dataSize)
            {
                Flags = r.ReadLEUInt32();
                GridX = r.ReadLEInt32();
                GridY = r.ReadLEInt32();
            }
        }

        public struct AMBIField
        {
            public uint AmbientColor;
            public uint SunlightColor;
            public uint FogColor;
            public float FogDensity;

            public AMBIField(UnityBinaryReader r, uint dataSize)
            {
                AmbientColor = r.ReadLEUInt32();
                SunlightColor = r.ReadLEUInt32();
                FogColor = r.ReadLEUInt32();
                FogDensity = r.ReadLESingle();
            }
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

        public override string ToString() => $"CELL: {RGNN.Value}";
        public STRVField EDID; // Cell ID. Can be an empty string for exterior cells in which case the region name is used instead.
        public DATAField DATA; // Cell Data
        public STRVField RGNN; // Region name
        public UI32Field? NAM0; // Number of objects in cell in current file (Optional)
        public INTVField INTV; // Unknown
        // Exterior Cells
        public CREFField? NAM5; // Map Color (COLORREF)
        // Interior Cells
        public FLTVField? WHGT; // Water Height
        public AMBIField? AMBI; // Ambient Light Level
        // Referenced Object Data Grouping
        public bool InObjectDataGroups = false;
        public List<RefObj> RefObjs = new List<RefObj>();

        public bool IsInterior => Utils.ContainsBitFlags((int)DATA.Flags, 0x01);
        public Vector2i GridCoords => new Vector2i(DATA.GridX, DATA.GridY);
        public Color? AmbientLight => AMBI != null ? (Color?)ColorUtils.B8G8R8ToColor32(AMBI.Value.AmbientColor) : null;

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
        {
            if (!InObjectDataGroups && type == "FRMR")
                InObjectDataGroups = true;
            if (!InObjectDataGroups)
                switch (type)
                {
                    case "NAME": EDID = new STRVField(r, dataSize); return true;
                    case "DATA": DATA = new DATAField(r, dataSize); return true;
                    case "RGNN": RGNN = new STRVField(r, dataSize); return true;
                    case "NAM0": NAM0 = new UI32Field(r, dataSize); return true;
                    case "INTV": INTV = new INTVField(r, dataSize); return true;
                    // Exterior Cell
                    case "NAM5": NAM5 = new CREFField(r, dataSize); return true;
                    // Interior Cell
                    case "WHGT": WHGT = new FLTVField(r, dataSize); return true;
                    case "AMBI": AMBI = new AMBIField(r, dataSize); return true;
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