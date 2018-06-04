//
//  Texture2DInfo.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class Texture2DInfo {
    public let width: Int
    public let height: Int
    public let format: CIFormat
    public let bpp: Int
    public let rawData: Data

    init(width: Int, height: Int, format: CIFormat, bpp: Int, rawData: Data) {
        self.width = width
        self.height = height
        self.format = format
        self.bpp = bpp
        self.rawData = rawData
    }

    public func toTexture2D() -> UIImage {
        let image = CIImage(
            bitmapData: rawData,
            bytesPerRow: width * bpp,
            size: CGSize(width: width, height: height), 
            format: format, 
            colorSpace: nil)
        return UIImage(ciImage: image)
    }
}
