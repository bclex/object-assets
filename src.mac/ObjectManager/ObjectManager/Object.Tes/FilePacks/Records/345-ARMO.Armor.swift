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
        public enum ARMOType {
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

    public var description: String { return "ARMO: \(EDID)" }
    public var EDID: STRVField  // Editor ID
    public var MODL: MODLGroup  // Male biped model
    public var FULL: STRVField // Item Name
    public var ICON: FILEField // Male icon
    public var DATA: DATAField // Armour Data
    public var SCRI: FMIDField<SCPTRecord>? // Script Name (optional)
    public var ENAM: FMIDField<ENCHRecord>? // Enchantment FormId (optional)
    // TES3
    public var INDXs = [CLOTRecord.INDXFieldGroup]() // Body Part Index
    // TES4
    public var BMDT: UI32Field // Flags
    public var MOD2: MODLGroup // Male world model (optional)
    public var MOD3: MODLGroup // Female biped (optional)
    public var MOD4: MODLGroup // Female world model (optional)
    public var ICO2: FILEField? // Female icon (optional)
    public var ANAM: IN16Field? // Enchantment points (optional)

    init() {
    }

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "DATA":
        case "AODT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "INDX": INDXs.append(CLOTRecord.INDXFieldGroup(INDX: INTVField(r, dataSize)))
        case "BNAM": INDXs.last!.BNAM = STRVField(r, dataSize)
        case "CNAM": INDXs.last!.CNAM = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "ENAM": ENAM = FMIDField<ENCHRecord>(r, dataSize)
        case "BMDT": BMDT = UI32Field(r, dataSize)
        case "MOD2": MOD2 = MODLGroup(r, dataSize)
        case "MO2B": MOD2.MODBField(r, dataSize)
        case "MO2T": MOD2.MODTField(r, dataSize)
        case "MOD3": MOD3 = MODLGroup(r, dataSize)
        case "MO3B": MOD3.MODBField(r, dataSize)
        case "MO3T": MOD3.MODTField(r, dataSize)
        case "MOD4": MOD4 = MODLGroup(r, dataSize)
        case "MO4B": MOD4.MODBField(r, dataSize)
        case "MO4T": MOD4.MODTField(r, dataSize)
        case "ICO2": ICO2 = FILEField(r, dataSize)
        case "ANAM": ANAM = IN16Field(r, dataSize)
        default: return false
        }
        return true
    }
}
