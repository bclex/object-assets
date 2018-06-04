﻿//
//  APPARecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class APPARecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField {
        public let type: UInt8 // 0 = Mortar and Pestle, 1 = Albemic, 2 = Calcinator, 3 = Retort
        public let quality: Float
        public let weight: Float
        public let value: Int32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                type = (byte)r.readLEInt32()
                quality = r.readLESingle()
                weight = r.readLESingle()
                value = r.readLEInt32()
                return
            }
            type = r.readByte()
            value = r.readLEInt32()
            weight = r.readLESingle()
            quality = r.readLESingle()
        }
    }

    public var description: String { return "APPA: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var MODL: MODLGroup  // Model Name
    public var FULL: STRVField // Item Name
    public var DATA: DATAField // Alchemy Data
    public var ICON: FILEField // Inventory Icon
    public var FMIDField<SCPTRecord> SCRI // Script Name

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "DATA":
        case "AADT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}