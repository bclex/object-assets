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

    public override var description: String { return "QUST: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var FULL: STRVField!  // Item Name
    public var ICON: FILEField!  // Icon
    public var DATA: DATAField!  // Icon
    public var SCRI: FMIDField<SCPTRecord>! // Script Name
    public var SCHR: SCPTRecord.SCHRField! // Script Data
    public var SCDA: BYTVField! // Compiled Script
    public var SCTX: STRVField! // Script Source
    public var SCROs = [FMIDField<Record>]() // Global variable reference
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "CTDA": r.skipBytes(dataSize)
        case "INDX": r.skipBytes(dataSize)
        case "QSDT": r.skipBytes(dataSize)
        case "CNAM": r.skipBytes(dataSize)
        case "QSTA": r.skipBytes(dataSize)
        case "SCHR": SCHR = SCPTRecord.SCHRField(r, dataSize)
        case "SCDA": SCDA = r.readBYTV(dataSize)
        case "SCTX": SCTX = r.readSTRV(dataSize)
        case "SCRO": SCROs.append(FMIDField<Record>(r, dataSize))
        default: return false
        }
        return true
    }
}
