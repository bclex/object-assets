//
//  GRASRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class GRASRecord: Record, IHaveEDID, IHaveMODL {
    public struct DATAField {
        public let density: UInt8
        public let minSlope: UInt8
        public let maxSlope: UInt8
        public let unitFromWaterAmount: UInt16
        public let unitFromWaterType: UInt32
        //Above - At Least,
        //Above - At Most,
        //Below - At Least,
        //Below - At Most,
        //Either - At Least,
        //Either - At Most,
        //Either - At Most Above,
        //Either - At Most Below
        public let positionRange: Float
        public let heightRange: Float
        public let colorRange: Float
        public let wavePeriod: Float
        public let flags: UInt8

        init(_ r: BinaryReader, _ dataSize: Int) {
            density = r.readByte()
            minSlope = r.readByte()
            maxSlope = r.readByte()
            r.skipBytes(1)
            unitFromWaterAmount = r.readLEUInt16()
            r.skipBytes(2)
            unitFromWaterType = r.readLEUInt32()
            positionRange = r.readLESingle()
            heightRange = r.readLESingle()
            colorRange = r.readLESingle()
            wavePeriod = r.readLESingle()
            flags = r.readByte()
            r.skipBytes(3)
        }
    }

    public override var description: String { return "GRAS: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil
    public var DATA: DATAField!
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        default: return false
        }
        return true
    }
}
