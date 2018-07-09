//
//  EsmFile+Fields.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public protocol IHaveEDID {
    var EDID: STRVField { get }
}

public protocol IHaveMODL {
    var MODL: MODLGroup? { get }
}

public class MODLGroup: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public let value: String
    public var bound: Float? = nil
    public var textures: Data? = nil // Texture Files Hashes

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

public struct FormId32<TRecord> {
    public let id: UInt32
}

public struct FormId<TRecord>: CustomStringConvertible {
    public var description: String { return "\(type):\(name ?? "none")\(id ?? 0)" }
    public let id: UInt32?
    public let name: String?
    public var type: String { let r = "\(TRecord.self)"; return String(r[r.startIndex..<r.index(r.startIndex, offsetBy: 4)])  }
    
    init(_ id: UInt32) { self.id = id ; name = nil }
    init(_ name: String) { id = 0 ; self.name = name }
    init(_ id: UInt32, _ name: String) { self.id = id; self.name = name }
    func adding(name: String) -> FormId<TRecord> { return FormId<TRecord>(id!, name) }
}

public let ColorRef4_empty = ColorRef4(red: 0, green: 0, blue: 0, null: 0)
public func ColorRef4_toColor32(v: ColorRef4) -> CGColor { return CGColor(red: CGFloat(v.red), green: CGFloat(v.green), blue: CGFloat(v.blue), alpha: 255) }
public typealias ColorRef4 = (red: UInt8, green: UInt8, blue: UInt8, null: UInt8)
public typealias ColorRef3 = (red: UInt8, green: UInt8, blue: UInt8)


public let STRVField_empty = STRVField("")
public typealias STRVField = (String)
public typealias FILEField = (String)
public typealias INTVField = (Int64)
public typealias DATVField = (b: Bool?, i: Int32?, f: Float?, s: String?)
public typealias FLTVField = (Float)
public typealias BYTEField = (UInt8)
public typealias IN16Field = (Int16)
public typealias UI16Field = (UInt16)
public typealias IN32Field = (Int32)
public typealias UI32Field = (UInt32)

public struct FMIDField<TRecord>: CustomStringConvertible {
    public var description: String { return "\(value)" }
    public var value: FormId<TRecord>

    init(_ r: BinaryReader, _ dataSize: Int) {
        value = dataSize == 4 ?
            FormId<TRecord>(r.readLEUInt32()) :
            FormId<TRecord>(r.readASCIIString(dataSize, format: .zeroPadded))
    }

    mutating func add(name: String) {
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

public typealias CREFField = (ColorRef4)

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

public typealias BYTVField = (Data)
public typealias UNKNField = (Data)
