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

    public typealias PGRPField = (
        point: Float3,
        connections: UInt8,
        pad01: UInt8,
        pad02: UInt16
    )

    public typealias PGRRField = (
        startPointId: Int16,
        endPointId: Int16
    )

    public typealias PGRIField = (
        pointId: Int16,
        pad02: UInt16,
        foreignNode: Float3
    )

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
    public var EDID: STRVField = STRVField_empty // Editor ID
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
             "NAME": EDID = r.readSTRV(dataSize)
        case "DATA": DATA = DATAField(r, dataSize, format)
        case "PGRP": PGRPs = r.readTArray(dataSize, count: dataSize >> 4)
        case "PGRC": PGRC = r.readBYTV(dataSize)
        case "PGAG": PGAG = r.readBYTV(dataSize)
        case "PGRR": PGRRs = r.readTArray(dataSize, count: dataSize >> 2) //r.skipBytes(dataSize % 4)
        case "PGRL": if PGRLs == nil { PGRLs = [PGRLField]() }; PGRLs!.append(PGRLField(r, dataSize))
        case "PGRI": PGRIs = r.readTArray(dataSize, count: dataSize >> 4)
        default: return false
        }
        return true
    }
}
