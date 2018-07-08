//
//  PROBRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class PROBRecord: Record, IHaveEDID, IHaveMODL {
    public struct PBDTField {
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

    public override var description: String { return "PROB: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model Name
    public var FNAM: STRVField! // Item Name
    public var PBDT: PBDTField! // Probe Data
    public var ICON: FILEField! // Inventory Icon
    public var SCRI: FMIDField<SCPTRecord>! // Script Name

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch (type)
        {
        case "NAME": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "FNAM": FNAM = r.readSTRV(dataSize)
        case "PBDT": PBDT = PBDTField(r, dataSize)
        case "ITEX": ICON = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
