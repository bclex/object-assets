    //
//  DIALRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class DIALRecord: Record {
    internal static var lastRecord: DIALRecord

    public enum DIALType: UInt8 {
        case regularTopic = 0, voice, greeting, persuasion, journal
    }

    public override var description: String { return "DIAL: \(EDID!)" }
    public var EDID: STRVField!  // Editor ID
    public var FULL: STRVField! // Dialogue Name
    public var DATA: BYTEField! // Dialogue Type
    public var QSTIs: [FMIDField<QUSTRecord>]? = nil // Quests (optional)
    public var INFOs = [INFORecord]() // Info Records

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize); lastRecord = self
        case "FULL": FULL = STRVField(r, dataSize)
        case "DATA": DATA = BYTEField(r, dataSize)
        case "QSTI",
             "QSTR": if QSTIs == nil { QSTIs = [FMIDField<QUSTRecord>]() }; QSTIs?.append(FMIDField<QUSTRecord>(r, dataSize))
        default: return false
        }
        return true
    }
}
