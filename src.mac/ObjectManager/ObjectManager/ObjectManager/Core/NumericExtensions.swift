//
//  NumericExtensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import CoreGraphics

public extension Float {
    public static let rad2Deg = 0.01745329

    public func roundedAsInt() -> Int {
        return Int(self.rounded())
    }
}

public extension FloatingPoint {
    public func clamped() -> Self {
        return self < 0 ? 0 : self > 1 ? 1 : self
    }
    
    public static func lerp(_ a: Self, _ b: Self, t: Self) -> Self {
        return a + ((b - a) * t.clamped())
    }
}
