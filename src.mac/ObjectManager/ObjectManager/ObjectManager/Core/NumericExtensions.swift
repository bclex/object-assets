//
//  NumericExtensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import CoreGraphics

public extension UInt8 {
    init(checkMax value: Int32) {
        self.init(value != -1 ? value : 255)
    }
}

public extension Float {
//    public static let rad2Deg = 0.01745329
    public func flooredAsInt32() -> Int32 {
        return Int32(self.rounded(.down))
    }
    
    public func roundedAsInt() -> Int {
        return Int(self.rounded())
    }
    
    public static func approximately(_ a: Float, _ b: Float) -> Bool {
        let factor: Float = 1e-06
        return abs(b - a) < max(factor * max(abs(a), abs(b)), .ulpOfOne * 8)
    }
}

public extension FloatingPoint {
    public func clamped() -> Self {
        return self < 0 ? 0 : self > 1 ? 1 : self
    }
    
    public static func lerp(_ a: Self, _ b: Self, t: Self) -> Self {
        return a + ((b - a) * t.clamped())
    }
    
//    public static func approximately(_ a: Self, _ b: Self) -> Bool {
//        let factor: Self = 1e-06
//        return abs(b - a) < max(factor * max(abs(a), abs(b)), .ulpOfOne * 8)
//    }
}
