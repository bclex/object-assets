//
//  INGRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class INGRRecord: Record, IHaveEDID, IHaveMODL {
    // TES3
    public struct IRDTField {
        public let weight: Float
        public let value: Int32
        public var effectId: [Int32] // 0 or -1 means no effect
        public var skillId: [Int32] // only for Skill related effects, 0 or -1 otherwise
        public var attributeId: [Int32] // only for Attribute related effects, 0 or -1 otherwise

        init(_ r: BinaryReader, _ dataSize: Int) {
            weight = r.readLESingle();
            value = r.readLEInt32();
            effectId = [Int32](); effectId.reserveCapacity(4)
            for _ in 0..<4 { effectId.append(r.readLEInt32()) }
            skillId = [Int32](); skillId.reserveCapacity(4)
            for _ in 0..<4 { skillId.append(r.readLEInt32()) }
            attributeId = [Int32](); attributeId.reserveCapacity(4)
            for _ in 0..<4 { attributeId.append(r.readLEInt32()) }
        }
    }

    // TES4
    public class DATAField {
        public let weight: Float
        public var value: Int32 = 0
        public var flags: UInt32 = 0

        init(_ r: BinaryReader, _ dataSize: Int) {
            weight = r.readLESingle()
        }

        func ENITField(_ r: BinaryReader, _ dataSize: Int) {
            value = r.readLEInt32()
            flags = r.readLEUInt32()
        }
    }

    public override var description: String { return "INGR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model Name
    public var FULL: STRVField! // Item Name
    public var IRDT: IRDTField! // Ingrediant Data //: TES3
    public var DATA: DATAField! // Ingrediant Data //: TES4
    public var ICON: FILEField! // Inventory Icon
    public var SCRI: FMIDField<SCPTRecord>! // Script Name
    // TES4
    public var EFITs = [ENCHRecord.EFITField]() // Effect Data
    public var SCITs = [ENCHRecord.SCITField]() // Script effect data

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL": if SCITs.count == 0 { FULL = r.readSTRV(dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "FNAM": FULL = r.readSTRV(dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "IRDT": IRDT = IRDTField(r, dataSize)
        case "ICON",
             "ITEX": ICON = r.readSTRV(dataSize)
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
