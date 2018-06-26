//
//  PGRDRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
import SceneKit

public class PGRDRecord: Record {
    public struct DATAField {
        public let x: Int32
        public let y: Int32
        public let granularity: Int16
        public let pointCount: Int16

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format == .TES3 else {
                x = 0; y = 0; granularity = 0
                pointCount = r.readLEInt16()
                return
            }
            x = r.readLEInt32()
            y = r.readLEInt32()
            granularity = r.readLEInt16()
            pointCount = r.readLEInt16()
        }
    }

    public struct PGRPField {
        public let point: Vector3
        public let connections: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            point = Vector3(r.readLESingle(), r.readLESingle(), r.readLESingle())
            connections = r.readByte()
            r.skipBytes(3) // Unused
        }
    }

    public struct PGRRField {
        public let startPointId: Int16
        public let endPointId: Int16

        init(_ r: BinaryReader, _ dataSize: Int) {
            startPointId = r.readLEInt16()
            endPointId = r.readLEInt16()
        }
    }

    public struct PGRIField {
        public let pointId: Int16
        public let foreignNode: Vector3

        init(_ r: BinaryReader, _ dataSize: Int) {
            pointId = r.readLEInt16()
            r.skipBytes(2) // Unused (can merge back)
            foreignNode = Vector3(r.readLESingle(), r.readLESingle(), r.readLESingle())
        }
    }

    public struct PGRLField {
        public let reference: FormId<REFRRecord>
        public var pointIds: [Int16]

        init(_ r: BinaryReader, _ dataSize: Int) {
            reference = FormId<REFRRecord>(r.readLEUInt32())
            pointIds = [Int16](); let capacity = (dataSize - 4) >> 2; pointIds.reserveCapacity(capacity)
            for _ in 0..<capacity {
                pointIds.append(r.readLEInt16())
                r.skipBytes(2) // Unused (can merge back)
            }
        }
    }

    public override var description: String { return "PGRD: \(EDID)" }
    public var EDID: STRVField = STRVField.empty // Editor ID
    public var DATA: DATAField! // Number of nodes
    public var PGRPs: [PGRPField]!
    public var PGRC: UNKNField!
    public var PGAG: UNKNField!
    public var PGRRs: [PGRRField]! // Point-to-Point Connections
    public var PGRLs: [PGRLField]? = nil// Point-to-Reference Mappings
    public var PGRIs: [PGRIField]! // Inter-Cell Connections

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize, format)
        case "PGRP":
            PGRPs = [PGRPField](); let capacity = dataSize >> 4; PGRPs.reserveCapacity(capacity)
            for _ in 0..<capacity { PGRPs.append(PGRPField(r, 16)) }
        case "PGRC": PGRC = UNKNField(r, dataSize)
        case "PGAG": PGAG = UNKNField(r, dataSize)
        case "PGRR":
            PGRRs = [PGRRField](); let capacity = dataSize >> 2; PGRRs.reserveCapacity(capacity)
            for _ in 0..<capacity { PGRRs.append(PGRRField(r, 4)) }; r.skipBytes(dataSize % 4)
        case "PGRL": if PGRLs == nil { PGRLs = [PGRLField]() }; PGRLs!.append(PGRLField(r, dataSize))
        case "PGRI":
            PGRIs = [PGRIField](); let capacity = dataSize >> 4; PGRIs.reserveCapacity(capacity)
            for _ in 0..<capacity { PGRIs.append(PGRIField(r, 16)) }
        default: return false
        }
        return true
    }
}
