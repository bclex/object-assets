using OA.Core;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    public class REFRRecord : Record
    {
        public struct XTELField
        {
            public FormId<REFRRecord> Door;
            public Vector3 Position;
            public Vector3 Rotation;

            public XTELField(UnityBinaryReader r, int dataSize)
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

            public DATAField(UnityBinaryReader r, int dataSize)
            {
                Position = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
                Rotation = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
            }
        }

        public struct XLOCField
        {
            public override string ToString() => $"{Key}";
            public byte LockLevel;
            public FormId<KEYMRecord> Key;
            public byte Flags;

            public XLOCField(UnityBinaryReader r, int dataSize)
            {
                LockLevel = r.ReadByte();
                r.SkipBytes(3); // Unused
                Key = new FormId<KEYMRecord>(r.ReadLEUInt32());
                if (dataSize == 16)
                    r.SkipBytes(4); // Unused
                Flags = r.ReadByte();
                r.SkipBytes(3); // Unused
            }
        }

        public struct XESPField
        {
            public override string ToString() => $"{Reference}";
            public FormId<Record> Reference;
            public byte Flags;

            public XESPField(UnityBinaryReader r, int dataSize)
            {
                Reference = new FormId<Record>(r.ReadLEUInt32());
                Flags = r.ReadByte();
                r.SkipBytes(3); // Unused
            }
        }

        public struct XSEDField
        {
            public override string ToString() => $"{Seed}";
            public byte Seed;

            public XSEDField(UnityBinaryReader r, int dataSize)
            {
                Seed = r.ReadByte();
                if (dataSize == 4)
                    r.SkipBytes(3); // Unused
            }
        }

        public class XMRKGroup
        {
            public override string ToString() => $"{FULL.Value}";
            public BYTEField FNAM; // Map Flags
            public STRVField FULL; // Name
            public BYTEField TNAM; // Type
        }

        public override string ToString() => $"REFR: {EDID.Value}";
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
        int _nextFull;

        public override bool CreateField(UnityBinaryReader r, GameFormatId format, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID": EDID = new STRVField(r, dataSize); return true;
                case "NAME": NAME = new FMIDField<Record>(r, dataSize); return true;
                case "XTEL": XTEL = new XTELField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize); return true;
                case "XLOC": XLOC = new XLOCField(r, dataSize); return true;
                case "XOWN": if (XOWNs == null) XOWNs = new List<CELLRecord.XOWNGroup>(); XOWNs.Add(new CELLRecord.XOWNGroup { XOWN = new FMIDField<Record>(r, dataSize) }); return true;
                case "XRNK": ArrayUtils.Last(XOWNs).XRNK = new IN32Field(r, dataSize); return true;
                case "XGLB": ArrayUtils.Last(XOWNs).XGLB = new FMIDField<Record>(r, dataSize); return true;
                case "XESP": XESP = new XESPField(r, dataSize); return true;
                case "XTRG": XTRG = new FMIDField<Record>(r, dataSize); return true;
                case "XSED": XSED = new XSEDField(r, dataSize); return true;
                case "XLOD": XLOD = new BYTVField(r, dataSize); return true;
                case "XCHG": XCHG = new FLTVField(r, dataSize); return true;
                case "XHLT": XCHG = new FLTVField(r, dataSize); return true;
                case "XPCI": XPCI = new FMIDField<CELLRecord>(r, dataSize); _nextFull = 1; return true;
                case "FULL":
                    if (_nextFull == 1) XPCI.Value.AddName(r.ReadASCIIString(dataSize));
                    else if (_nextFull == 2) ArrayUtils.Last(XMRKs).FULL = new STRVField(r, dataSize);
                    _nextFull = 0;
                    return true;
                case "XLCM": XLCM = new IN32Field(r, dataSize); return true;
                case "XRTM": XRTM = new FMIDField<REFRRecord>(r, dataSize); return true;
                case "XACT": XACT = new UI32Field(r, dataSize); return true;
                case "XCNT": XCNT = new IN32Field(r, dataSize); return true;
                case "XMRK": if (XMRKs == null) XMRKs = new List<XMRKGroup>(); XMRKs.Add(new XMRKGroup()); _nextFull = 2; return true;
                case "FNAM": ArrayUtils.Last(XMRKs).FNAM = new BYTEField(r, dataSize); return true;
                case "TNAM": ArrayUtils.Last(XMRKs).TNAM = new BYTEField(r, dataSize); r.ReadByte(); return true;
                case "ONAM": return true;
                case "XRGD": XRGD = new BYTVField(r, dataSize); return true;
                case "XSCL": XSCL = new FLTVField(r, dataSize); return true;
                case "XSOL": XSOL = new BYTEField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}