//
//  EsmFile+Fields.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public protocol IHaveEDID {
    var EDID: STRVField {get}
}

public protocol IHaveMODL {
    var MODL: MODLGroup {get}
}

public class MODLGroup: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: String
    public var bound: Float
    public var textures: Data // Texture Files Hashes

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readASCIIString(dataSize, format: .possibleNullTerminated)
    }

    func MODBField(_ r: BinaryReader, _ dataSize: Int) {
        bound = r.readLESingle()
    }

    func MODTField(_ r: BinaryReader, _ dataSize: Int) {
        textures = r.readBytes(dataSize)
    }
}

public struct STRVField: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: String

    init(_ r: BinaryReader, _ dataSize: Int, format: ASCIIFormat = .possibleNullTerminated) {
        value = r.readASCIIString(dataSize, format: format)
    }
}

public struct FILEField: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: String

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readASCIIString(dataSize, format: .possibleNullTerminated)
    }
}

public struct INTVField: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: Int64

    init(_ r: BinaryReader, _ dataSize: Int) {
        switch dataSize {
            case 1: value = Int64(r.readByte())
            case 2: value = Int64(r.readLEInt16())
            case 4: value = Int64(r.readLEInt32())
            case 8: value = Int64(r.readLEInt64())
            default: fatalError("Tried to read an INTV subrecord with an unsupported size (\(dataSize))")
        }
    }
    
    var toUI16Field: UI16Field { return UI16Field(value: UInt16(value)) }
}

public struct DATVField: CustomStringConvertible {
    public var description: String { return "DATV" }
    public let valueB : Bool? = nil
    public let valueI : Int32? = nil
    public let valueF : Float? = nil
    public let valueS : String? = nil

    init(_ r: BinaryReader, _ dataSize: Int, type: Character) {
        switch type {
            case "b": valueB = r.readLEInt32() != 0
            case "i": valueI = r.readLEInt32()
            case "f": valueF = r.readLESingle()
            case "s": valueS = r.readASCIIString(dataSize, format: .possibleNullTerminated)
            default: fatalError("\(type)")
        }
    }
}

public struct FLTVField: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: Float

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readLESingle()
    }
}

public struct BYTEField: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: UInt8

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readByte()
    }
}

public struct IN16Field: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: Int16

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readLEInt16()
    }
}

public struct UI16Field: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: UInt16

    init(value: UInt16) { self.value = value }
    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readLEUInt16();
    }
}

public struct IN32Field: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: Int32

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readLEInt32()
    }
}

public struct UI32Field: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: UInt32

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readLEUInt32()
    }
}

public struct FormId<TRecord>: CustomStringConvertible {
    public var description: String { return "\(type):\(name ?? "none")\(id ?? 0)" }
    public let id: UInt32?
    public let name: String?
    public var type: String { let r = "\(TRecord.self)" ; return String(r[r.startIndex..<r.index(r.startIndex, offsetBy: 4)])  }

    init(_ id: UInt32) { self.id = id ; name = nil }
    init(_ name: String) { id = 0 ; self.name = name }
    init(_ id: UInt32, _ name: String) { self.id = id ; self.name = name }
    func adding(name: String) -> FormId<TRecord> { return FormId<TRecord>(id!, name) }
}

public struct FMIDField<TRecord>: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public var value: FormId<TRecord>

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = dataSize == 4 ?
            FormId<TRecord>(r.readLEUInt32()) :
            FormId<TRecord>(r.readASCIIString(dataSize, format: .zeroPadded))
    }

    mutating func addName(name: String) {
        value = value.adding(name: name)
    }
}

public struct FMID2Field<TRecord>: CustomStringConvertible {
    public var description: String { return "\(value1)x\(value2)" }
    public let value1: FormId<TRecord>
    public let value2: FormId<TRecord>

    init(_ r: BinaryReader, _ dataSize: Int) {
        value1 = FormId<TRecord>(r.readLEUInt32())
        value2 = FormId<TRecord>(r.readLEUInt32())
    }
}

public struct ColorRef: CustomStringConvertible {
    public var description: String { return "\(red):\(green):\(blue)" }
    public let red: UInt8
    public let green: UInt8
    public let blue: UInt8
    public let nullByte: UInt8

    init(red: UInt8, green: UInt8, blue: UInt8) {
        self.red = red
        self.green = green
        self.blue = blue
        nullByte = 255
    }

    init(_ r: BinaryReader, _ dataSize: Int) {
        red = r.readByte()
        green = r.readByte()
        blue = r.readByte()
        nullByte = r.readByte()
    }

    //func toColor32() -> CGColor { return CGColor(red: red, green: green, blue: blue, alpha: 255) }
}

public struct CREFField: CustomStringConvertible {
    public var description: String { return "\(color)" }
    public let color: ColorRef

    init(_ r: BinaryReader, _ dataSize: Int) {
        color = ColorRef(r, dataSize)
    }
}

public struct CNTOField: CustomStringConvertible {
    public var description: String { return "\(item)" }
    public let itemCount: UInt32 // Number of the item
    public let item: FormId<Record> // The ID of the item

    init(_ r: BinaryReader, _ dataSize: Int, for format: GameFormatId)
    {
        if format == .TES3 {
            itemCount = r.readLEUInt32()
            item = FormId<Record>(r.readASCIIString(32, format: .zeroPadded))
            return;
        }
        item = FormId<Record>(r.readLEUInt32())
        itemCount = r.readLEUInt32()
    }
}

public struct BYTVField: CustomStringConvertible {
    public var description: String { return "BYTS" }
    public let value: Data

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readBytes(dataSize);
    }
}

public struct UNKNField: CustomStringConvertible {
    public var description: String { return "UNKN" }
    public let value: Data

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = r.readBytes(dataSize)
    }
}
