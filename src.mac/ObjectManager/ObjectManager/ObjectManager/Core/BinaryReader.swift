//
//  BinaryReader.swift
//  CocoApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import simd

public enum ASCIIFormat {
    case raw, possibleNullTerminated, zeroPadded
}

public protocol BaseStream {
    func close()
    func readData(ofLength: Int) -> Data
    var position: UInt64 {get set}
    var length: UInt64 {get}
}

public class FileBaseStream: BaseStream {
    public let length: UInt64
    public var position: UInt64 {
        get { return data.offsetInFile }
        set(newValue) { data.seek(toFileOffset: newValue) }
    }
    let data: FileHandle
    
    public func close() { data.closeFile() }
    public func readData(ofLength: Int) -> Data { return data.readData(ofLength: ofLength) }
   
    public init?(path: String) {
        let attribs = try! FileManager.default.attributesOfItem(atPath: path)
        let length = UInt64(attribs[.size]! as! UInt32)
//        debugPrint("opening", path, length)
        data = FileHandle(forReadingAtPath: path)!
        self.length = length
    }
}

public class DataBaseStream: BaseStream {
    public var length: UInt64 { return UInt64(data.count) }
    public var position: UInt64
    let data: Data
    
    init(data: Data) {
        self.data = data
        position = 0
    }
    
    public func close() {
    }
    
    public func readData(ofLength: Int) -> Data {
        let startIndex = data.startIndex.advanced(by: Int(position))
        position += UInt64(ofLength)
        return data[startIndex..<(startIndex + ofLength)]
    }
}

public class BinaryReader {
    public var baseStream: BaseStream!
    
    init(_ stream: BaseStream) {
        baseStream = stream
    }
    
    deinit {
        close()
    }
    
    func close() {
        baseStream?.close()
        baseStream = nil
    }

    func readByte() -> UInt8 {
        return baseStream.readData(ofLength: 1).first!
    }
    
    public func readSByte() -> Int8 {
        return Int8(bitPattern: baseStream.readData(ofLength: 1).first!)
    }
    
    public func read(_ data: inout Data, offsetBy: Int, count: Int) {
        let newData = baseStream.readData(ofLength: count)
        let range = data.index(data.startIndex, offsetBy: offsetBy)..<data.index(data.startIndex, offsetBy: offsetBy + newData.count)
        data.replaceSubrange(range, with: newData)
    }
    
    public func skipBytes(_ count: Int) {
        _ = baseStream.readData(ofLength: count)
    }

    public func readBytes(_ count: Int) -> Data {
        return baseStream.readData(ofLength: count)
    }
    
    public func readRestOfBytes() -> Data {
        let remainingByteCount = baseStream.length - baseStream.position
        assert(remainingByteCount <= Int.max)
        return readBytes(Int(remainingByteCount))
    }

    public func readRestOfBytes(_ data: inout Data, offsetBy: Int) {
        let remainingByteCount = baseStream.length - baseStream.position
        assert(offsetBy >= 0 && remainingByteCount <= Int.max && offsetBy + Int(remainingByteCount) <= data.count)
        read(&data, offsetBy: offsetBy, count: Int(remainingByteCount))
    }
    
// MARK: A

    public func readASCIIString(_ length: Int, format: ASCIIFormat = .raw) -> String {
        var data = baseStream.readData(ofLength: length)
        switch format {
        case .raw: break
        case .possibleNullTerminated:
            if data.last! == 0 {
                data.removeLast()
            }
        case .zeroPadded:
            for idx in (data.startIndex..<data.endIndex).reversed() {
                if data[idx] != 0 {
                    data = data[data.startIndex...idx]
                    break
                }
            }
        }
        return String(data: data, encoding: .ascii)!
    }
    
    public func readASCIIMultiString(_ length: Int, bufSize: Int = 64) -> [String] {
        var list = [String]()
        var data = [UInt8](); data.reserveCapacity(bufSize)
        var len = length
        while len > 0 {
            data.removeAll()
            len -= 1
            var c = readByte()
            while len > 0 && c != 0 {
                data.append(c)
                len -= 1
                c = readByte()
            }
            list.append(String(bytes: data, encoding: .ascii)!)
        }
        return list
    }
    
// MARK: Fields
    
