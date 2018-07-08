//
//  KEYMRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class KEYMRecord: Record, IHaveEDID, IHaveMODL {
    public struct DATAField {
        public let value: Int32
        public let weight: Float

        init(_ r: BinaryReader, _ dataSize: Int) {
            value = r.readLEInt32()
            weight = r.readLESingle()
        }
    }

    public override var description: String { return "KEYM: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Item Name
    public var SCRI: FMIDField<SCPTRecord>? = nil // Script (optional)
    public var DATA: DATAField! // Type of soul contained in the gem
    public var ICON: FILEField! // Icon (optional)
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "MODT": MODL!.MODTField(r, dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "DATA": DATA = DATAField(r, dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        default: return false
        }
        return true
    }
}
