//
//  IDLERecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class IDLERecord: Record, IHaveEDID, IHaveMODL {
    public override var description: String { return "IDLE: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var MODL: MODLGroup? = nil
    public var CTDAs = [SCPTRecord.CTDAField]() // Conditions
    public var ANAM: BYTEField!
    public var DATAs: [FMIDField<IDLERecord>]!
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "MODL": MODL = MODLGroup(r, dataSize)
        case "MODB": MODL!.MODBField(r, dataSize)
        case "CTDA",
             "CTDT": CTDAs.append(SCPTRecord.CTDAField(r, dataSize, format))
        case "ANAM": ANAM = r.readT(dataSize)
        case "DATA":
            DATAs = [FMIDField<IDLERecord>](); let capacity = dataSize >> 2; DATAs.reserveCapacity(capacity)
            for _ in 0..<capacity { DATAs.append(FMIDField<IDLERecord>(r, 4)) }
        default: return false
        }
        return true
    }
}
