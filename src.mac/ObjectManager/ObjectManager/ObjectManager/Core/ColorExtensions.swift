//
//  ColorExtensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
    
import CoreGraphics

public struct Color32 {
    public var red: UInt8
    public var green: UInt8
    public var blue: UInt8
    public var alpha: UInt8
    
    init(red: UInt8, green: UInt8, blue: UInt8, alpha: UInt8) {
        self.red = red
        self.green = green
        self.blue = blue
        self.alpha = alpha
    }
    init(b565: UInt16) {
        let r5 = (b565 >> 11) & 31
        let g6 = (b565 >> 5) & 63
        let b5 = b565 & 31
        self.init(color: Color(red: CGFloat(r5 / 31), green: CGFloat(g6 / 63), blue: CGFloat(b5 / 31), alpha: 1))
    }
    init(color: Color) {
        guard let comp = color.components else {
            fatalError("color must have componets")
        }
        let r = UInt8(comp[0].clamped() * 0xFF)
        let g = UInt8(comp[1].clamped() * 0xFF)
        let b = UInt8(comp[2].clamped() * 0xFF)
        let a = UInt8(comp[3].clamped() * 0xFF)
        self.init(red: r, green: g, blue: b, alpha: a)
    }
    public var toColor: Color {
        let r = CGFloat(self.red / 0xFF)
        let g = CGFloat(self.green / 0xFF)
        let b = CGFloat(self.blue / 0xFF)
        let a = CGFloat(self.alpha / 0xFF)
        return Color(red: r, green: g, blue: b, alpha: a)
    }
}

public extension Color {
    public static func color(b565: UInt16) -> Color {
        let r5 = (b565 >> 11) & 31
        let g6 = (b565 >> 5) & 63
        let b5 = b565 & 31
        return Color(red: CGFloat(r5 / 31), green: CGFloat(g6 / 63), blue: CGFloat(b5 / 31), alpha: 1)
    }
    
    public static func lerp(_ a: Color, _ b: Color, fraction: Float) -> Color? {
        let f = CGFloat(fraction.clamped())
        //let f = min(max(0, fraction), 1)
        guard let c1 = a.components, let c2 = b.components else { return nil }
        let r = CGFloat(c1[0] + (c2[0] - c1[0]) * f)
        let g = CGFloat(c1[1] + (c2[1] - c1[1]) * f)
        let b = CGFloat(c1[2] + (c2[2] - c1[2]) * f)
        let a = CGFloat(c1[3] + (c2[3] - c1[3]) * f)
        return CGColor(red: r, green: g, blue: b, alpha: a)
        //let r = a.red + (b.red - a.red) * f
        //let g = a.green + (b.green - a.green) * f
        //let b = a.blue + (b.blue - a.blue) * f
        //let a = a.alpha + (b.alpha - a.alpha) * f
        //return CGColor(red: r, green: g, blue: b, alpha: a)
    }
}

