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
//    public static func containsBitFlags<T>(_ bits: T, _ args: Int...) -> Bool where T: OptionSet {
//        var flagBits: UInt64 = 0
//        for arg in args {
//            flagBits |= UInt64(arg)
//        }
//        return (UInt64(Self.RawValue(bits.rawValue)) & flagBits) == flagBits
//    }

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
        return min1 + (xpct * range1)
    }
}
