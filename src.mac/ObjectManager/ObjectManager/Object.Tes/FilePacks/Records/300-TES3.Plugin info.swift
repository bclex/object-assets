//
//  TES3Record.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class TES3Record: Record {
    public struct HEDRField: CustomStringConvertible {
        public var description: String { return "TES3" }
        public let version: Float
        public let fileType: UInt32
        public let companyName: String
        public let fileDescription: String
        public let numRecords: UInt32

        init(_ r: BinaryReader, _ dataSize: Int) {
            version = r.readLESingle()
            fileType = r.readLEUInt32()
            companyName = r.readASCIIString(32, format: .zeroPadded)
            fileDescription = r.readASCIIString(256, format: .zeroPadded)
            numRecords = r.readLEUInt32()
        }
    }

    public var HEDR: HEDRField? = nil
    public var MASTs: [STRVField]? = nil
    public var DATAs: [INTVField]? = nil

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        guard format == .TES3 else {
            return false
        }
        switch type {
        case "HEDR": HEDR = HEDRField(r, dataSize)
        case "MAST": if MASTs == nil { MASTs = [STRVField]() }; MASTs!.append(r.readSTRV(dataSize))
        case "DATA": if DATAs == nil { DATAs = [INTVField]() }; DATAs!.append(r.readINTV(dataSize))
        default: return false
        }
        return true
    }
}
