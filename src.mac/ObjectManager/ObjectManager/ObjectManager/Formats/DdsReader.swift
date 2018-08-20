//
//  DdsReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//

import Foundation
import CoreImage

public struct DDSFlags: OptionSet { //: DDSD
    public let rawValue: UInt32
    public static let caps = DDSFlags(rawValue: 0x1)             // Required in every .dds file.
    public static let height = DDSFlags(rawValue: 0x2)           // Required in every .dds file.
    public static let width = DDSFlags(rawValue: 0x4)            // Required in every .dds file.
    public static let pitch = DDSFlags(rawValue: 0x8)            // Required when pitch is provided for an uncompressed texture.
    public static let pixelFormat = DDSFlags(rawValue: 0x1000)   // Required in every .dds file.
    public static let mipmapCount = DDSFlags(rawValue: 0x20000)  // Required in a mipmapped texture.
    public static let linearSize = DDSFlags(rawValue: 0x80000)   // Required when pitch is provided for a compressed texture.
    public static let depth = DDSFlags(rawValue: 0x800000)       // Required in a depth texture.
    public static let HEADER_FLAGS_TEXTURE: DDSFlags = [.caps, .height, .width, .pixelFormat]
    public static let HEADER_FLAGS_MIPMAP: DDSFlags = .mipmapCount
    public static let HEADER_FLAGS_VOLUME: DDSFlags = .depth
    public static let HEADER_FLAGS_PITCH: DDSFlags = .pitch
    public static let HEADER_FLAGS_LINEARSIZE: DDSFlags = .linearSize

    public init(rawValue: UInt32) {
        self.rawValue = rawValue
    }
}

public struct DDSCaps: OptionSet { //: DDSCAPS
    public let rawValue: UInt32
    public static let complex = DDSCaps(rawValue: 0x8)          // Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
    public static let mipmap = DDSCaps(rawValue: 0x400000)      // Optional; should be used for a mipmap.
    public static let texture = DDSCaps(rawValue: 0x1000)       // Required
    public static let SURFACE_FLAGS_MIPMAP: DDSCaps = [.complex, .mipmap]
    public static let SURFACE_FLAGS_TEXTURE: DDSCaps = .texture
    public static let SURFACE_FLAGS_CUBEMAP: DDSCaps = .complex
    
    public init(rawValue: UInt32) {
        self.rawValue = rawValue
    }
}

public struct DDSCaps2: OptionSet { //: DDSCAPS2
    public let rawValue: UInt32
    public static let none = DDSCaps2(rawValue: 0)
    public static let cubemap = DDSCaps2(rawValue: 0x200)            // Required for a cube map.
    public static let cubemapPositiveX = DDSCaps2(rawValue: 0x400)   // Required when these surfaces are stored in a cube map.
    public static let cubemapNegativeX = DDSCaps2(rawValue: 0x800)   // Required when these surfaces are stored in a cube map.
    public static let cubemapPositiveY = DDSCaps2(rawValue: 0x1000)  // Required when these surfaces are stored in a cube map.
    public static let cubemapNegativeY = DDSCaps2(rawValue: 0x2000)  // Required when these surfaces are stored in a cube map.
    public static let cubemapPositiveZ = DDSCaps2(rawValue: 0x4000)  // Required when these surfaces are stored in a cube map.
    public static let cubemapNegativeZ = DDSCaps2(rawValue: 0x8000)  // Required when these surfaces are stored in a cube map.
    public static let volume = DDSCaps2(rawValue: 0x200000)          // Required for a volume texture.
    public static let CUBEMAP_POSITIVEX: DDSCaps2 = [.cubemap, .cubemapPositiveX]
    public static let CUBEMAP_NEGATIVEX: DDSCaps2 = [.cubemap, .cubemapNegativeX]
    public static let CUBEMAP_POSITIVEY: DDSCaps2 = [.cubemap, .cubemapPositiveY]
    public static let CUBEMAP_NEGATIVEY: DDSCaps2 = [.cubemap, .cubemapNegativeY]
    public static let CUBEMAP_POSITIVEZ: DDSCaps2 = [.cubemap, .cubemapPositiveZ]
    public static let CUBEMAP_NEGATIVEZ: DDSCaps2 = [.cubemap, .cubemapNegativeZ]
    public static let CUBEMAP_ALLFACES: DDSCaps2 = [.cubemapPositiveX, .cubemapNegativeX, .cubemapPositiveY, .cubemapNegativeY, .cubemapPositiveZ, .cubemapNegativeZ]
    public static let FLAGS_VOLUME = volume
    
