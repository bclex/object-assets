    //
//  DIALRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class DIALRecord: Record {
    internal static var LastRecord: DIALRecord

    public enum DIALType: UInt8 {
        case RegularTopic = 0
        case Voice = 1
        case Greeting = 2
        case Persuasion = 3
        case Journal = 4
    }

    public var description: String { return "DIAL: \(EDID)" }
    public STRVField EDID  // Editor ID
    public STRVField FULL // Dialogue Name
    public BYTEField DATA // Dialogue Type
    public List<FMIDField<QUSTRecord>> QSTIs // Quests (optional)
    public List<INFORecord> INFOs = List<INFORecord>() // Info Records

    init() {
    }

    override func createField(r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID",
             "NAME": EDID = STRVField(r, dataSize); LastRecord = self
        case "FULL": FULL = STRVField(r, dataSize)
        case "DATA": DATA = BYTEField(r, dataSize)
        case "QSTI",
             "QSTR": if QSTIs == nil { QSTIs = [FMIDField<QUSTRecord>]() }; QSTIs.append(FMIDField<QUSTRecord>(r, dataSize))
        default: return false
        }
        return true
    }
}
