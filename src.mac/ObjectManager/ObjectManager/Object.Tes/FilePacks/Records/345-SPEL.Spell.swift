//
//  SPELRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SPELRecord: Record {
    // TESX
    public struct SPITField: CustomStringConvertible {
        public var description: String { return "\(type)" }
        // TES3: 0 = Spell, 1 = Ability, 2 = Blight, 3 = Disease, 4 = Curse, 5 = Power
        // TES4: 0 = Spell, 1 = Disease, 2 = Power, 3 = Lesser Power, 4 = Ability, 5 = Poison
        public let type: UInt32
        public let spellCost: Int32
        public let flags: UInt32 // 0x0001 = AutoCalc, 0x0002 = PC Start, 0x0004 = Always Succeeds
        // TES4
        public let spellLevel: Int32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            type = r.readLEUInt32()
            spellCost = r.readLEInt32()
            spellLevel = format != .TES3 ? r.readLEInt32() : 0
            flags = r.readLEUInt32()
        }
    }

    public override var description: String { return "SPEL: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var FULL: STRVField! // Spell name
    public var SPIT: SPITField! // Spell data
    public var EFITs = [ENCHRecord.EFITField]() // Effect Data
    // TES4
    public var SCITs = [ENCHRecord.SCITField]() // Script effect data

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "FULL": if SCITs.count == 0 { FULL = r.readSTRV(dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "FNAM": FULL = r.readSTRV(dataSize)
        case "SPIT",
             "SPDT": SPIT = SPITField(r, dataSize, format)
        case "EFID": r.skipBytes(dataSize)
        case "EFIT",
             "ENAM": EFITs.append(ENCHRecord.EFITField(r, dataSize, format))
        case "SCIT": SCITs.append(ENCHRecord.SCITField(r, dataSize))
        default: return false
        }
        return true
    }
}
