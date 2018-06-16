//
//  Texture2DInfo.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import AppKit

public class Texture2DInfo {
    public let width: Int
    public let height: Int
    public let format: TextureFormat
    public let hasMipmaps: Bool
    public let bytesPerPixel: Int
    public let rawData: Data

    init(width: Int, height: Int, format: TextureFormat, hasMipmaps: Bool, bytesPerPixel: Int, rawData: Data) {
        self.width = width
        self.height = height
        self.format = format
        self.hasMipmaps = hasMipmaps
        self.bytesPerPixel = bytesPerPixel
        self.rawData = rawData
    }

    public func toTexture2D() -> Texture2D {
        let ciImage = CIImage(
            bitmapData: rawData,
            bytesPerRow: width * bytesPerPixel,
            size: CGSize(width: width, height: height), 
            format: format, 
            colorSpace: nil)
        let rep = NSCIImageRep(ciImage: ciImage)
        let nsImage = NSImage(size: rep.size)
        nsImage.addRepresentation(rep)
        return nsImage
        /*
        let size = CGSize(width: width, height: height)
        let cgImage = CGImage(
            width: width, height: height,
            bitsPerComponent: 8, bitsPerPixel: bpp,
            bytesPerRow: width * bpp,
            space: CGColorSpace(name: CGColorSpace.sRGB)!,
            bitmapInfo: CGBitmapInfo(rawValue: rawData),
            provider: CGDataProvider.init(data: rawData),
            decode: nil,
            shouldInterpolate: false,
            intent: nil)
        return NSImage(cgImage: cgImage, size: size)
        */
    }
}
