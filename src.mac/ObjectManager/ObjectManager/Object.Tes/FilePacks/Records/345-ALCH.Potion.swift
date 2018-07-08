//
//  ALCHRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ALCHRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public class DATAField {
        public let weight: Float
        public var value: Int32 = 0
        public var flags: Int32 = 0 //: AutoCalc

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            weight = r.readLESingle()
            if format == .TES3 {
                value = r.readLEInt32()
                flags = r.readLEInt32()
            }
        }

        func ENITField(_ r: BinaryReader, _ dataSize: Int) {
            value = r.readLEInt32()
            flags = Int32(r.readByte())
            r.skipBytes(3) // Unknown
        }
    }

    // TES3
    public struct ENAMField {
        public let effectId: Int16
        public let skillId: UInt8 // for skill related effects, -1/0 otherwise
        public let attributeId: UInt8 // for attribute related effects, -1/0 otherwise
        public let unknown1: Int32
        public let unknown2: Int32
        public let duration: Int32
        public let magnitude: Int32
        public let unknown4: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            effectId = r.readLEInt16()
            skillId = r.readByte()
            attributeId = r.readByte()
            unknown1 = r.readLEInt32()
            unknown2 = r.readLEInt32()
            duration = r.readLEInt32()
            magnitude = r.readLEInt32()
            unknown4 = r.readLEInt32()
        }
    }

    public override var description: String { return "ALCH: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Item Name
    public var DATA: DATAField! // Alchemy Data
    public var ENAM: ENAMField? = nil // Enchantment
    public var ICON: FILEField! // Icon
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script (optional)
    // TES4
    public var EFITs = [ENCHRecord.EFITField]() // Effect Data
    public var SCITs = [ENCHRecord.SCITField]() // Script Effect Data

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL": if SCITs.count == 0 { FULL = r.readSTRV(dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "FNAM": FULL = r.readSTRV(dataSize)
        case "DATA",
             "ALDT": DATA = DATAField(r, dataSize, format)
        case "ENAM": ENAM = ENAMField(r, dataSize)
        case "ICON",
             "TEXT": ICON = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        //
        case "ENIT": DATA.ENITField(r, dataSize)
        case "EFID": r.skipBytes(dataSize)
        case "EFIT": EFITs.append(ENCHRecord.EFITField(r, dataSize, format))
        case "SCIT": SCITs.append(ENCHRecord.SCITField(r, dataSize))
        default: return false
        }
        return true
    }
}
