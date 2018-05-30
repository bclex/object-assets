//
//  File.swift
//  CocoApp
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public enum ASCIIFormat {
    case raw, possibleNullTerminated, zeroPadded
}

public class BinaryReader {
    
    public var baseStream: FileHandle!
    
    init(stream: FileHandle) {
        baseStream = stream
    }
    
    //init(stream: Data) {
    //    baseStream = stream
    //}
    
    deinit {
        close()
    }
    
    func close() {
        baseStream?.closeFile()
        baseStream = nil
    }

    func readByte() -> UInt8 {
        return baseStream.readData(ofLength: 1).first!
    }
    
    public func readSByte() -> Int8 {
        return Int8(bitPattern: baseStream.readData(ofLength: 1).first!)
    }
    
    public func read(buffer: Data) {
        // NOT IMPLEMENTED
    }
    
    public func readBytes(_ count: Int) -> Data {
        return baseStream.readData(ofLength: count)
    }
    
    // readRestOfBytes
    // readRestOfBytes
    
    public func readASCIIString(length: Int, format: ASCIIFormat = .raw) -> String {
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
}
