//
//  SBSPRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SBSPRecord: Record {
    public struct DNAMField {
        public let x: Float // X dimension
        public let y: Float // Y dimension
        public let z: Float // Z dimension

        init(_ r: BinaryReader, _ dataSize: Int) {
            x = r.readLESingle()
            y = r.readLESingle()
            z = r.readLESingle()
        }
    }

    public override var description: String { return "SBSP: \(EDID)" }
    public var EDID: STRVField = STRVField_empty // Editor ID
    public var DNAM: DNAMField!
    
    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "DNAM": DNAM = DNAMField(r, dataSize)
        default: return false
        }
        return true
    }
}
