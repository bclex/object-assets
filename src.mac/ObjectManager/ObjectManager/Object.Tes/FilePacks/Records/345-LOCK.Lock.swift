//
//  LOCKRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class LOCKRecord: Record, IHaveEDID, IHaveMODL {
    public struct LKDTField {
        public let weight: Float
        public let value: Int32
        public let quality: Float
        public let uses: Int32

        init(_ r: BinaryReader, _ dataSize: Int) {
            weight = r.readLESingle()
            value = r.readLEInt32()
            quality = r.readLESingle()
            uses = r.readLEInt32()
        }
    }

    public var description: String { return "LOCK: \(EDID)" }
    public STRVField EDID // Editor ID
    public MODLGroup MODL // Model Name
    public STRVField FNAM // Item Name
    public LKDTField LKDT // Lock Data
    public FILEField ICON // Inventory Icon
    public FMIDField<SCPTRecord> SCRI // Script Name

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard formatId == GameFormatId.TES3 else {
            return false
        }
        switch type {
        case "NAME": EDID = STRVField(r, dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = STRVField(r, dataSize)
        case "LKDT": LKDT = LKDTField(r, dataSize)
        case "ITEX": ICON = FILEField(r, dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}