//
//  BinaryReader.swift
//  CocoApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public enum ASCIIFormat {
    case raw, possibleNullTerminated, zeroPadded
}

public protocol BaseStream {
    func close()
    func readData(ofLength: Int) -> Data
    var position: UInt64 {get set}
    var length: UInt64 {get}
}

public class FileBaseStream: FileHandle, BaseStream {
    public var position: UInt64 {
        get { return self.offsetInFile }
        set { self.seek(toFileOffset: newValue) }
    }
    
    public var length: UInt64 { return 0 }
    
    public func close() { super.closeFile() }
    //public func readData(ofLength: Int) -> Data { return super.readData(ofLength: ofLength) }
    //public required init?(coder: NSCoder) { }
    //public convenience init?(forReadingAtPath path: String) { self.init(forReadingAtPath: path) }
}

public class DataBaseStream: BaseStream {
    let data: Data
    public var position: UInt64
    
    init(data: Data) {
        self.data = data
        position = 0
    }
    
    public func close() {
    }
    
    public var length: UInt64 { return UInt64(data.count) }
    
    public func readData(ofLength: Int) -> Data {
        let startIndex = Int(position)
        //let endIndex = startIndex + ofLength
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
        return r.readBytes(Int(remainingByteCount))
    }

    public func readRestOfBytes(_ inout data: Data, offsetBy: Int) -> Data {
        let remainingByteCount = baseStream.length - baseStream.position
        assert(startIndex >= 0 && remainingByteCount <= Int.max && startIndex + remainingByteCount <= buffer.Length)
        r.read(&data, offsetBy: offsetBy, count: Int(remainingByteCount))
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
        return String(data: data, encoding: .utf8)!
    }
    
    // MARK: A

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
        return r.readBytes(Int(length))
    }

    public func readLELength32PrefixedASCIIString() -> String {
        let length = readLEUInt32()
        let data = r.readBytes(Int(length))
        return String(data: data, encoding: .utf8)!
    }

    public func readLEVector2() -> Vector2 {
        return new Vector2(readLESingle(), readLESingle())
    }

    public func readLEVector3() -> Vector3 {
        return new Vector3(readLESingle(), readLESingle(), readLESingle())
    }

    public func readLEColumnMajorMatrix3x3() -> Matrix4x4 {
        var matrix = Matrix4x4()
        for columnIndex in 0..< 4 {
            for rowIndex in 0..< 4 {
                // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                if rowIndex <= 2 && columnIndex <= 2 { matrix[rowIndex, columnIndex] = readLESingle() }
                else { matrix[rowIndex, columnIndex] = rowIndex == columnIndex ? 1 : 0 }
            }
        }
        return matrix
    }

    public func readLERowMajorMatrix3x3() -> Matrix4x4 {
        var matrix = Matrix4x4()
        for rowIndex in 0..< 4 {
            for columnIndex in 0..< 4 {
                // If we're in the 3x3 part of the matrix, read values. Otherwise, use the identity matrix.
                if rowIndex <= 2 && columnIndex <= 2 { matrix[rowIndex, columnIndex] = readLESingle() }
                else { matrix[rowIndex, columnIndex] = rowIndex == columnIndex ? 1 : 0 }
            }
        }
        return matrix
    }

    public func readLEColumnMajorMatrix4x4() -> Matrix4x4 {
        var matrix = Matrix4x4()
        for columnIndex in 0..<4 {
            for rowIndex in 0..<4 {
                matrix[rowIndex, columnIndex] = readLESingle()
            }
        }
        return matrix
    }

    public func readLERowMajorMatrix4x4() -> Matrix4x4 {
        var matrix = Matrix4x4()
        for rowIndex in 0..4 {
            for columnIndex in 0..< 4 {
                matrix[rowIndex, columnIndex] = readLESingle()
            }
        }
        return matrix
    }

    public func readLEQuaternionWFirst() -> Quaternion {
        let w = readLESingle()
        let x = readLESingle()
        let y = readLESingle()
        let z = readLESingle()
        return Quaternion(x, y, z, w)
    }

    public func readLEQuaternionWLast() -> Quaternion {
        let x = readLESingle()
        let y = readLESingle()
        let z = readLESingle()
        let w = readLESingle()
        return Quaternion(x, y, z, w)
    }
}
