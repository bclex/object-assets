//
//  EYESRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class EYESRecord: Record {
    public override var description: String { return "EYES: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var FULL: STRVField!
    public var ICON: FILEField!
    public var DATA: BYTEField! // Playable

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "FULL": FULL = r.readSTRV(dataSize)
        case "ICON": ICON = r.readSTRV(dataSize)
        case "DATA": DATA = r.readT(dataSize)
        default: return false
        }
        return true
    }
}
