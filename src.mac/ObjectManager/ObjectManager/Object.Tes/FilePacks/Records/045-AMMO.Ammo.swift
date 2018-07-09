//
//  AMMORecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class AMMORecord: Record, IHaveEDID, IHaveMODL {
    public struct DATAField {
        public let speed: Float
        public let flags: UInt32
        public let value: UInt32
        public let weight: Float
        public let damage: UInt16

        init(_ r: BinaryReader, _ dataSize: Int) {
            speed = r.readLESingle()
            flags = r.readLEUInt32()
            value = r.readLEUInt32()
            weight = r.readLESingle()
            damage = r.readLEUInt16()
        }
    }

    public override var description: String { return "AMMO: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Item Name
    public var ICON: FILEField? = nil// Male Icon (optional)
    public var ENAM: FMIDField<ENCHRecord>? = nil // Enchantment ID (optional)
    public var ANAM: IN16Field? = nil // Enchantment points (optional)
    public var DATA: DATAField! // Ammo Data
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "ANAM": ANAM = r.readT(dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        default: return false
        }
        return true
    }
}
