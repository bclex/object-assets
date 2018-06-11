//
//  ColorExtensions.swift
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
public typealias Vector2 = CGVector
public typealias Vector3 = SCNVector3
public typealias Matrix4x4 = SCNMatrix4
public typealias Quaternion = SCNQuaternion

public extension Float {
    public static let rad2Deg = 0.01745329

    public static func clamp01(_ value: Float) -> Float {
        return value < 0 ? 0 : value > 1 ? 1 : value
    }

    public static func lerp(_ a: Float, _ b: Float, t: Float) -> Float {
        return a + ((b - a) * clamp01(t))
    }

    public func roundToInt() -> Int {
        return Int(self.round())
    }
}

public class Utils {
    public static func containsBitFlags(bits: Int, args: Int...) -> Bool {
        var flagBits: UInt64 = 0
        for arg in args {
            flagBits |= UInt64(arg)
        }
        return (UInt64(bits) & flagBits) == flagBits
    }

    public static func getBits(bitOffset: Int, bitCount: Int, bytes: Data) -> UInt64 {
        assert(bitCount <= 64 && (bitOffset + bitCount) <= (8 * bytes.count))
        var bits: UInt64 = 0
        let remainingBitCount = bitCount
        let byteIndex = bitOffset / 8
        let bitIndex = bitOffset - (8 * byteIndex)
        while remainingBitCount > 0 {
            // Read bits from the byte array.
            let numBitsLeftInByte = 8 - bitIndex
            let numBitsReadNow = min(remainingBitCount, numBitsLeftInByte)
            let unmaskedBits = bytes[byteIndex] >> (8 - (bitIndex + numBitsReadNow))
            let bitMask = 0xFF >> (8 - numBitsReadNow)
            let bitsReadNow = unmaskedBits & bitMask

            // Store the bits we read.
            bits <<= numBitsReadNow;
            bits |= bitsReadNow;

            // Prepare for the next iteration.
            bitIndex += numBitsReadNow;

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
        return min1 + (xPct * range1)
    }
}
