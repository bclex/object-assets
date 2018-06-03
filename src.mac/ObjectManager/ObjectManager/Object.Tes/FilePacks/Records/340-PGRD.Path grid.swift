//
//  PGRDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class PGRDRecord: Record {
    public struct DATAField
    {
        public int X;
        public int Y;
        public short Granularity;
        public short PointCount;

        public DATAField(UnityBinaryReader r, uint dataSize, GameFormatId formatId)
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

        public PGRPField(UnityBinaryReader r, uint dataSize)
        {
            Point = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
            Connections = r.ReadByte();
            r.skipBytes(3); // Unused
        }
    }

    public struct PGRRField
    {
        public short StartPointId;
        public short EndPointId;

        public PGRRField(UnityBinaryReader r, uint dataSize)
        {
            StartPointId = r.ReadLEInt16();
            EndPointId = r.ReadLEInt16();
        }
    }

    public struct PGRIField
    {
        public short PointId;
        public Vector3 ForeignNode;

        public PGRIField(UnityBinaryReader r, uint dataSize)
        {
            PointId = r.ReadLEInt16();
            r.skipBytes(2); // Unused (can merge back)
            ForeignNode = new Vector3(r.ReadLESingle(), r.ReadLESingle(), r.ReadLESingle());
        }
    }

    public struct PGRLField
    {
        public FormId<REFRRecord> Reference;
        public short[] PointIds;

        public PGRLField(UnityBinaryReader r, uint dataSize)
        {
            Reference = new FormId<REFRRecord>(r.ReadLEUInt32());
            PointIds = new short[(dataSize - 4) >> 2];
            for (var i = 0; i < PointIds.Length; i++)
            {
                PointIds[i] = r.ReadLEInt16();
                r.skipBytes(2); // Unused (can merge back)
            }
        }
    }

    public var description: String { return "PGRD: \(EDID)" }
    public STRVField EDID { get; set; } // Editor ID
    public DATAField DATA; // Number of nodes
    public PGRPField[] PGRPs;
    public UNKNField PGRC;
    public UNKNField PGAG;
    public PGRRField[] PGRRs; // Point-to-Point Connections
    public PGRLs: [PGRLField]? // Point-to-Reference Mappings
    public PGRIField[] PGRIs; // Inter-Cell Connections

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize, format)
        case "PGRP": PGRPs = [PGRPField](); PGRPs.reserveCapacity(dataSize >> 4); for i in 0..<PGRPs.capacity { PGRPs[i] = PGRPField(r, 16) }
        case "PGRC": PGRC = UNKNField(r, dataSize)
        case "PGAG": PGAG = UNKNField(r, dataSize)
        case "PGRR": PGRRs = [PGRRField](); PGRRs.reserveCapacity(dataSize >> 2); for i in 0..<PGRRs.capacity { PGRRs[i] = PGRRField(r, 4); } r.skipBytes(dataSize % 4)
        case "PGRL": if PGRLs == nil { PGRLs = [PGRLField]() }; PGRLs.append(PGRLField(r, dataSize))
        case "PGRI": PGRIs = [PGRIField](); PGRIs.reserveCapacity(dataSize >> 4); for i in 0..<PGRIs.capacity { PGRIs[i] = PGRIField(r, 16) }
        default: return false
        }
        return true
    }
}
