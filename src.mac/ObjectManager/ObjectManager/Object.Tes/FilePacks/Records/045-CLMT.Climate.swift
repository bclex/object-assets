//
//  CLMTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CLMTRecord: Record, IHaveEDID, IHaveMODL {
    public struct WLSTField {
        public let weather: FormId<WTHRRecord>
        public let chance: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            weather = FormId<WTHRRecord>(r.readLEUInt32())
            chance = r.readLEInt32()
        }
    }

    public struct TNAMField {
        public let sunrise_begin: UInt8
        public let sunrise_end: UInt8
        public let sunset_begin: UInt8
        public let sunset_end: UInt8
        public let volatility: UInt8
        public let moonsPhaseLength: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            sunrise_begin = r.readByte()
            sunrise_end = r.readByte()
            sunset_begin = r.readByte()
            sunset_end = r.readByte()
            volatility = r.readByte()
            moonsPhaseLength = r.readByte()
        }
    }

    public override var description: String { return "CLMT: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FNAM: FILEField! // Sun Texture
    public var GNAM: FILEField! // Sun Glare Texture
    public var WLSTs = [WLSTField]() // Climate
    public var TNAM: TNAMField! // Timing
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "FNAM": FNAM = r.readSTRV(dataSize)
        case "GNAM": GNAM = r.readSTRV(dataSize)
        case "WLST": for _ in 0..<(dataSize >> 3) { WLSTs.append(WLSTField(r, dataSize)) }
        case "TNAM": TNAM = TNAMField(r, dataSize)
        default: return false
        }
        return true
    }
}
