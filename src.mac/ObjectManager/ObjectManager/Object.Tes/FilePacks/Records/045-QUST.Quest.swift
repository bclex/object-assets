    //
//  QUSTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class QUSTRecord: Record {
    public struct DATAField {
        public let flags: UInt8
        public let priority: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            flags = r.readByte()
            priority = r.readByte()
        }
    }

    public var description: String { return "QUST: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var FULL: STRVField  // Item Name
    public var ICON: FILEField  // Icon
    public var DATA: DATAField  // Icon
    public var SCRI: FMIDField<SCPTRecord> // Script Name
    public var SCHR: SCPTRecord.SCHRField // Script Data
    public var SCDA: BYTVField // Compiled Script
    public var SCTX: STRVField // Script Source
    public var SCROs = [FMIDField<Record>]() // Global variable reference

    init() {
    }
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "FULL": FULL = STRVField(r, dataSize)
        case "ICON": ICON = FILEField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "CTDA": r.skipBytes((int)dataSize)
        case "INDX": r.skipBytes((int)dataSize)
        case "QSDT": r.skipBytes((int)dataSize)
        case "CNAM": r.skipBytes((int)dataSize)
        case "QSTA": r.skipBytes((int)dataSize)
        case "SCHR": SCHR = SCPTRecord.SCHRField(r, dataSize)
        case "SCDA": SCDA = BYTVField(r, dataSize)
        case "SCTX": SCTX = STRVField(r, dataSize)
        case "SCRO": SCROs.append(FMIDField<Record>(r, dataSize))
        default: return false
        }
        return true
    }
}
