//
//  ACHRRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ACHRRecord: Record {
    public var description: String { return "ACHR: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var NAME: FMIDField<Record> // Base
    public var DATA: REFRRecord.DATAField  // Position/Rotation
    public var XPCI: FMIDField<CELLRecord>? // Unused (optional)
    public var XLOD: BYTVField? // Distant LOD Data (optional)
    public var XESP: REFRRecord.XESPField? // Enable Parent (optional)
    public var XMRC: FMIDField<REFRRecord>? // Merchant container (optional)
    public var XHRS: FMIDField<ACRERecord>? // Horse (optional)
    public var XSCL: FLTVField? // Scale (optional)
    public var XRGD: BYTVField? // Ragdoll Data (optional)

    init() {
    }
    
    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "NAME": NAME = FMIDField<Record>(r, dataSize)
        case "DATA": DATA = REFRRecord.DATAField(r, dataSize)
        case "XPCI": XPCI = FMIDField<CELLRecord>(r, dataSize)
        case "FULL": XPCI.addName(r.readASCIIString(dataSize))
        case "XLOD": XLOD = BYTVField(r, dataSize)
        case "XESP": XESP = REFRRecord.XESPField(r, dataSize)
        case "XMRC": XMRC = FMIDField<REFRRecord>(r, dataSize)
        case "XHRS": XHRS = FMIDField<ACRERecord>(r, dataSize)
        case "XSCL": XSCL = FLTVField(r, dataSize)
        case "XRGD": XRGD = BYTVField(r, dataSize)
        default: return false;
        }
        return true
    }
}
