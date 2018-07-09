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
        public enum WEAPType: UInt16 {
            case shortBladeOneHand = 0, longBladeOneHand, longBladeTwoClose, bluntOneHand, bluntTwoClose, bluntTwoWide, spearTwoWide, axeOneHand, axeTwoHand, marksmanBow, marksmanCrossbow, marksmanThrown, arrow, bolt
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

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
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
                return
            }
            type = UInt16(r.readLEUInt32())
            speed = r.readLESingle()
            reach = r.readLESingle()
            flags = r.readLEInt32()
            value = r.readLEInt32()
            health = Int16(r.readLEInt32())
            weight = r.readLESingle()
            damage = r.readLEInt16()
            chopMin = 0; chopMax = 0; slashMin = 0; slashMax = 0; thrustMin =  0; thrustMax = 0
        }
    }

    public override var description: String { return "WEAP: \(String(describing: EDID))" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Item Name
    public var DATA: DATAField! // Weapon Data
    public var ICON: FILEField? = nil // Male Icon (optional)
    public var ENAM: FMIDField<ENCHRecord>! // Enchantment ID
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script (optional)
    // TES4
    public var ANAM: IN16Field? // Enchantment points (optional)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = r.readSTRV(dataSize)
        case "DATA",
             "WPDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = r.readSTRV(dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ANAM": ANAM = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
