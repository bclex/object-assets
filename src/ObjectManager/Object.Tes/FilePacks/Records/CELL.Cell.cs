using OA.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    // TODO: add support for strange INTV before object data?
    public class CELLRecord : Record, ICellRecord
    {
        public class CELLDATAField : Field
        {
            public uint Flags;
            public int GridX;
            public int GridY;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                Flags = r.ReadLEUInt32();
                GridX = r.ReadLEInt32();
                GridY = r.ReadLEInt32();
            }
        }
        public class AMBIField : Field
        {
            public uint AmbientColor;
            public uint SunlightColor;
            public uint FogColor;
            public float FogDensity;

            public override void Read(UnityBinaryReader r, uint dataSize)
            {
                AmbientColor = r.ReadLEUInt32();
                SunlightColor = r.ReadLEUInt32();
                FogColor = r.ReadLEUInt32();
                FogDensity = r.ReadLESingle();
            }
        }

        public class RefObj
        {
            public class DODTField : Field
            {
                public Vector3 Position;
                public Vector3 EulerAngles;

                public override void Read(UnityBinaryReader r, uint dataSize)
                {
                    Position = r.ReadLEVector3();
                    EulerAngles = r.ReadLEVector3();
                }
            }
            public class DATAField : Field
            {
                public Vector3 Position;
                public Vector3 EulerAngles;

                public override void Read(UnityBinaryReader r, uint dataSize)
                {
                    Position = r.ReadLEVector3();
                    EulerAngles = r.ReadLEVector3();
                }
            }

            public UInt32Field FRMR;
            public STRVField NAME;
            public FLTVField XSCL;
            public DODTField DODT;
            public STRVField DNAM;
            public FLTVField FLTV;
            public STRVField KNAM;
            public STRVField TNAM;
            public ByteField UNAM;
            public STRVField ANAM;
            public STRVField BNAM;
            public INTVField INTV;
            public UInt32Field NAM9;
            public STRVField XSOL;
            public DATAField DATA;
        }

        public bool IsInterior
        {
            get { return Utils.ContainsBitFlags(DATA.Flags, 0x01U); }
        }

        public Vector2i GridCoords
        {
            get { return new Vector2i(DATA.GridX, DATA.GridY); }
        }

        public Color? AmbientLight
        {
            get { return AMBI != null ? (Color?)ColorUtils.B8G8R8ToColor32(AMBI.AmbientColor) : null; }
        }

        public STRVField NAME;

        public bool IsReadingObjectDataGroups = false;
        public CELLDATAField DATA;

        public STRVField RGNN;
        public UInt32Field NAM0;

        // Exterior Cells
        public Int32Field NAM5; // map color (COLORREF)

        // Interior Cells
        public FLTVField WHGT;
        public AMBIField AMBI;

        public List<RefObj> RefObjs = new List<RefObj>();

        public override Field CreateField(string type)
        {
            if (!IsReadingObjectDataGroups && type == "FRMR")
                IsReadingObjectDataGroups = true;
            if (!IsReadingObjectDataGroups)
                switch (type)
                {
                    case "NAME": NAME = new STRVField(); return NAME;
                    case "DATA": DATA = new CELLDATAField(); return DATA;
                    case "RGNN": RGNN = new STRVField(); return RGNN;
                    case "NAM0": NAM0 = new UInt32Field(); return NAM0;
                    case "NAM5": NAM5 = new Int32Field(); return NAM5;
                    case "WHGT": WHGT = new FLTVField(); return WHGT;
                    case "AMBI": AMBI = new AMBIField(); return AMBI;
                    default: return null;
                }
            else switch (type)
                {
                    // RefObjDataGroup sub-records
                    case "FRMR": RefObjs.Add(new RefObj()); ArrayUtils.Last(RefObjs).FRMR = new UInt32Field(); return ArrayUtils.Last(RefObjs).FRMR;
                    case "NAME": ArrayUtils.Last(RefObjs).NAME = new STRVField(); return ArrayUtils.Last(RefObjs).NAME;
                    case "XSCL": ArrayUtils.Last(RefObjs).XSCL = new FLTVField(); return ArrayUtils.Last(RefObjs).XSCL;
                    case "DODT": ArrayUtils.Last(RefObjs).DODT = new RefObj.DODTField(); return ArrayUtils.Last(RefObjs).DODT;
                    case "DNAM": ArrayUtils.Last(RefObjs).DNAM = new STRVField(); return ArrayUtils.Last(RefObjs).DNAM;
                    case "FLTV": ArrayUtils.Last(RefObjs).FLTV = new FLTVField(); return ArrayUtils.Last(RefObjs).FLTV;
                    case "KNAM": ArrayUtils.Last(RefObjs).KNAM = new STRVField(); return ArrayUtils.Last(RefObjs).KNAM;
                    case "TNAM": ArrayUtils.Last(RefObjs).TNAM = new STRVField(); return ArrayUtils.Last(RefObjs).TNAM;
                    case "UNAM": ArrayUtils.Last(RefObjs).UNAM = new ByteField(); return ArrayUtils.Last(RefObjs).UNAM;
                    case "ANAM": ArrayUtils.Last(RefObjs).ANAM = new STRVField(); return ArrayUtils.Last(RefObjs).ANAM;
                    case "BNAM": ArrayUtils.Last(RefObjs).BNAM = new STRVField(); return ArrayUtils.Last(RefObjs).BNAM;
                    case "INTV": ArrayUtils.Last(RefObjs).INTV = new INTVField(); return ArrayUtils.Last(RefObjs).INTV;
                    case "NAM9": ArrayUtils.Last(RefObjs).NAM9 = new UInt32Field(); return ArrayUtils.Last(RefObjs).NAM9;
                    case "XSOL": ArrayUtils.Last(RefObjs).XSOL = new STRVField(); return ArrayUtils.Last(RefObjs).XSOL;
                    case "DATA": ArrayUtils.Last(RefObjs).DATA = new RefObj.DATAField(); return ArrayUtils.Last(RefObjs).DATA;
                    default: return null;
                }
        }

        public override Field CreateField(string type, GameFormatId gameFormatId) => throw new NotImplementedException();
    }
}