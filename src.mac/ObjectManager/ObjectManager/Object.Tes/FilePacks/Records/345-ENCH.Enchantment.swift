//
//  ENCHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ENCHRecord: Record {
    // TESX
    public struct ENITField
    {
        // TES3: 0 = Cast Once, 1 = Cast Strikes, 2 = Cast when Used, 3 = Constant Effect
        // TES4: 0 = Scroll, 1 = Staff, 2 = Weapon, 3 = Apparel
        public let type: Int32
        public let enchantCost: Int32
        public let chargeAmount: Int32 //: Charge
        public let flags: Int32 //: AutoCalc

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            type = r.readLEInt32()
            if format == .TES3 {
                enchantCost = r.readLEInt32()
                chargeAmount = r.readLEInt32()
            }
            else {
                chargeAmount = r.readLEInt32()
                enchantCost = r.readLEInt32()
            }
            flags = r.readLEInt32()
        }
    }

    public class EFITField {
        public let effectId: String
        public let type: Int32 //:RangeType - 0 = Self, 1 = Touch, 2 = Target
        public let area: Int32
        public let duration: Int32
        public let magnitudeMin: Int32
        // TES3
        public var skillId: UInt8 = 0 // (-1 if NA)
        public var attributeId: UInt8 = 0 // (-1 if NA)
        public var magnitudeMax: Int32 = 0
        // TES4
        public var actorValue: Int32 = 0

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                effectId = r.readASCIIString(2)
                skillId = r.readByte()
                attributeId = r.readByte()
                type = r.readLEInt32()
                area = r.readLEInt32()
                duration = r.readLEInt32()
                magnitudeMin = r.readLEInt32()
                magnitudeMax = r.readLEInt32()
                return
            }
            effectId = r.readASCIIString(4)
            magnitudeMin = r.readLEInt32()
            area = r.readLEInt32()
            duration = r.readLEInt32()
            type = r.readLEInt32()
            actorValue = r.readLEInt32()
        }
    }

    // TES4
    public class SCITField {
        public var name: String
        public let scriptFormId: Int32
        public var school: Int32 = 0// 0 = Alteration, 1 = Conjuration, 2 = Destruction, 3 = Illusion, 4 = Mysticism, 5 = Restoration
        public var visualEffect: String = ""
        public var flags: UInt32 = 0

        init(_ r: BinaryReader, _ dataSize: Int) {
            name = "Script Effect"
            scriptFormId = r.readLEInt32()
            guard dataSize != 4 else {
                return
            }
            school = r.readLEInt32()
            visualEffect = r.readASCIIString(4)
            flags = dataSize > 12 ? r.readLEUInt32() : 0
        }

        func FULLField(_ r: BinaryReader, _ dataSize: Int) {
            name = r.readASCIIString(dataSize, format: .possibleNullTerminated)
        }
    }

    public override var description: String { return "ENCH: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var FULL: STRVField! // Enchant name
    public var ENIT: ENITField! // Enchant Data
    public var EFITs = [EFITField]() // Effect Data
    // TES4
    public var SCITs = [SCITField]() // Script effect data

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "FULL": if SCITs.count == 0 { FULL = r.readSTRV(dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "ENIT",
             "ENDT": ENIT = ENITField(r, dataSize, format)
        case "EFID": r.skipBytes(dataSize)
        case "EFIT",
             "ENAM": EFITs.append(EFITField(r, dataSize, format))
        case "SCIT": SCITs.append(SCITField(r, dataSize))
        default: return false
        }
        return true
    }
}
