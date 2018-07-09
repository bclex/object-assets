//
//  ACHRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ACHRRecord: Record {
    public override var description: String { return "ACHR: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var NAME: FMIDField<Record>! // Base
    public var DATA: REFRRecord.DATAField!  // Position/Rotation
    public var XPCI: FMIDField<CELLRecord>? = nil// Unused (optional)
    public var XLOD: BYTVField? = nil // Distant LOD Data (optional)
    public var XESP: REFRRecord.XESPField? = nil // Enable Parent (optional)
    public var XMRC: FMIDField<REFRRecord>? = nil// Merchant container (optional)
    public var XHRS: FMIDField<ACRERecord>? = nil// Horse (optional)
    public var XSCL: FLTVField? = nil// Scale (optional)
    public var XRGD: BYTVField? = nil// Ragdoll Data (optional)

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "NAME": NAME = FMIDField<Record>(r, dataSize)
        case "DATA": DATA = REFRRecord.DATAField(r, dataSize)
        case "XPCI": XPCI = FMIDField<CELLRecord>(r, dataSize)
        case "FULL": XPCI!.add(name: r.readASCIIString(dataSize))
        case "XLOD": XLOD = r.readBYTV(dataSize)
        case "XESP": XESP = REFRRecord.XESPField(r, dataSize)
        case "XMRC": XMRC = FMIDField<REFRRecord>(r, dataSize)
        case "XHRS": XHRS = FMIDField<ACRERecord>(r, dataSize)
        case "XSCL": XSCL = r.readT(dataSize)
        case "XRGD": XRGD = r.readBYTV(dataSize)
        default: return false;
        }
        return true
    }
}
