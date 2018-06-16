//
//  Color32.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
    
public struct Color32 {
    public var red: UInt8
    public var green: UInt8
    public var blue: UInt8
    public var alpha: UInt8

    init(b565: UInt16) {
        let r5 = (b565 >> 11) & 31
        let g6 = (b565 >> 5) & 63
        let b5 = b565 & 31
        self.init(red: UInt8(r5 / 31), green: UInt8(g6 / 63), blue: UInt8(b5 / 31), alpha: 1)
    }
    init(red: UInt8, green: UInt8, blue: UInt8, alpha: UInt8) {
        self.red = red
        self.green = green
        self.blue = blue
        self.alpha = alpha
    }
}
