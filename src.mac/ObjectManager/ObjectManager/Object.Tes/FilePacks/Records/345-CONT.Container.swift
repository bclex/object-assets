//
//  CONTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class CONTRecord: Record, IHaveEDID, IHaveMODL {
    // TESX
    public class DATAField {
        public var flags: UInt8 // flags 0x0001 = Organic, 0x0002 = Respawns, organic only, 0x0008 = Default, unknown
        public let weight: Float

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                flags = 0
                weight = r.readLESingle()
                return
            }
            flags = r.readByte()
            weight = r.readLESingle()
        }

        func FLAGField(_ r: BinaryReader, _ dataSize: Int) {
            flags = UInt8(r.readLEUInt32())
        }
    }

    public override var description: String { return "CONT: \(EDID)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var MODL: MODLGroup? = nil // Model
    public var FULL: STRVField! // Container Name
    public var DATA: DATAField! // Container Data
    public var SCRI: FMIDField<SCPTRecord>? = nil
    public var CNTOs = [CNTOField]()
    // TES4
    public var SNAM: FMIDField<SOUNRecord>! // Open sound
    public var QNAM: FMIDField<SOUNRecord>! // Close sound
    
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
             "CNDT": DATA = DATAField(r, dataSize, format)
        case "FLAG": DATA.FLAGField(r, dataSize)
        case "CNTO",
             "NPCO": CNTOs.append(CNTOField(r, dataSize, for: format))
        case "SCRI": SCRI = FMIDField<SCPTRecord>(r, dataSize)
        case "SNAM": SNAM = FMIDField<SOUNRecord>(r, dataSize)
        case "QNAM": QNAM = FMIDField<SOUNRecord>(r, dataSize)
        default: return false
        }
        return true
    }
}
