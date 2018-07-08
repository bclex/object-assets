//
//  ARMORecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ARMORecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField {
        public enum ARMOType: Int32 {
            case helmet = 0, cuirass, l_pauldron, r_pauldron, greaves, boots, l_gauntlet, r_gauntlet, shield, l_bracer, r_bracer
        }

        public let armour: Int16
        public let value: Int32
        public let health: Int32
        public let weight: Float
        // TES3
        public let type: Int32
        public let enchantPts: Int32

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                type = r.readLEInt32()
                weight = r.readLESingle()
                value = r.readLEInt32()
                health = r.readLEInt32()
                enchantPts = r.readLEInt32()
                armour = Int16(r.readLEInt32())
                return
            }
            armour = r.readLEInt16()
            value = r.readLEInt32()
            health = r.readLEInt32()
            weight = r.readLESingle()
            type = 0
            enchantPts = 0
        }
    }

    public override var description: String { return "ARMO: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Male biped model
    public var FULL: STRVField! // Item Name
    public var ICON: FILEField! // Male icon
    public var DATA: DATAField! // Armour Data
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script Name (optional)
    public var ENAM: FMIDField<ENCHRecord>? = nil// Enchantment FormId (optional)
    // TES3
    public var INDXs = [CLOTRecord.INDXFieldGroup]() // Body Part Index
    // TES4
    public var BMDT: UI32Field! // Flags
    public var MOD2: MODLGroup? = nil // Male world model (optional)
    public var MOD3: MODLGroup? = nil // Female biped (optional)
    public var MOD4: MODLGroup? = nil // Female world model (optional)
    public var ICO2: FILEField? = nil // Female icon (optional)
    public var ANAM: IN16Field? = nil // Enchantment points (optional)

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
             "AODT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = r.readSTRV(dataSize)
        case "INDX": INDXs.append(CLOTRecord.INDXFieldGroup(INDX: r.readINTV(dataSize)))
        case "BNAM": INDXs.last!.BNAM = r.readSTRV(dataSize)
        case "CNAM": INDXs.last!.CNAM = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "BMDT": BMDT = r.readT(dataSize)
        case "MOD2": MOD2 = MODLGroup(r, dataSize)
        case "MO2B": MOD2?.MODBField(r, dataSize)
        case "MO2T": MOD2?.MODTField(r, dataSize)
        case "MOD3": MOD3 = MODLGroup(r, dataSize)
        case "MO3B": MOD3?.MODBField(r, dataSize)
        case "MO3T": MOD3?.MODTField(r, dataSize)
        case "MOD4": MOD4 = MODLGroup(r, dataSize)
        case "MO4B": MOD4?.MODBField(r, dataSize)
        case "MO4T": MOD4?.MODTField(r, dataSize)
        case "ICO2": ICO2 = r.readSTRV(dataSize)
        case "ANAM": ANAM = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
