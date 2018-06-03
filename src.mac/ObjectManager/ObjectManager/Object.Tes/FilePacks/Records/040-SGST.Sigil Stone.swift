//
//  SGSTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SGSTRecord: Record {
    public struct DATAField {
        public let uses: UInt8
        public let value: Int32
        public let weight: Float

        init(_ r: BinaryReader, _ dataSize: Init) {
            uses = r.readByte();
            value = r.readLEInt32();
            weight = r.readLESingle();
        }
    }

    public var description: String { return "SGST: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var MODL: MODLGroup // Model
    public var FULL: STRVField // Item Name
    public var DATA: DATAField // Sigil Stone Data
    public var ICON: FILEField // Icon
    public var SCRI: FMIDField<SCPTRecord>? // Script (optional)
    public var EFITs = [ENCHRecord.EFITField]() // Effect Data
    public var SCITs = [ENCHRecord.SCITField]() // Script Effect Data

    init() {
    }
    
    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL.MODBField(r, dataSize)
        case "MODT": MODL.MODTField(r, dataSize)
        case "FULL": if SCITs.count == 0 { FULL = STRVField(r, dataSize) } else { SCITs.last!.FULLField(r, dataSize) }
        case "DATA": DATA = DATAField(r, dataSize, format)
        case "ICON": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "EFID": r.skipBytes(dataSize)
        case "EFIT": EFITs.append(ENCHRecord.EFITField(r, dataSize, format))
        case "SCIT": SCITs.append(ENCHRecord.SCITField(r, dataSize))
        default: return false
        }
        return true
    }
}