//
//  ACRERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class ACRERecord: Record {
    public override var description: String { return "GMST: \(EDID)" }
    public var EDID: STRVField // Editor ID
    public var NAME: FMIDField<Record> // Base
    public var DATA: REFRRecord.DATAField // Position/Rotation
    public var XOWNs: [CELLRecord.XOWNGroup]? // Ownership (optional)
    public var XESP: REFRRecord.XESPField? // Enable Parent (optional)
    public var XSCL: FLTVField? // Scale (optional)
    public var XRGD: BYTVField? // Ragdoll Data (optional)

    init() {
    }
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = STRVField(r, dataSize)
        case "NAME": NAME = FMIDField<Record>(r, dataSize)
        case "DATA": DATA = REFRRecord.DATAField(r, dataSize)
        case "XOWN":
            if XOWNs == nil { XOWNs = [CELLRecord.XOWNGroup]() }; XOWNs!.append(CELLRecord.XOWNGroup(XOWN: FMIDField<Record>(r, dataSize)))
        case "XRNK": XOWNs!.last!.XRNK = IN32Field(r, dataSize)
        case "XGLB": XOWNs!.last!.XGLB = FMIDField<Record>(r, dataSize)
        case "XESP": XESP = REFRRecord.XESPField(r, dataSize)
        case "XSCL": XSCL = FLTVField(r, dataSize)
        case "XRGD": XRGD = BYTVField(r, dataSize)
        default: return false
        }
        return true
    }
}