    public init(rawValue: UInt32) {
        self.rawValue = rawValue
    }
}

public struct DDSHeader { //: DDS_HEADER
    public var dwSize: UInt32 { return 124 }
    public let dwFlags: DDSFlags
    public let dwHeight: UInt32
    public let dwWidth: UInt32
    public let dwPitchOrLinearSize: UInt32
    public let dwDepth: UInt32 // only if DDS_HEADER_FLAGS_VOLUME is set in flags
    public let dwMipMapCount: UInt32
    public var dwReserved1: [UInt32]
    public let ddspf: DDSPixelFormat
    public let dwCaps: DDSCaps // dwSurfaceFlags
    public let dwCaps2: DDSCaps2 // dwCubemapFlags
    public let dwCaps3: UInt32
    public let dwCaps4: UInt32
    public let dwReserved2: UInt32

    init(dwFlags: DDSFlags,
         dwHeight: UInt32, dwWidth: UInt32,
         dwPitchOrLinearSize: UInt32,
         dwMipMapCount: UInt32,
         ddspf: DDSPixelFormat,
         dwCaps: DDSCaps,
         dwCaps2: DDSCaps2) {
        self.dwFlags = dwFlags
        self.dwHeight = dwHeight; self.dwWidth = dwWidth
        self.dwPitchOrLinearSize = dwPitchOrLinearSize
        dwDepth = 0
        self.dwMipMapCount = dwMipMapCount
        dwReserved1 = [UInt32](repeating: 0, count: 11)
        self.ddspf = ddspf
        self.dwCaps = dwCaps
        self.dwCaps2 = dwCaps2
        dwCaps3 = 0
        dwCaps4 = 0
        dwReserved2 = 0
    }
    init(_ r: BinaryReader) {
        let size = r.readLEUInt32()
        guard size == 124 else {
            fatalError("Invalid DDS file header size: \(size).")
        }
        dwFlags = DDSFlags(rawValue: r.readLEUInt32())
        guard dwFlags.contains([.height, .width]) else {
            fatalError("Invalid DDS file flags")
        }
        dwHeight = r.readLEUInt32()
        dwWidth = r.readLEUInt32()
        dwPitchOrLinearSize = r.readLEUInt32()
        dwDepth = r.readLEUInt32()
        dwMipMapCount = r.readLEUInt32()
        dwReserved1 = r.readTArray(11 << 2, count: 11)
        ddspf = DDSPixelFormat(r)
        dwCaps = DDSCaps(rawValue: r.readLEUInt32())
        guard dwCaps.contains(.texture) else {
            fatalError("Invalid DDS file caps")
        }
        dwCaps2 = DDSCaps2(rawValue: r.readLEUInt32())
        dwCaps3 = r.readLEUInt32()
        dwCaps4 = r.readLEUInt32()
        dwReserved2 = r.readLEUInt32()
    }

    func write(_ w: BinaryWriter) {
        w.write(dwSize)
        w.write(dwFlags.rawValue)
        w.write(dwHeight)
        w.write(dwWidth)
        w.write(dwPitchOrLinearSize)
        w.write(dwDepth)
        w.write(dwMipMapCount)
        for i in 0..<11 {
            w.write(dwReserved1[i])
        }
        ddspf.write(w)
        w.write(dwCaps.rawValue)
        w.write(dwCaps2.rawValue)
        w.write(dwCaps3)
        w.write(dwCaps4)
        w.write(dwReserved2)
    }
}

