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
        dwReserved1 = [UInt32](); dwReserved1.reserveCapacity(11)
        for i in 0..<11 {
            dwReserved1[i] = r.readLEUInt32()
        }
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

    init() {
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
    public static func loadDDSTexture(filePath: String, flipVertically: Bool = false) -> Texture2DInfo {
        let r = BinaryReader(FileBaseStream(forReadingAtPath: filePath)!)
        defer { r.close() }
        return loadDDSTexture(r, flipVertically: flipVertically)
    }

    public static func loadDDSTexture(_ r: BinaryReader, flipVertically: Bool = false) -> Texture2DInfo {
        // Check the magic string.
        let magicString = r.readASCIIString(4)
        guard magicString == "DDS " else {
            fatalError("Invalid DDS file magic string: '\(magicString)'.")
        }
        // Deserialize the DDS file header.
        let header = DDSHeader(r)
        // Figure out the texture format and load the texture data.
        var hasMipmaps = false
        var ddsMipmapLevelCount = 0
        var textureFormat = TextureFormat()
        var bytesPerPixel = 0
        var textureData: Data
        extractDDSTextureFormatAndData(header, r,
                                       hasMipmaps: &hasMipmaps, ddsMipmapLevelCount: &ddsMipmapLevelCount,
                                       textureFormat: &textureFormat, bytesPerPixel: &bytesPerPixel, textureData: &textureData)
        // Post-process the texture to generate missing mipmaps and possibly flip it vertically.
        postProcessDDSTexture(width: Int(header.dwWidth), height: Int(header.dwHeight), bytesPerPixel: bytesPerPixel,
                              hasMipmaps: hasMipmaps, ddsMipmapLevelCount: ddsMipmapLevelCount,
                              data: &textureData, flipVertically: flipVertically)
        return Texture2DInfo(width: Int(header.dwWidth), height: Int(header.dwHeight),
                             format: textureFormat, hasMipmaps: hasMipmaps,
                             bytesPerPixel: bytesPerPixel, rawData: textureData)
    }

    static func decodeDXT1TexelBlock(_ r: BinaryReader, colorTable: [Color]) -> [Color32] {
        assert(colorTable.count == 4)
        // Read pixel color indices.
        var colorIndices = [Int](); colorIndices.reserveCapacity(16)
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
        for i in 0..<16 {
            colors[i] = Color32(color: colorTable[colorIndices[i]])
        }
        return colors
    }

    static func decodeDXT1TexelBlock(_ r: BinaryReader, containsAlpha: Bool) -> [Color32] {
        // Create the color table.
        var colorTable = [Color](); colorTable.reserveCapacity(4)
        colorTable[0] = Color.color(b565: r.readLEUInt16())
        colorTable[1] = Color.color(b565: r.readLEUInt16())
        if !containsAlpha {
            colorTable[2] = Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 3)!
            colorTable[3] = Color.lerp(colorTable[0], colorTable[1], fraction: 2.0 / 3)!
        }
        else {
            colorTable[2] = Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 2)!
            colorTable[3] = Color(red: 0, green: 0, blue: 0, alpha: 0)
        }
        // Calculate pixel colors.
        return decodeDXT1TexelBlock(r, colorTable: colorTable)
    }

    static func decodeDXT3TexelBlock(_ r: BinaryReader) -> [Color32] {
        // Read compressed pixel alphas.
        var compressedAlphas = [UInt8](); compressedAlphas.reserveCapacity(16)
        for rowIndex in 0..<4 {
            let compressedAlphaRow = Int(r.readLEUInt16())
            for columnIndex in 0..<4 {
                // Each compressed alpha is 4 bits.
                compressedAlphas[(4 * rowIndex) + columnIndex] = UInt8((compressedAlphaRow >> (columnIndex * 4)) & 0xF)
            }
        }
        // Calculate pixel alphas.
        var alphas = [UInt8](); alphas.reserveCapacity(16)
        for i in 0..<16 {
            let alphaPercent = Float(compressedAlphas[i] / 15)
            alphas[i] = UInt8((alphaPercent * 255).roundedAsInt())
        }
        // Create the color table.
        var colorTable = [Color](); colorTable.reserveCapacity(4)
        colorTable[0] = Color.color(b565: r.readLEUInt16())
        colorTable[1] = Color.color(b565: r.readLEUInt16())
        colorTable[2] = Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 3)!
        colorTable[3] = Color.lerp(colorTable[0], colorTable[1], fraction: 2.0 / 3)!
        // Calculate pixel colors.
        var colors = decodeDXT1TexelBlock(r, colorTable: colorTable)
        for i in 0..<16 {
            colors[i].alpha = alphas[i]
        }
        return colors
    }

    static func decodeDXT5TexelBlock(_ r: BinaryReader) -> [Color32] {
        // Create the alpha table.
        var alphaTable = [Float](); alphaTable.reserveCapacity(8)
        alphaTable[0] = Float(r.readByte())
        alphaTable[1] = Float(r.readByte())
        if alphaTable[0] > alphaTable[1] {
            for i in 0..<6 {
                alphaTable[2 + i] = Float.lerp(alphaTable[0], alphaTable[1], t: Float(1 + i) / 7)
            }
        }
        else {
            for i in 0..<4 {
                alphaTable[2 + i] = Float.lerp(alphaTable[0], alphaTable[1], t: Float(1 + i) / 5)
            }
            alphaTable[6] = 0
            alphaTable[7] = 255
        }

        // Read pixel alpha indices.
        var alphaIndices = [Int](); alphaIndices.reserveCapacity(16)
        var alphaIndexBytesRow0 = r.readBytes(3)
        alphaIndexBytesRow0.reverse() // Take care of little-endianness.
        var alphaIndexBytesRow1 = r.readBytes(3) // Take care of little-endianness.
        alphaIndexBytesRow1.reverse() // Take care of little-endianness.
        let bitsPerAlphaIndex = 3
        alphaIndices[0] = Int(Utils.getBits(21, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[1] = Int(Utils.getBits(18, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[2] = Int(Utils.getBits(15, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[3] = Int(Utils.getBits(12, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[4] = Int(Utils.getBits(9, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[5] = Int(Utils.getBits(6, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[6] = Int(Utils.getBits(3, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[7] = Int(Utils.getBits(0, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[8] = Int(Utils.getBits(21, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[9] = Int(Utils.getBits(18, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[10] = Int(Utils.getBits(15, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[11] = Int(Utils.getBits(12, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[12] = Int(Utils.getBits(9, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[13] = Int(Utils.getBits(6, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[14] = Int(Utils.getBits(3, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[15] = Int(Utils.getBits(0, bitsPerAlphaIndex, alphaIndexBytesRow1))
        // Create the color table.
        var colorTable = [Color](); colorTable.reserveCapacity(4)
        colorTable[0] = Color.color(b565: r.readLEUInt16())
        colorTable[1] = Color.color(b565: r.readLEUInt16())
        colorTable[2] = Color.lerp(colorTable[0], colorTable[1], fraction: 1.0 / 3)!
        colorTable[3] = Color.lerp(colorTable[0], colorTable[1], fraction: 2.0 / 3)!
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
    static func decodeDXT1ToARGB(compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: Int) -> Data {
        return decodeDXTToARGB(dxtVersion: 1, compressedData: compressedData, width: width, height: height, pixelFormat: pixelFormat, mipmapCount: mipmapCount) }
    static func decodeDXT3ToARGB(compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: Int) -> Data {
        return decodeDXTToARGB(dxtVersion: 3, compressedData: compressedData, width: width, height: height, pixelFormat: pixelFormat, mipmapCount: mipmapCount) }
    static func decodeDXT5ToARGB(compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: Int) -> Data {
        return decodeDXTToARGB(dxtVersion: 5, compressedData: compressedData, width: width, height: height, pixelFormat: pixelFormat, mipmapCount: mipmapCount) }

    static func extractDDSTextureFormatAndData(_ header: DDSHeader, _ r: BinaryReader, hasMipmaps: inout Bool, ddsMipmapLevelCount: inout Int,
                                               textureFormat: inout TextureFormat, bytesPerPixel: inout Int, textureData: inout Data) {
        hasMipmaps = header.dwCaps.contains(.mipmap)
        // Non-mipmapped textures still have one mipmap level: the texture itself.
        ddsMipmapLevelCount = hasMipmaps ? Int(header.dwMipMapCount) : 1
        // If the DDS file contains uncompressed data.
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
                textureFormat = kCIFormatBGRA8
                bytesPerPixel = 4
            }
            // ARGB32
            else if header.ddspf.dwABitMask == 0x000000FF && header.ddspf.dwRBitMask == 0x0000FF00 &&
                header.ddspf.dwGBitMask == 0x00FF0000 && header.ddspf.dwBBitMask == 0xFF000000 {
                textureFormat = kCIFormatARGB8
                
                bytesPerPixel = 4
            }
            else { fatalError("Unsupported DDS file pixel format.") }

            if !hasMipmaps { textureData = Data(count: Int(header.dwPitchOrLinearSize * header.dwHeight)) }
            // Create a data buffer to hold all mipmap levels down to 1x1.
            else { textureData = Data(count: TextureUtils.calculateMipMappedTextureDataSize(width: Int(header.dwWidth), height: Int(header.dwHeight), bytesPerPixel: bytesPerPixel)) }
            r.readRestOfBytes(&textureData, offsetBy: 0)
        }
        else if header.ddspf.dwFourCC == "DXT1" {
            textureFormat = kCIFormatARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXT1ToARGB(compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else if header.ddspf.dwFourCC == "DXT3" {
            textureFormat = kCIFormatARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXT3ToARGB(compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else if header.ddspf.dwFourCC == "DXT5" {
            textureFormat = kCIFormatARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXT5ToARGB(compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else { fatalError("Unsupported DDS file pixel format.") }
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
