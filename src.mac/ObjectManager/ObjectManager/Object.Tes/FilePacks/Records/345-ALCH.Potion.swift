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
        public let value: Int32
        public let flags: Int32 //: AutoCalc

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            weight = r.readLESingle()
            if format == .TES3 {
                value = r.readLEInt32()
                flags = r.readLEInt32()
            }
        }

        func ENITField(_ r: BinaryReader, _ dataSize: Int) {
            value = r.readLEInt32()
            flags = r.readByte()
            r.skipBytes(3) // Unknown
        }
    }

    // TES3
    public struct ENAMField {
        public let effectId: UInt16
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

    public var description: String { return "ALCH: \(EDID)" }
    public EDID: STRVField  // Editor ID
    public MODL: MODLGroup  // Model
    public FULL: STRVField // Item Name
    public DATA: DATAField // Alchemy Data
    public ENAM: ENAMField? // Enchantment
    public ICON: FILEField // Icon
    public SCRI: FMIDField<SCPTRecord>? // Script (optional)
    // TES4
    public EFITs = [ENCHRecord.EFITField]() // Effect Data
    public SCITs = [ENCHRecord.SCITField]() // Script Effect Data

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL": if SCITs.count == 0 { FULL = STRVField(r, dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "FNAM": FULL = STRVField(r, dataSize)
        case "DATA":
        case "ALDT": DATA = DATAField(r, dataSize, format)
        case "ENAM": ENAM = ENAMField(r, dataSize)
        case "ICON":
        case "TEXT": ICON = FILEField(r, dataSize)
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