public enum DDSDimension: UInt32 { //: D3D10_RESOURCE_DIMENSION
    case texture1D = 2 // Resource is a 1D texture. The dwWidth member of DDS_HEADER specifies the size of the texture. Typically, you set the dwHeight member of DDS_HEADER to 1; you also must set the DDSD_HEIGHT flag in the dwFlags member of DDS_HEADER.
    case texture2D = 3 // Resource is a 2D texture with an area specified by the dwWidth and dwHeight members of DDS_HEADER. You can also use this type to identify a cube-map texture. For more information about how to identify a cube-map texture, see miscFlag and arraySize members.
    case texture3D = 4 // Resource is a 3D texture with a volume specified by the dwWidth, dwHeight, and dwDepth members of DDS_HEADER. You also must set the DDSD_DEPTH flag in the dwFlags member of DDS_HEADER.
}

public struct DDSHeader_DXT10 { //: DDS_HEADER_DXT10
    public let dxgiFormat: Int32
    public let resourceDimension: DDSDimension
    public let miscFlag: UInt32 // see D3D11_RESOURCE_MISC_FLAG
    public let arraySize: UInt32
    public let miscFlags2: UInt32 // see DDS_MISC_FLAGS2

    init(dxgiFormat: Int32,
         resourceDimension: DDSDimension,
         miscFlag: UInt32 = 0,
         arraySize: UInt32 = 1,
         miscFlags2: UInt32 = 0) {
        self.dxgiFormat = dxgiFormat
        self.resourceDimension = resourceDimension
        self.miscFlag = miscFlag
        self.arraySize = arraySize
        self.miscFlags2 = miscFlags2
    }

    func write(_ w: BinaryWriter) {
        w.write(dxgiFormat)
        w.write(resourceDimension.rawValue)
        w.write(miscFlag)
        w.write(arraySize)
        w.write(miscFlags2)
    }
}

public struct DDSPixelFormats: OptionSet { //: DDS_PIXELFORMAT
    public let rawValue: UInt32
    public static let alphaPixels = DDSPixelFormats(rawValue: 0x1)      // Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
    public static let alpha = DDSPixelFormats(rawValue: 0x2)            // Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
    public static let fourCC = DDSPixelFormats(rawValue: 0x4)           // Texture contains compressed RGB data; dwFourCC contains valid data.
    public static let rgb = DDSPixelFormats(rawValue: 0x40)             // Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
    public static let yuv = DDSPixelFormats(rawValue: 0x200)            // Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
    public static let luminance = DDSPixelFormats(rawValue: 0x20000)    // Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.

    public init(rawValue: UInt32) {
        self.rawValue = rawValue
    }
}

public struct DDSPixelFormat { //: DDS_PIXELFORMAT
    public var dwSize: UInt32 { return 32 }
    public let dwFlags: DDSPixelFormats
    public let dwFourCC: String
    public let dwRGBBitCount: UInt32
    public let dwRBitMask: UInt32
    public let dwGBitMask: UInt32
    public let dwBBitMask: UInt32
    public let dwABitMask: UInt32

    init(dwFlags: DDSPixelFormats,
         dwFourCC: String = "",
         dwRGBBitCount: UInt32 = 0,
         dwRBitMask: UInt32 = 0,
         dwGBitMask: UInt32 = 0,
         dwBBitMask: UInt32 = 0,
         dwABitMask: UInt32 = 0) {
        self.dwFlags = dwFlags
        self.dwFourCC = dwFourCC
        self.dwRGBBitCount = dwRGBBitCount
        self.dwRBitMask = dwRBitMask
        self.dwGBitMask = dwGBitMask
        self.dwBBitMask = dwBBitMask
        self.dwABitMask = dwABitMask
    }
    init(_ r: BinaryReader) {
        let size = r.readLEUInt32()
        guard size == 32 else {
            fatalError("Invalid DDS file pixel format size: \(size).")
        }
        dwFlags = DDSPixelFormats(rawValue: r.readLEUInt32())
        dwFourCC = r.readASCIIString(4)
        dwRGBBitCount = r.readLEUInt32()
        dwRBitMask = r.readLEUInt32()
        dwGBitMask = r.readLEUInt32()
        dwBBitMask = r.readLEUInt32()
        dwABitMask = r.readLEUInt32()
    }

