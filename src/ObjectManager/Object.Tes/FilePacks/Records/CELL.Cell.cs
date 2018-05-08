using OA.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    // TODO: add support for strange INTV before object data?
    public class CELLRecord : Record, ICellRecord
    {
        public class CELLDATASubRecord : SubRecord
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
        public class RGNNSubRecord : NAMESubRecord { }
        public class NAM0SubRecord : UInt32SubRecord { }
        public class NAM5SubRecord : Int32SubRecord { } // map color (COLORREF)
        public class WHGTSubRecord : FLTVSubRecord { }
        public class AMBISubRecord : SubRecord
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
            public class FRMRSubRecord : UInt32SubRecord { }
            public class XSCLSubRecord : FLTVSubRecord { }
            public class DODTSubRecord : SubRecord
            {
                public Vector3 Position;
                public Vector3 EulerAngles;

                public override void Read(UnityBinaryReader r, uint dataSize)
                {
                    Position = r.ReadLEVector3();
                    EulerAngles = r.ReadLEVector3();
                }
            }
            public class DNAMSubRecord : NAMESubRecord { }
            public class KNAMSubRecord : NAMESubRecord { }
            public class TNAMSubRecord : NAMESubRecord { }
            public class UNAMSubRecord : ByteSubRecord { }
            public class ANAMSubRecord : NAMESubRecord { }
            public class BNAMSubRecord : NAMESubRecord { }
            public class NAM9SubRecord : UInt32SubRecord { }
            public class XSOLSubRecord : NAMESubRecord { }
            public class DATASubRecord : SubRecord
            {
                public Vector3 Position;
                public Vector3 EulerAngles;

                public override void Read(UnityBinaryReader r, uint dataSize)
                {
                    Position = r.ReadLEVector3();
                    EulerAngles = r.ReadLEVector3();
                }
            }

            public FRMRSubRecord FRMR;
            public NAMESubRecord NAME;
            public XSCLSubRecord XSCL;
            public DODTSubRecord DODT;
            public DNAMSubRecord DNAM;
            public FLTVSubRecord FLTV;
            public KNAMSubRecord KNAM;
            public TNAMSubRecord TNAM;
            public UNAMSubRecord UNAM;
            public ANAMSubRecord ANAM;
            public BNAMSubRecord BNAM;
            public INTVSubRecord INTV;
            public NAM9SubRecord NAM9;
            public XSOLSubRecord XSOL;
            public DATASubRecord DATA;
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

        public NAMESubRecord NAME;

        public bool IsReadingObjectDataGroups = false;
        public CELLDATASubRecord DATA;

        public RGNNSubRecord RGNN;
        public NAM0SubRecord NAM0;

        // Exterior Cells
        public NAM5SubRecord NAM5;

        // Interior Cells
        public WHGTSubRecord WHGT;
        public AMBISubRecord AMBI;

        public List<RefObj> RefObjs = new List<RefObj>();

        public override SubRecord CreateUninitializedSubRecord(string subRecordName)
        {
            if (!IsReadingObjectDataGroups && subRecordName == "FRMR")
                IsReadingObjectDataGroups = true;
            if (!IsReadingObjectDataGroups)
                switch (subRecordName)
                {
                    case "NAME": NAME = new NAMESubRecord(); return NAME;
                    case "DATA": DATA = new CELLDATASubRecord(); return DATA;
                    case "RGNN": RGNN = new RGNNSubRecord(); return RGNN;
                    case "NAM0": NAM0 = new NAM0SubRecord(); return NAM0;
                    case "NAM5": NAM5 = new NAM5SubRecord(); return NAM5;
                    case "WHGT": WHGT = new WHGTSubRecord(); return WHGT;
                    case "AMBI": AMBI = new AMBISubRecord(); return AMBI;
                    default: return null;
                }
            else switch (subRecordName)
                {
                    // RefObjDataGroup sub-records
                    case "FRMR": RefObjs.Add(new RefObj()); ArrayUtils.Last(RefObjs).FRMR = new RefObj.FRMRSubRecord(); return ArrayUtils.Last(RefObjs).FRMR;
                    case "NAME": ArrayUtils.Last(RefObjs).NAME = new NAMESubRecord(); return ArrayUtils.Last(RefObjs).NAME;
                    case "XSCL": ArrayUtils.Last(RefObjs).XSCL = new RefObj.XSCLSubRecord(); return ArrayUtils.Last(RefObjs).XSCL;
                    case "DODT": ArrayUtils.Last(RefObjs).DODT = new RefObj.DODTSubRecord(); return ArrayUtils.Last(RefObjs).DODT;
                    case "DNAM": ArrayUtils.Last(RefObjs).DNAM = new RefObj.DNAMSubRecord(); return ArrayUtils.Last(RefObjs).DNAM;
                    case "FLTV": ArrayUtils.Last(RefObjs).FLTV = new FLTVSubRecord(); return ArrayUtils.Last(RefObjs).FLTV;
                    case "KNAM": ArrayUtils.Last(RefObjs).KNAM = new RefObj.KNAMSubRecord(); return ArrayUtils.Last(RefObjs).KNAM;
                    case "TNAM": ArrayUtils.Last(RefObjs).TNAM = new RefObj.TNAMSubRecord(); return ArrayUtils.Last(RefObjs).TNAM;
                    case "UNAM": ArrayUtils.Last(RefObjs).UNAM = new RefObj.UNAMSubRecord(); return ArrayUtils.Last(RefObjs).UNAM;
                    case "ANAM": ArrayUtils.Last(RefObjs).ANAM = new RefObj.ANAMSubRecord(); return ArrayUtils.Last(RefObjs).ANAM;
                    case "BNAM": ArrayUtils.Last(RefObjs).BNAM = new RefObj.BNAMSubRecord(); return ArrayUtils.Last(RefObjs).BNAM;
                    case "INTV": ArrayUtils.Last(RefObjs).INTV = new INTVSubRecord(); return ArrayUtils.Last(RefObjs).INTV;
                    case "NAM9": ArrayUtils.Last(RefObjs).NAM9 = new RefObj.NAM9SubRecord(); return ArrayUtils.Last(RefObjs).NAM9;
                    case "XSOL": ArrayUtils.Last(RefObjs).XSOL = new RefObj.XSOLSubRecord(); return ArrayUtils.Last(RefObjs).XSOL;
                    case "DATA": ArrayUtils.Last(RefObjs).DATA = new RefObj.DATASubRecord(); return ArrayUtils.Last(RefObjs).DATA;
                    default: return null;
                }
        }

        public override SubRecord CreateUninitializedSubRecord(string subRecordName, GameId gameId) => throw new NotImplementedException();
    }
}