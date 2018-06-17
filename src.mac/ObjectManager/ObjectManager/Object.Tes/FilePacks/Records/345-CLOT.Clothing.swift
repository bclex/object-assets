//
//  CLOTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CLOTRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public struct DATAField {
        public enum CLOTType: UInt32 {
            case pants = 0, shoes, shirt, belt, robe, r_glove, l_glove, skirt, ring, amulet
        }

        public let value: Int32
        public let weight: Float
        //
        public let type: Int32
        public let enchantPts: Int16

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                type = r.readLEInt32()
                weight = r.readLESingle()
                value = r.readLEInt16()
                enchantPts = r.readLEInt16()
                return
            }
            value = r.readLEInt32()
            weight = r.readLESingle()
            type = 0
            enchantPts = 0
        }
    }

    public class INDXFieldGroup: CustomStringConvertible {
        public var description: String { return "\(INDX.value): \(BNAM.value)" }
        public var INDX: INTVField!
        public var BNAM: STRVField!
        public var CNAM: STRVField!
    }

    public var description: String { return "CLOT: \(EDID!)" }
    public var EDID: STRVField!  // Editor ID
    public var MODL: MODLGroup!  // Model Name
    public var FULL: STRVField! // Item Name
    public var DATA: DATAField! // Clothing Data
    public var ICON: FILEField! // Male Icon
    public var ENAM: STRVField! // Enchantment Name
    public var SCRI: FMIDField<SCPTRecord>! // Script Name
    // TES3
    public var INDXs = [INDXFieldGroup]() // Body Part Index (Moved to Race)
    // TES4
    public var BMDT: UI32Field!  // Clothing Flags
    public var MOD2: MODLGroup!  // Male world model (optional)
    public var MOD3: MODLGroup!  // Female biped (optional)
    public var MOD4: MODLGroup!  // Female world model (optional)
    public var ICO2: FILEField? = nil // Female icon (optional)
    public var ANAM: IN16Field? = nil // Enchantment points (optional)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL",
             "FNAM": FULL = STRVField(r, dataSize)
        case "DATA",
             "CTDT": DATA = DATAField(r, dataSize, format)
        case "ICON",
             "ITEX": ICON = FILEField(r, dataSize)
        case "INDX": INDXs.append(INDXFieldGroup(INDX: INTVField(r, dataSize)))
        case "BNAM": INDXs.last!.BNAM = STRVField(r, dataSize)
        case "CNAM": INDXs.last!.CNAM = STRVField(r, dataSize)
        case "ENAM": ENAM = STRVField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
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
