using OA.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Tes.FilePacks.Records
{
    public class PGRDRecord : Record
    {
        public struct DATAField
        {
            public int X;
            public int Y;
            public short Granularity;
            public short PointCount;

            public DATAField(UnityBinaryReader r, int dataSize, GameFormatId formatId)
            {
                if (formatId == GameFormatId.TES3)
                {
                    X = r.ReadLEInt32();
                    Y = r.ReadLEInt32();
                    Granularity = r.ReadLEInt16();
                    PointCount = r.ReadLEInt16();
                    return;
                }
                else X = Y = Granularity = 0;
                PointCount = r.ReadLEInt16();
            }
        }

        public struct PGRPField
        {
            public Vector3 Point;
            public byte Connections;

            public PGRPField(UnityBinaryReader r, int dataSize)
            {
                Point = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
                Connections = r.ReadByte();
                r.SkipBytes(3); // Unused
            }
        }

        public struct PGRRField
        {
            public short StartPointId;
            public short EndPointId;

            public PGRRField(UnityBinaryReader r, int dataSize)
            {
                StartPointId = r.ReadLEInt16();
                EndPointId = r.ReadLEInt16();
            }
        }

        public struct PGRIField
        {
            public short PointId;
            public Vector3 ForeignNode;

            public PGRIField(UnityBinaryReader r, int dataSize)
            {
                PointId = r.ReadLEInt16();
                r.SkipBytes(2); // Unused (can merge back)
                ForeignNode = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
            }
        }

        public struct PGRLField
        {
            public FormId<REFRRecord> Reference;
            public short[] PointIds;

            public PGRLField(UnityBinaryReader r, int dataSize)
            {
                Reference = new FormId<REFRRecord>(r.ReadLEUInt32());
                PointIds = new short[(dataSize - 4) >> 2];
                for (var i = 0; i < PointIds.Length; i++)
                {
                    PointIds[i] = r.ReadLEInt16();
                    r.SkipBytes(2); // Unused (can merge back)
                }
            }
        }

        public override string ToString() => $"PGRD: {EDID.Value}";
        public STRVField EDID { get; set; } // Editor ID
        public DATAField DATA; // Number of nodes
        public PGRPField[] PGRPs;
        public UNKNField PGRC;
        public UNKNField PGAG;
        public PGRRField[] PGRRs; // Point-to-Point Connections
        public List<PGRLField> PGRLs; // Point-to-Reference Mappings
        public PGRIField[] PGRIs; // Inter-Cell Connections

        public override bool CreateField(UnityBinaryReader r, GameFormatId formatId, string type, int dataSize)
        {
            switch (type)
            {
                case "EDID":
                case "NAME": EDID = new STRVField(r, dataSize); return true;
                case "DATA": DATA = new DATAField(r, dataSize, formatId); return true;
                case "PGRP": PGRPs = new PGRPField[dataSize >> 4]; for (var i = 0; i < PGRPs.Length; i++) PGRPs[i] = new PGRPField(r, 16); return true;
                case "PGRC": PGRC = new UNKNField(r, dataSize); return true;
                case "PGAG": PGAG = new UNKNField(r, dataSize); return true;
                case "PGRR": PGRRs = new PGRRField[dataSize >> 2]; for (var i = 0; i < PGRRs.Length; i++) PGRRs[i] = new PGRRField(r, 4); r.SkipBytes(dataSize % 4); return true;
                case "PGRL": if (PGRLs == null) PGRLs = new List<PGRLField>(); PGRLs.Add(new PGRLField(r, dataSize)); return true;
                case "PGRI": PGRIs = new PGRIField[dataSize >> 4]; for (var i = 0; i < PGRIs.Length; i++) PGRIs[i] = new PGRIField(r, 16); return true;
                default: return false;
            }
        }
    }
}