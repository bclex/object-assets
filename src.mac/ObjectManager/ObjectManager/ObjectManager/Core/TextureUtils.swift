//
//  Texture2DInfo.swift
//  ObjectManager
//
//  Created by Sky Morey on 5/28/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation

public class TextureUtils {
    public static func calculateMipMapCount(width: Int, height: Int) -> Int {
        assert(width > 0 && height > 0)
        let longerLength = Swift.max(width, height)
        var mipMapCount = 0
        var currentLongerLength = longerLength
        while currentLongerLength > 0 {
            mipMapCount += 1
            currentLongerLength /= 2
        }
        return mipMapCount
    }

    public static func calculateMipMappedTextureDataSize(width: Int, height: Int, bytesPerPixel: Int) -> Int {
        assert(width > 0 && height > 0 && bytesPerPixel > 0)
        var dataSize = 0
        var currentWidth = width
        var currentHeight = height;
        while true {
            dataSize += currentWidth * currentHeight * bytesPerPixel
            if currentWidth == 1 && currentHeight == 1 {
                break
            }
            currentWidth = currentWidth > 1 ? (currentWidth / 2) : currentWidth
            currentHeight = currentHeight > 1 ? (currentHeight / 2) : currentHeight
        }
        return dataSize
    }

    // TODO: Improve algorithm for images with odd dimensions.
    public static func downscale4Component32BitPixelsX2(srcData: Data, srcStartIndex: Int, srcRowCount: Int, srcColumnCount: Int, dstData: inout Data, dstStartIndex: Int) {
        let bytesPerPixel = 4
        let componentCount = 4
        assert(srcStartIndex >= 0 && srcRowCount >= 0 && srcColumnCount >= 0 &&
            (srcStartIndex + (bytesPerPixel * srcRowCount * srcColumnCount)) <= srcData.count)
        let dstRowCount = srcRowCount / 2
        let dstColumnCount = srcColumnCount / 2
        assert(dstStartIndex >= 0 && (dstStartIndex + (bytesPerPixel * dstRowCount * dstColumnCount)) <= dstData.count)
        for dstRowIndex in 0..<dstRowCount {
            for dstColumnIndex in 0..<dstColumnCount {
                let srcRowIndex0 = 2 * dstRowIndex
                let srcColumnIndex0 = 2 * dstColumnIndex
                let srcPixel0Index = (srcColumnCount * srcRowIndex0) + srcColumnIndex0

                var srcPixelStartIndices = [Int](); srcPixelStartIndices.reserveCapacity(4)
                srcPixelStartIndices.append(srcStartIndex + (bytesPerPixel * srcPixel0Index)) // top-left
                srcPixelStartIndices.append(srcPixelStartIndices[0] + bytesPerPixel) // top-right
                srcPixelStartIndices.append(srcPixelStartIndices[0] + (bytesPerPixel * srcColumnCount)) // bottom-left
                srcPixelStartIndices.append(srcPixelStartIndices[2] + bytesPerPixel) // bottom-right

                let dstPixelIndex = (dstColumnCount * dstRowIndex) + dstColumnIndex
                let dstPixelStartIndex = dstStartIndex + (bytesPerPixel * dstPixelIndex)
                for componentIndex in 0..<componentCount {
                    var averageComponent: Float = 0
                    for srcPixelIndex in 0..<srcPixelStartIndices.capacity {
                        averageComponent += Float(srcData[srcPixelStartIndices[srcPixelIndex] + componentIndex])
                    }
                    averageComponent /= Float(srcPixelStartIndices.count)
                    dstData[dstPixelStartIndex + componentIndex] = UInt8(averageComponent.roundedAsInt())
                }
            }
        }
    }
}
