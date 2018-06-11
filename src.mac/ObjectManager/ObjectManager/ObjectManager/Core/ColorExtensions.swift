//
//  ColorExtensions.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
    
import CoreGraphics

public extension CGColor {
    public convenience init(b565: UInt16) {
        let r5 = (b565 >> 11) & 31
        let g6 = (b565 >> 5) & 63
        let b5 = b565 & 31
        super.init(Float(r5) / 31, Float(g6) / 63, Float(b5) / 31, 1)
    }

    public static func lerp(_ a: CGColor, _ b: UIColor, fraction: Float) -> CGColor {
        let f = Float.clamp01(fraction)
        return CGColor(red: a.red + (b.red - a.red) * f, green: a.green + (b.green - a.green) * f, blue: a.blue + (b.blue - a.blue) * f, alpha: a.alpha + (b.alpha - a.alpha) * f)
        // let f = min(max(0, fraction), 1)
        // guard let c1 = a.cgColor.components, let c2 = b.cgColor.components else { return nil }
        // let r = CGFloat(c1[0] + (c2[0] - c1[0]) * f)
        // let g = CGFloat(c1[1] + (c2[1] - c1[1]) * f)
        // let b = CGFloat(c1[2] + (c2[2] - c1[2]) * f)
        // let a = CGFloat(c1[3] + (c2[3] - c1[3]) * f)
        // return CGColor(red: r, green: g, blue: b, alpha: a)
    }
}

public extension Color32 {
    public convenience init(b565: UInt16) {
        let r5 = (b565 >> 11) & 31
        let g6 = (b565 >> 5) & 63
        let b5 = b565 & 31
        super.init(Float(r5) / 31, Float(g6) / 63, Float(b5) / 31, 1)
    }    
}
