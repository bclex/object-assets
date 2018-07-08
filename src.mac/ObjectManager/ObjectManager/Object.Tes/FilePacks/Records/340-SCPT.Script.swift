//
//  SCPTRecord.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

public class SCPTRecord: Record {
    // TESX
    public struct CTDAField {
        public enum INFOType: UInt8 {
            case nothing = 0, function, global, local, journal, item, dead, notId, notFaction, notClass, notRace, notCell, notLocal
        }

        // TES3: 0 = [=], 1 = [!=], 2 = [>], 3 = [>=], 4 = [<], 5 = [<=]
        // TES4: 0 = [=], 2 = [!=], 4 = [>], 6 = [>=], 8 = [<], 10 = [<=]
        public let compareOp: UInt8 
        // (00-71) - sX = Global/Local/Not Local types, JX = Journal type, IX = Item Type, DX = Dead Type, XX = Not ID Type, FX = Not Faction, CX = Not Class, RX = Not Race, LX = Not Cell
        public let functionId: String
                                    // TES3
        public let index: UInt8 // (0-5)
        public let type: UInt8
        // Except for the function type, this is the ID for the global/local/etc. Is not nessecarily NULL terminated.The function type SCVR sub-record has
        public let name: String 
        // TES4
        public let comparisonValue: Float
        public let parameter1: Int32 // Parameter #1
        public let parameter2: Int32 // Parameter #2

        init(_ r: BinaryReader, _ dataSize: Int, _ format: GameFormatId) {
            guard format != .TES3 else {
                index = r.readByte()
                type = r.readByte()
                functionId = r.readASCIIString(2)
                compareOp = r.readByte() << 1
                name = r.readASCIIString(dataSize - 5)
                comparisonValue = 0; parameter1 = 0; parameter2 = 0
                return
            }
            compareOp = r.readByte();
            r.skipBytes(3) // Unused
            comparisonValue = r.readLESingle();
            functionId = r.readASCIIString(4);
            parameter1 = r.readLEInt32();
            parameter2 = r.readLEInt32();
            if dataSize == 24 {
                r.skipBytes(4) // Unused
            }
            index = 0; type = 0
            name = ""
        }
    }

    // TES3
    public class SCHDField: CustomStringConvertible {
        public var description: String { return "\(name)" }
        public let name: String
        public let numShorts: Int32
        public let numLongs: Int32
        public let numFloats: Int32
        public let scriptDataSize: Int32
        public let localVarSize: Int32
        public var variables: [String]? = nil

        init(_ r: BinaryReader, _ dataSize: Int) {
            name = r.readASCIIString(32, format: .zeroPadded)
            numShorts = r.readLEInt32()
            numLongs = r.readLEInt32()
            numFloats = r.readLEInt32()
            scriptDataSize = r.readLEInt32()
            localVarSize = r.readLEInt32()
            // SCVRField
            variables = nil
        }

        func SCVRField(_ r: BinaryReader, _ dataSize: Int) {
            variables = r.readASCIIMultiString(dataSize)
        }
    }

    // TES4
    public struct SCHRField: CustomStringConvertible {
        public var description: String { return "\(refCount)" }
        public let refCount: UInt32
        public let compiledSize: UInt32
        public let variableCount: UInt32
        public let type: UInt32 // 0x000 = Object, 0x001 = Quest, 0x100 = Magic Effect

        init(_ r: BinaryReader, _ dataSize: Int) {
            r.skipBytes(4) // Unused
            refCount = r.readLEUInt32()
            compiledSize = r.readLEUInt32()
            variableCount = r.readLEUInt32()
            type = r.readLEUInt32()
            guard dataSize != 20 else {
                return
            }    
            r.skipBytes(dataSize - 20)
        }
    }

    public class SLSDField: CustomStringConvertible {
        public var description: String { return "\(idx):\(variableName)" }
        public let idx: UInt32
        public let type: UInt32
        public var variableName: String

        init(_ r: BinaryReader, _ dataSize: Int) {
            idx = r.readLEUInt32()
            _ = r.readLEUInt32() // Unknown
            _ = r.readLEUInt32() // Unknown
            _ = r.readLEUInt32() // Unknown
            type = r.readLEUInt32()
            _ = r.readLEUInt32() // Unknown
            // SCVRField
            variableName = ""
        }

        func SCVRField(_ r: BinaryReader, _ dataSize: Int) {
            variableName = r.readASCIIString(dataSize, format: .possibleNullTerminated)
        }
    }

    public override var description: String { return "SCPT: \(!EDID.isEmpty ? EDID : SCHD.name)" }
    public var EDID: STRVField = STRVField_empty  // Editor ID
    public var SCDA: BYTVField! // Compiled Script
    public var SCTX: STRVField! // Script Source
    // TES3
    public var SCHD: SCHDField! // Script Data
    // TES4
    public var SCHR: SCHRField! // Script Data
    public var SLSDs = [SLSDField]() // Variable data
    public var SCRVs = [SLSDField]() // Ref variable data (one for each ref declared)
    public var SCROs = [FMIDField<Record>]() // Global variable reference

    override func createField(_ r: BinaryReader, for format: GameFormatId, type: String, dataSize: Int) -> Bool {
        switch type {
        case "EDID": EDID = r.readSTRV(dataSize)
        case "SCHD": SCHD = SCHDField(r, dataSize)
        case "SCVR": if format != .TES3 { SLSDs.last!.SCVRField(r, dataSize) } else { SCHD.SCVRField(r, dataSize) }
        case "SCDA",
             "SCDT": SCDA = r.readBYTV(dataSize)
        case "SCTX": SCTX = r.readSTRV(dataSize)
        // TES4
        case "SCHR": SCHR = SCHRField(r, dataSize)
        case "SLSD": SLSDs.append(SLSDField(r, dataSize))
        case "SCRO": SCROs.append(FMIDField<Record>(r, dataSize))
        case "SCRV": let idx = r.readLEUInt32(); SCRVs.append(SLSDs.first(where: { $0.idx == idx })!)
        default: return false
        }
        return true
    }
}