    public func readINTV(_ length: Int) -> Int64 {
        switch length {
        case 1: return Int64(readByte())
        case 2: return Int64(readLEInt16())
        case 4: return Int64(readLEInt32())
        case 8: return Int64(readLEInt64())
        default: fatalError("Tried to read an INTV subrecord with an unsupported size (\(length))")
        }
    }
    
    public func readDATV(_ length: Int, type: Character) -> (b: Bool?, i: Int32?, f: Float?, s: String?) {
        switch type {
        case "b": return (b: readLEInt32() != 0, i: nil, f: nil, s: nil)
        case "i": return (b: nil, i: readLEInt32(), f: nil, s: nil)
        case "f": return (b: nil, i: nil, f: readLESingle(), s: nil)
        case "s": return (b: nil, i: nil, f: nil, s: readASCIIString(length))
        default: fatalError("\(type)")
        }
    }
    
    public func readSTRV(_ length: Int, format: ASCIIFormat = .possibleNullTerminated) -> String {
        return readASCIIString(length, format: format)
    }
    
    public func readBYTV(_ length: Int) -> Data {
        return readBytes(length)
    }
    
    public func readT<T: ExpressibleByNilLiteral>(_ length: Int) -> T {
        let size = MemoryLayout<T>.size
        var data = baseStream.readData(ofLength: length)
        assert(!(data.count > size))
        if data.count < size {
            data.append(Data.init(count: size - data.count))
        }
        return data.withUnsafeBytes { (ptr: UnsafePointer<T>) -> T in
            return ptr.pointee
        }
    }
    
    public func readT<T>(_ length: Int) -> T {
        return baseStream.readData(ofLength: length).withUnsafeBytes { (ptr: UnsafePointer<T>) -> T in
            return ptr.pointee
        }
//        return baseStream.readData(ofLength: length).withUnsafeBytes { (ptr: UnsafePointer<UInt8>) in
//            let rawPtr = UnsafeRawPointer(ptr)
//            return rawPtr.load(as: T.self)
//        }
//        return baseStream.readData(ofLength: length).withUnsafeBytes { (ptr: UnsafePointer<UInt8>) in
//            let rawPtr = UnsafeRawPointer(ptr)
//            return rawPtr.bindMemory(to: T.self, capacity: 1).pointee
//        }
    }
    
    public func readTArray<T>(_ length: Int, count: Int) -> [T] {
        return baseStream.readData(ofLength: length).withUnsafeBytes { (ptr: UnsafePointer<UInt8>) in
            let rawPtr = UnsafeRawPointer(ptr)
            let typedPtr = rawPtr.bindMemory(to: T.self, capacity: count)
            let buffer = UnsafeBufferPointer(start: typedPtr, count: count)
            return Array(buffer)
        }
    }
    
    // MARK: A
    // https://stackoverflow.com/questions/41574498/how-to-use-unsafemutablerawpointer-to-fill-an-array
    // https://github.com/apple/swift-evolution/blob/master/proposals/0138-unsaferawbufferpointer.md

    public func readLEBool32() -> Bool {
        return readLEUInt32() != 0
    }
    
    public func readLEUInt16() -> UInt16 {
        return baseStream.readData(ofLength: 2).withUnsafeBytes { (ptr: UnsafePointer<UInt16>) -> UInt16 in
            return ptr.pointee
        }
    }

    public func readLEUInt32() -> UInt32 {
        return baseStream.readData(ofLength: 4).withUnsafeBytes { (ptr: UnsafePointer<UInt32>) -> UInt32 in
            return ptr.pointee
        }
    }
    
    public func readLEUInt64() -> UInt64 {
        return baseStream.readData(ofLength: 8).withUnsafeBytes { (ptr: UnsafePointer<UInt64>) -> UInt64 in
            return ptr.pointee
        }
    }
    
    public func readLEInt16() -> Int16 {
        return baseStream.readData(ofLength: 2).withUnsafeBytes { (ptr: UnsafePointer<Int16>) -> Int16 in
            return ptr.pointee
        }
    }
    