    func write(_ w: BinaryWriter ) {
        w.write(dwSize)
        w.write(dwFlags.rawValue)
        w.write(dwFourCC)
        w.write(dwRGBBitCount)
        w.write(dwRBitMask)
        w.write(dwGBitMask)
        w.write(dwBBitMask)
        w.write(dwABitMask)
    }
}

public enum DXGIFormat: UInt8 { //: DXGI_FORMAT
    case DXGI_FORMAT_UNKNOWN = 0
    case DXGI_FORMAT_R8_UNORM = 61
    case BC1_UNORM = 71
    case BC2_UNORM = 74
    case BC3_UNORM = 77
    case BC5_UNORM = 83
    case DXGI_FORMAT_B8G8R8A8_UNORM = 87
    case BC7_UNORM = 98
}

public class DdsReader {
//    public static func loadDDSTexture(filePath: String, flipVertically: Bool = false) -> Texture2DInfo {
//        let r = BinaryReader(FileBaseStream(path: filePath)!)
//        defer { r.close() }
//        return loadDDSTexture(r, flipVertically: flipVertically)
//    }

    public static func loadDDSTexture(_ r: BinaryReader, flipVertically: Bool = false) -> Texture2DInfo {
        // Check the magic string.
        let magicString = r.readASCIIString(4)
        guard magicString == "DDS " else {
            fatalError("Invalid DDS file magic string: '\(magicString)'.")
        }
        // Deserialize the DDS file header.
        let header = DDSHeader(r)
        // Figure out the texture format and load the texture data.
        let dds = extractDDSTextureFormatAndData(header, r)
        // Post-process the texture to generate missing mipmaps and possibly flip it vertically.
        var textureData = dds.textureData
        postProcessDDSTexture(
            width: Int(header.dwWidth), height: Int(header.dwHeight), bytesPerPixel: dds.bytesPerPixel,
            hasMipmaps: dds.hasMipmaps, ddsMipmapLevelCount: dds.ddsMipmapLevelCount,
            data: &textureData, flipVertically: flipVertically)
        return Texture2DInfo(
            width: Int(header.dwWidth), height: Int(header.dwHeight),
            format: dds.textureFormat, hasMipmaps: dds.hasMipmaps,
            bytesPerPixel: dds.bytesPerPixel, rawData: textureData)
    }

    static func decodeDXT1TexelBlock(_ r: BinaryReader, colorTable: [Color]) -> [Color32] {
        assert(colorTable.count == 4)
        // Read pixel color indices.
        var colorIndices = [Int](repeating: 0, count: 16)
        let colorIndexBytes = r.readBytes(4)
        let bitsPerColorIndex = 2
        for rowIndex in 0..<4 {
            let rowBaseColorIndexIndex = 4 * rowIndex
            let rowBaseBitOffset = 8 * rowIndex
            for columnIndex in 0..<4 {
                // Color indices are arranged from right to left.
                let bitOffset = rowBaseBitOffset + (bitsPerColorIndex * (3 - columnIndex))
                colorIndices[rowBaseColorIndexIndex + columnIndex] = Int(Utils.getBits(bitOffset, bitsPerColorIndex, colorIndexBytes))
            }
        }
        // Calculate pixel colors.
        var colors = [Color32](); colors.reserveCapacity(16)
        for i in 0..<16 { colors.append(Color32(color: colorTable[colorIndices[i]])) }
        return colors
    }

