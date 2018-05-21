using OA.Core;
using System;
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
            public byte LockLevel;
            public FormId<KEYMRecord> Key;
            public byte Flags;

            public XLOCField(UnityBinaryReader r, uint dataSize)
            {
                LockLevel = r.ReadByte();
                Key = new FormId<KEYMRecord>(r.ReadLEUInt32());
                Flags = r.ReadByte();
            }
        }

        public struct XESPField
        {
            public FormId<Record> Reference;
            public byte Flags;

            public XESPField(UnityBinaryReader r, uint dataSize)
            {
                Reference = new FormId<Record>(r.ReadLEUInt32());
                Flags = r.ReadByte();
                r.ReadBytes(3); // Unused
            }
        }

        public struct XSEDField
        {
            public byte Seed;

            public XSEDField(UnityBinaryReader r, uint dataSize)
            {
                Seed = r.ReadByte();
                r.ReadBytes(3); // Unused
            }
        }

        public override string ToString() => $"REFR: {EDID.Value}";
        public STRVField EDID { get; set; }
        public FMIDField<Record> NAME; // Base
        public XTELField? XTEL; // Teleport Destination (optional)
        public DATAField DATA; // Position/Rotation
        public XLOCField? XLOC; // Lock information (optional)
        public List<CELLRecord.XOWNGroup> XOWNs; // Ownership (optional)
        public XESPField? XESP; // Enable Parent (optional)
        public FMIDField<Record>? XTRG; // Target (optional)
        public XESPField? XSED; // SpeedTree (optional)
        public BYTVField? XLOD; // Distant LOD Data (optional)
        public FLTVField? XCHG; // Charge (optional)
        public FLTVField? XHLT; // Health (optional)
        public FMIDField<CELLRecord>? XPCI; // Unused (optional)
        public IN32Field? XLCM; // Level Modifier (optional)
        public FMIDField<REFRRecord>? XRTM; // Unknown (optional)
        public UI32Field? XACT; // Action Flag (optional)
        public IN32Field? XCNT; // Count (optional)
        //public bool? ONAM; // Open by Default
        public BYTVField? XRGD; // Ragdoll Data
        public FLTVField? XSCL; // Scale
        public BYTEField? XSOL; // Contained Soul

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, uint dataSize)
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
                case "XLOD": XLOD = new BYTVField(r, dataSize); return true;
                case "XCHG": XCHG = new FLTVField(r, dataSize); return true;
                case "XHLT": XCHG = new FLTVField(r, dataSize); return true;
                case "XPCI": XRTM = new FMIDField<REFRRecord>(r, dataSize); return true;
                case "FULL": XRTM.Value.AddName(r.ReadASCIIString((int)dataSize)); return true;
                case "XLCM": XLCM = new IN32Field(r, dataSize); return true;
                case "XRTM": XRTM = new FMIDField<REFRRecord>(r, dataSize); return true;
                case "XACT": XACT = new UI32Field(r, dataSize); return true;
                case "XCNT": XCNT = new IN32Field(r, dataSize); return true;
                case "ONAM": return true;
                case "XRGD": XRGD = new BYTVField(r, dataSize); return true;
                case "XSCL": XSCL = new FLTVField(r, dataSize); return true;
                case "XSOL": XSOL = new BYTEField(r, dataSize); return true;
                default: return false;
            }
        }
    }
}