    public func readLEInt32() -> Int32 {
        return baseStream.readData(ofLength: 4).withUnsafeBytes { (ptr: UnsafePointer<Int32>) -> Int32 in
            return ptr.pointee
        }
    }
    
    public func readLEInt64() -> Int64 {
        return baseStream.readData(ofLength: 8).withUnsafeBytes { (ptr: UnsafePointer<Int64>) -> Int64 in
            return ptr.pointee
        }
    }

    public func readLESingle() -> Float {
        return baseStream.readData(ofLength: 4).withUnsafeBytes { (ptr: UnsafePointer<Float>) -> Float in
            return ptr.pointee
        }
    }

    public func readLEDouble() -> Double {
        return baseStream.readData(ofLength: 8).withUnsafeBytes { (ptr: UnsafePointer<Double>) -> Double in
            return ptr.pointee
        }
    }

    // MARK: A

    public func readLELength32PrefixedBytes()-> Data {
        let length = readLEUInt32()
        return readBytes(Int(length))
    }

    public func readLELength32PrefixedASCIIString() -> String {
        let length = readLEUInt32()
        let data = readBytes(Int(length))
        return String(data: data, encoding: .ascii)!
    }

    public func readLEFloat2() -> float2 {
        return float2(readLESingle(), readLESingle())
    }

    public func readLEFloat3() -> float3 {
        return float3(readLESingle(), readLESingle(), readLESingle())
    }
    
    public func readLEColumnMajorMatrix3x3() -> simd_float4x4 {
        let m11 = readLESingle(), m21 = readLESingle(), m31 = readLESingle()
        let m12 = readLESingle(), m22 = readLESingle(), m32 = readLESingle()
        let m13 = readLESingle(), m23 = readLESingle(), m33 = readLESingle()
        return simd_float4x4(
            float4(m11, m12, m13, 0),
            float4(m21, m22, m23, 0),
            float4(m31, m32, m33, 0),
            float4(0, 0, 0, 1))
    }

    public func readLERowMajorMatrix3x3() -> simd_float4x4 {
        return simd_float4x4(
            float4(readLESingle(), readLESingle(), readLESingle(), 0),
            float4(readLESingle(), readLESingle(), readLESingle(), 0),
            float4(readLESingle(), readLESingle(), readLESingle(), 0),
            float4(0, 0, 0, 1))
    }

    public func readLEColumnMajorMatrix4x4() -> simd_float4x4 {
        let m11 = readLESingle(), m21 = readLESingle(), m31 = readLESingle(), m41 = readLESingle()
        let m12 = readLESingle(), m22 = readLESingle(), m32 = readLESingle(), m42 = readLESingle()
        let m13 = readLESingle(), m23 = readLESingle(), m33 = readLESingle(), m43 = readLESingle()
        let m14 = readLESingle(), m24 = readLESingle(), m34 = readLESingle(), m44 = readLESingle()
        return simd_float4x4(
            float4(m11, m12, m13, m14),
            float4(m21, m22, m23, m24),
            float4(m31, m32, m33, m34),
            float4(m41, m42, m43, m44))
    }

    public func readLERowMajorMatrix4x4() -> simd_float4x4 {
        return simd_float4x4(
            float4(readLESingle(), readLESingle(), readLESingle(), readLESingle()),
            float4(readLESingle(), readLESingle(), readLESingle(), readLESingle()),
            float4(readLESingle(), readLESingle(), readLESingle(), readLESingle()),
            float4(readLESingle(), readLESingle(), readLESingle(), readLESingle()))
    }

    public func readLEQuaternionWFirst() -> simd_quatf {
        let w = readLESingle(), x = readLESingle(), y = readLESingle(), z = readLESingle()
        return simd_quatf(ix: x, iy: y, iz: z, r: w)
    }

    public func readLEQuaternionWLast() -> simd_quatf {
        let x = readLESingle(), y = readLESingle(), z = readLESingle(), w = readLESingle()
        return simd_quatf(ix: x, iy: y, iz: z, r: w)
    }
}