    static func decodeDXT1TexelBlock(_ r: BinaryReader, containsAlpha: Bool) -> [Color32] {
        // Create the color table.
        var colorTable = [Color](); colorTable.reserveCapacity(4)
        colorTable.append(Color.color(b565: r.readLEUInt16()))
        colorTable.append(Color.color(b565: r.readLEUInt16()))
        if !containsAlpha {
            colorTable.append(Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 3.0)!)
            colorTable.append(Color.lerp(colorTable[0], colorTable[1], fraction: 2.0 / 3.0)!)
        }
        else {
            colorTable.append(Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 2.0)!)
            colorTable.append(Color(red: 0, green: 0, blue: 0, alpha: 0))
        }
        // Calculate pixel colors.
        return decodeDXT1TexelBlock(r, colorTable: colorTable)
    }

    static func decodeDXT3TexelBlock(_ r: BinaryReader) -> [Color32] {
        // Read compressed pixel alphas.
        var compressedAlphas = [UInt8](); compressedAlphas.reserveCapacity(16)
        for _ in 0..<4 {
            let compressedAlphaRow = Int(r.readLEUInt16())
            for columnIndex in 0..<4 {
                // Each compressed alpha is 4 bits.
                compressedAlphas.append(UInt8((compressedAlphaRow >> (columnIndex * 4)) & 0xF))
            }
        }
        // Calculate pixel alphas.
        var alphas = [UInt8](); alphas.reserveCapacity(16)
        for i in 0..<16 {
            let alphaPercent = Float(compressedAlphas[i] / 15)
            alphas.append(UInt8((alphaPercent * 255).roundedAsInt()))
        }
        // Create the color table.
        var colorTable = [Color](); colorTable.reserveCapacity(4)
        colorTable.append(Color.color(b565: r.readLEUInt16()))
        colorTable.append(Color.color(b565: r.readLEUInt16()))
        colorTable.append(Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 3.0)!)
        colorTable.append(Color.lerp(colorTable[0], colorTable[1], fraction: 2.0 / 3.0)!)
        // Calculate pixel colors.
        var colors = decodeDXT1TexelBlock(r, colorTable: colorTable)
        for i in 0..<16 { colors[i].alpha = alphas[i] }
        return colors
    }

    static func decodeDXT5TexelBlock(_ r: BinaryReader) -> [Color32] {
        // Create the alpha table.
        var alphaTable = [Float](); alphaTable.reserveCapacity(8)
        alphaTable.append(Float(r.readByte()))
        alphaTable.append(Float(r.readByte()))
        if alphaTable[0] > alphaTable[1] {
            for i in 0..<6 {
                alphaTable.append(Float.lerp(alphaTable[0], alphaTable[1], t: Float(1 + i) / 7.0))
            }
        }
        else {
            for i in 0..<4 {
                alphaTable.append(Float.lerp(alphaTable[0], alphaTable[1], t: Float(1 + i) / 5.0))
            }
            alphaTable.append(0)
            alphaTable.append(255)
        }

        // Read pixel alpha indices.
        var alphaIndices = [Int](); alphaIndices.reserveCapacity(16)
        var alphaIndexBytesRow0 = r.readBytes(3)
        alphaIndexBytesRow0.reverse() // Take care of little-endianness.
        var alphaIndexBytesRow1 = r.readBytes(3) // Take care of little-endianness.
        alphaIndexBytesRow1.reverse() // Take care of little-endianness.
        let bitsPerAlphaIndex = 3
        alphaIndices.append(Int(Utils.getBits(21, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(18, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(15, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(12, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(9, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(6, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(3, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(0, bitsPerAlphaIndex, alphaIndexBytesRow0)))
        alphaIndices.append(Int(Utils.getBits(21, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        alphaIndices.append(Int(Utils.getBits(18, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        alphaIndices.append(Int(Utils.getBits(15, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        alphaIndices.append(Int(Utils.getBits(12, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        alphaIndices.append(Int(Utils.getBits(9, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        alphaIndices.append(Int(Utils.getBits(6, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        alphaIndices.append(Int(Utils.getBits(3, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        alphaIndices.append(Int(Utils.getBits(0, bitsPerAlphaIndex, alphaIndexBytesRow1)))
        // Create the color table.
        var colorTable = [Color](); colorTable.reserveCapacity(4)
        colorTable.append(Color.color(b565: r.readLEUInt16()))
        colorTable.append(Color.color(b565: r.readLEUInt16()))
        colorTable.append(Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 3.0)!)
        colorTable.append(Color.lerp(colorTable[0], colorTable[1], fraction: 2.0 / 3.0)!)
        // Calculate pixel colors.
        var colors = decodeDXT1TexelBlock(r, colorTable: colorTable)
        for i in 0..<16 {
            colors[i].alpha = UInt8(alphaTable[alphaIndices[i]].rounded())
        }
        return colors
    }

    static func copyDecodedTexelBlock(decodedTexels: [Color32], argb: inout Data, baseARGBIndex: Int, baseRowIndex: Int, baseColumnIndex: Int, textureWidth: Int, textureHeight: Int) {
        for i in 0..<4 { // row
            for j in 0..<4 { // column
                let rowIndex = baseRowIndex + i
                let columnIndex = baseColumnIndex + j
                // Don't copy padding on mipmaps.
                if (rowIndex < textureHeight && columnIndex < textureWidth) {
                    let decodedTexelIndex = (4 * i) + j
                    let color = decodedTexels[decodedTexelIndex]
                    let argbPixelOffset = (textureWidth * rowIndex) + columnIndex
                    let basePixelARGBIndex = baseARGBIndex + (4 * argbPixelOffset)
                    argb[basePixelARGBIndex] = color.alpha
                    argb[basePixelARGBIndex + 1] = color.red
                    argb[basePixelARGBIndex + 2] = color.green
                    argb[basePixelARGBIndex + 3] = color.blue
                }
            }
        }
    }

    static func decodeDXTToARGB(dxtVersion: Int, compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: Int) -> Data {
        let alphaFlag = pixelFormat.dwFlags.contains(.alphaPixels)
        let containsAlpha = alphaFlag || (pixelFormat.dwRGBBitCount == 32 && pixelFormat.dwABitMask != 0)
        let r = BinaryReader(DataBaseStream(data: compressedData))
        defer { r.close() }
        var argb = Data(count: TextureUtils.calculateMipMappedTextureDataSize(width: width, height: height, bytesPerPixel: 4))
        var mipMapWidth = width
        var mipMapHeight = height
        var baseARGBIndex = 0
        for _ in 0..<mipmapCount {
            for rowIndex in stride(from: 0, to: mipMapHeight, by: 4) {
                for columnIndex in stride(from: 0, to: mipMapWidth, by: 4) {
                    let colors: [Color32]
                    switch dxtVersion { // Doing a switch instead of using a delegate for speed.
                        case 1: colors = decodeDXT1TexelBlock(r, containsAlpha: containsAlpha)
                        case 3: colors = decodeDXT3TexelBlock(r)
                        case 5: colors = decodeDXT5TexelBlock(r)
                        default: fatalError("Tried decoding a DDS file using an unsupported DXT format: DXT \(dxtVersion))")
                    }
                    copyDecodedTexelBlock(decodedTexels: colors, argb: &argb, baseARGBIndex: baseARGBIndex, baseRowIndex: rowIndex, baseColumnIndex: columnIndex, textureWidth: mipMapWidth, textureHeight: mipMapHeight)
                }
            }
            baseARGBIndex += mipMapWidth * mipMapHeight * 4
            mipMapWidth /= 2
            mipMapHeight /= 2
        }
        return argb
    }

    static func extractDDSTextureFormatAndData(_ header: DDSHeader, _ r: BinaryReader) -> (hasMipmaps: Bool, ddsMipmapLevelCount: Int, textureFormat: TextureFormat, bytesPerPixel: Int, textureData: Data) {
        let hasMipmaps = header.dwCaps.contains(.mipmap)
        // Non-mipmapped textures still have one mipmap level: the texture itself.
        let ddsMipmapLevelCount = hasMipmaps ? Int(header.dwMipMapCount) : 1
        // If the DDS file contains uncompressed data.
        var textureFormat: TextureFormat
        var bytesPerPixel: Int
        var textureData: Data
        if header.ddspf.dwFlags.contains(.rgb) {
            // some permutation of RGB
            guard header.ddspf.dwFlags.contains(.alphaPixels) else {
                fatalError("Unsupported DDS file pixel format.")
            }
            // There should be 32 bits per pixel.
            guard header.ddspf.dwRGBBitCount == 32 else {
                fatalError("Invalid DDS file pixel format.")
            }
            // BGRA32
            if header.ddspf.dwBBitMask == 0x000000FF && header.ddspf.dwGBitMask == 0x0000FF00 &&
                header.ddspf.dwRBitMask == 0x00FF0000 && header.ddspf.dwABitMask == 0xFF000000 {
                textureFormat = CIFormat.BGRA8
                bytesPerPixel = 4
            }
            // ARGB32
            else if header.ddspf.dwABitMask == 0x000000FF && header.ddspf.dwRBitMask == 0x0000FF00 &&
                header.ddspf.dwGBitMask == 0x00FF0000 && header.ddspf.dwBBitMask == 0xFF000000 {
                textureFormat = CIFormat.ARGB8
                bytesPerPixel = 4
            }
            else { fatalError("Unsupported DDS file pixel format.") }

            if !hasMipmaps { textureData = Data(count: Int(header.dwPitchOrLinearSize * header.dwHeight)) }
            // Create a data buffer to hold all mipmap levels down to 1x1.
            else { textureData = Data(count: TextureUtils.calculateMipMappedTextureDataSize(width: Int(header.dwWidth), height: Int(header.dwHeight), bytesPerPixel: bytesPerPixel)) }
            r.readRestOfBytes(&textureData, offsetBy: 0)
        }
        else if header.ddspf.dwFourCC == "DXT1" {
            textureFormat = CIFormat.ARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXTToARGB(dxtVersion: 1, compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else if header.ddspf.dwFourCC == "DXT3" {
            textureFormat = CIFormat.ARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXTToARGB(dxtVersion: 3, compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else if header.ddspf.dwFourCC == "DXT5" {
            textureFormat = CIFormat.ARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXTToARGB(dxtVersion: 5, compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else { fatalError("Unsupported DDS file pixel format.") }
            
        return (hasMipmaps: hasMipmaps, ddsMipmapLevelCount: ddsMipmapLevelCount, textureFormat: textureFormat, bytesPerPixel: bytesPerPixel, textureData: textureData)
    }

    static func postProcessDDSTexture(width: Int, height: Int, bytesPerPixel: Int, hasMipmaps: Bool, ddsMipmapLevelCount: Int, data: inout Data, flipVertically: Bool) {
        assert(width > 0 && height > 0 && bytesPerPixel > 0 && ddsMipmapLevelCount > 0); // && data != nil)
        // Flip mip-maps if necessary and generate missing mip-map levels.
        var mipMapLevelWidth = width
        var mipMapLevelHeight = height
        var mipMapLevelIndex = 0
        var mipMapLevelDataOffset = 0

        // While we haven't processed all of the mipmap levels we should process.
        while mipMapLevelWidth > 1 || mipMapLevelHeight > 1 {
            let mipMapDataSize = mipMapLevelWidth * mipMapLevelHeight * bytesPerPixel
            // If the DDS file contains the current mipmap level, flip it vertically if necessary.
            if flipVertically && mipMapLevelIndex < ddsMipmapLevelCount {
                SequenceUtils.flip2DSubArrayVertically(&data, offsetBy: mipMapLevelDataOffset, rows: mipMapLevelHeight, bytesPerRow: mipMapLevelWidth * bytesPerPixel)
            }
            // Break after optionally flipping the first mipmap level if the DDS texture doesn't have mipmaps.
            guard hasMipmaps else {
                break
            }
            // Generate the next mipmap level's data if the DDS file doesn't contain it.
            if mipMapLevelIndex + 1 >= ddsMipmapLevelCount {
                TextureUtils.downscale4Component32BitPixelsX2(srcData: data, srcStartIndex: mipMapLevelDataOffset, srcRowCount: mipMapLevelHeight, srcColumnCount: mipMapLevelWidth, dstData: &data, dstStartIndex: mipMapLevelDataOffset + mipMapDataSize)
            }
            // Switch to the next mipmap level.
            mipMapLevelIndex += 1
            mipMapLevelWidth = mipMapLevelWidth > 1 ? (mipMapLevelWidth / 2) : mipMapLevelWidth
            mipMapLevelHeight = mipMapLevelHeight > 1 ? (mipMapLevelHeight / 2) : mipMapLevelHeight
            mipMapLevelDataOffset += mipMapDataSize
        }
    }
}
