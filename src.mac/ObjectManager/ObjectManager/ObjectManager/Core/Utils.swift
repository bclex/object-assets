//
//  Utils.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
    
import CoreGraphics
import SceneKit
import AppKit

public typealias Color = CGColor
public typealias Texture2D = NSImage
public typealias TextureFormat = CIFormat
public typealias Vector2 = CGVector
public typealias Vector3 = SCNVector3
public typealias Matrix4x4 = SCNMatrix4
public typealias Quaternion = SCNQuaternion

public class Utils {
    public static func bytes<T>(of value: T) -> [UInt8] {
        var value = value
        let size = MemoryLayout<T>.size
        return withUnsafePointer(to: &value, {
            $0.withMemoryRebound(to: UInt8.self, capacity: size, { Array(UnsafeBufferPointer(start: $0, count: size)) })
        })
    }
    
    public static func hexString<T>(of value: T) -> String {
        return hexString(bytes: bytes(of: value))
    }
    public static func hexString<Seq: Sequence>(bytes: Seq, limit: Int? = nil, separator: String = " ") -> String
        where Seq.Iterator.Element == UInt8 {
            let spacesInterval = 8
            var result = ""
            for (index, byte) in bytes.enumerated() {
                if let limit = limit, index >= limit {
                    result.append("...")
                    break
                }
                if index > 0 && index % spacesInterval == 0 {
                    result.append(separator)
                }
                result.append(String(format: "%02x", byte))
            }
            return result
    }
    
    public static func containsBitFlags(_ bits: UInt, _ args: UInt...) -> Bool {
        var flagBits: UInt = 0
        for arg in args {
            flagBits |= arg
        }
        return (bits & flagBits) == flagBits
    }
    
    public static func swap(_ a: inout CGFloat, _ b: inout CGFloat) {
        let temp = a
        a = b
        b = temp
    }

    public static func getBits(_ bitOffset: Int, _ bitCount: Int, _ bytes: Data) -> UInt64 {
        assert(bitCount <= 64 && (bitOffset + bitCount) <= (8 * bytes.count))
        var bits: UInt64 = 0
        var remainingBitCount = bitCount
        var byteIndex = bitOffset / 8
        var bitIndex = bitOffset - (8 * byteIndex)
        while remainingBitCount > 0 {
            // Read bits from the byte array.
            let numBitsLeftInByte = 8 - bitIndex
            let numBitsReadNow = min(remainingBitCount, numBitsLeftInByte)
            let unmaskedBits = UInt64(bytes[byteIndex] >> (8 - (bitIndex + numBitsReadNow)))
            let bitMask = UInt64(0xFF >> (8 - numBitsReadNow))
            let bitsReadNow = unmaskedBits & bitMask

            // Store the bits we read.
            bits <<= numBitsReadNow
            bits |= bitsReadNow

            // Prepare for the next iteration.
            bitIndex += numBitsReadNow

            if bitIndex == 8 {
                byteIndex += 1
                bitIndex = 0
            }
            remainingBitCount -= numBitsReadNow;
        }
        return bits
    }

    public static func changeRange(x: Float, min0: Float, max0: Float, min1: Float, max1: Float) -> Float {
        assert(min0 <= max0 && min1 <= max1 && x >= min0 && x <= max0)
        let range0 = max0 - min0
        let range1 = max1 - min1
        let xpct = (x - min0) / range0
        return min1 + (xpct * range1)
    }
}
