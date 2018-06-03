//
//  WEAPRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class WEAPRecord: Record, IHaveEDID, IHaveMODL {
    public struct DATAField {
        public enum WEAPType {
            case shortBladeOneHand = 0
            case longBladeOneHand = 1
            case longBladeTwoClose = 2
            case bluntOneHand = 3
            case bluntTwoClose = 4
            case bluntTwoWide = 5
            case spearTwoWide = 6
            case axeOneHand = 7
            case axeTwoHand = 8
            case marksmanBow = 9
            case marksmanCrossbow = 10
            case marksmanThrown = 11
            case arrow = 12
            case bolt = 13
        }

        public let weight: Float
        public let value: Int32
        public let type: UInt16
        public let health: Int16
        public let speed: Float
        public let reach: Float
        public let damage: Int16 //: EnchantPts;
        public let chopMin: UInt8
        public let chopMax: UInt8
        public let slashMin: UInt8
        public let slashMax: UInt8
        public let thrustMin: UInt8
        public let thrustMax: UInt8
        public let flags: Int32 // 0 = ?, 1 = Ignore Normal Weapon Resistance?

        init(r: BinaryReader, dataSize: UInt32, forFormat: GameFormatId) {
            if forFormat == .TES3 {
                weight = r.readLESingle()
                value = r.readLEInt32()
                type = r.readLEUInt16()
                health = r.readLEInt16()
                speed = r.readLESingle()
                reach = r.readLESingle()
                damage = r.readLEInt16()
                chopMin = r.readByte()
                chopMax = r.readByte()
                slashMin = r.readByte()
                slashMax = r.readByte()
                thrustMin = r.readByte()
                thrustMax = r.readByte()
                flags = r.readLEInt32()
                return;
            }
            type = UInt16(r.readLEUInt32())
            speed = r.readLESingle()
            reach = r.readLESingle()
            flags = r.readLEInt32()
            value = r.readLEInt32()
            health = Int16(r.readLEInt32())
            weight = r.readLESingle()
            damage = r.readLEInt16()
            chopMin = chopMax = slashMin = slashMax = thrustMin = thrustMax = 0
        }
    }

    public var description: String { return "WEAP: \(EDID)" }
    public let EDID: STRVField // Editor ID
    public let MODL: MODLGroup // Model
    public let FULL: STRVField // Item Name
    public let DATA: DATAField // Weapon Data
    public let ICON: FILEField // Male Icon (optional)
    public let ENAM: FMIDField<ENCHRecord> // Enchantment ID
    public let SCRI: FMIDField<SCPTRecord> // Script (optional)
    // TES4
    public let IN16Field? ANAM // Enchantment points (optional)

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
        case "DATA",
             "WPDT": DATA = DATAField(r, dataSize, formatId)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ANAM": ANAM = IN16Field(r, dataSize)
        default: return false
        }
        return true
    }
}