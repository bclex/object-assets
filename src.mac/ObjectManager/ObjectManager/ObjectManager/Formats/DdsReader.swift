//
//  DdsReader.swift
//  ObjectManager
//
//  Created by Sky Morey on 6/5/18.
//  Copyright © 2018 Sky Morey. All rights reserved.
//
public enum DDSFlags: UInt32 { //: DDSD
    case caps = 0x1             // Required in every .dds file.
    case height = 0x2           // Required in every .dds file.
    case width = 0x4            // Required in every .dds file.
    case pitch = 0x8            // Required when pitch is provided for an uncompressed texture.
    case pixelFormat = 0x1000   // Required in every .dds file.
    case mipmapCount = 0x20000  // Required in a mipmapped texture.
    case linearSize = 0x80000   // Required when pitch is provided for a compressed texture.
    case depth = 0x800000       // Required in a depth texture.
    case HEADER_FLAGS_TEXTURE = Caps | Height | Width | PixelFormat
    case HEADER_FLAGS_MIPMAP = MipmapCount
    case HEADER_FLAGS_VOLUME = Depth
    case HEADER_FLAGS_PITCH = Pitch
    case HEADER_FLAGS_LINEARSIZE = LinearSize
}

public enum DDSCaps: UInt32 { //: DDSCAPS
    case Complex = 0x8          // Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
    case Mipmap = 0x400000      // Optional; should be used for a mipmap.
    case Texture = 0x1000       // Required
    case SURFACE_FLAGS_MIPMAP = Complex | Mipmap
    case SURFACE_FLAGS_TEXTURE = Texture
    case SURFACE_FLAGS_CUBEMAP = Complex
}

public enum DDSCaps2: UInt32 { //: DDSCAPS2
    case Cubemap = 0x200            // Required for a cube map.
    case CubemapPositiveX = 0x400   // Required when these surfaces are stored in a cube map.	
    case CubemapNegativeX = 0x800   // Required when these surfaces are stored in a cube map.
    case CubemapPositiveY = 0x1000  // Required when these surfaces are stored in a cube map.
    case CubemapNegativeY = 0x2000  // Required when these surfaces are stored in a cube map.
    case CubemapPositiveZ = 0x4000  // Required when these surfaces are stored in a cube map.
    case CubemapNegativeZ = 0x8000  // Required when these surfaces are stored in a cube map.
    case Volume = 0x200000          // Required for a volume texture.
    case CUBEMAP_POSITIVEX = Cubemap | CubemapPositiveX
    case CUBEMAP_NEGATIVEX = Cubemap | CubemapNegativeX
    case CUBEMAP_POSITIVEY = Cubemap | CubemapPositiveY
    case CUBEMAP_NEGATIVEY = Cubemap | CubemapNegativeY
    case CUBEMAP_POSITIVEZ = Cubemap | CubemapPositiveZ
    case CUBEMAP_NEGATIVEZ = Cubemap | CubemapNegativeZ
    case CUBEMAP_ALLFACES = CubemapPositiveX | CubemapNegativeX | CubemapPositiveY | CubemapNegativeY | CubemapPositiveZ | CubemapNegativeZ
    case FLAGS_VOLUME = Volume
}

public struct DDSHeader { //: DDS_HEADER
    public var dwSize: UInt32 {return 124}
    public let dwFlags: DDSFlags
    public let dwHeight: UInt32
    public let dwWidth: UInt32
    public let dwPitchOrLinearSize: UInt32
    public let dwDepth: UInt32 // only if DDS_HEADER_FLAGS_VOLUME is set in flags
    public let dwMipMapCount: UInt32
    public let dwReserved1: [UInt32]
    public let ddspf: DDSPixelFormat
    public let dwCaps: DDSCaps // dwSurfaceFlags
    public let dwCaps2: DDSCaps2 // dwCubemapFlags
    public let dwCaps3: UInt32
    public let dwCaps4: UInt32
    public let dwReserved2: UInt32

    init(_ r: BinaryReader) {
        var size = r.readLEUInt32()
        guard size == 124 else {
            fatalError("Invalid DDS file header size: \(size).")
        }
        dwFlags = DDSFlags(rawValue: r.readLEUInt32())
        guard Utils.containsBitFlags(dwFlags.rawValue, DDSFlags.height, DDSFlags.width) else {
            fatalError("Invalid DDS file flags: \(dwFlags).")
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
        ddspf.read(r)
        dwCaps = DDSCaps(rawValue: r.readLEUInt32())
        guard Utils.containsBitFlags(dwCaps, DDSCaps.Texture) else {
            fatalError("Invalid DDS file caps: \(dwCaps).")
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
        for i in 0..< 11 {
            w.write(dwReserved1[i])
        }
        ddspf.write(w)
        w.write((uint)dwCaps)
        w.write((uint)dwCaps2)
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

public enum DDSPixelFormats: UInt32 { //: DDS_PIXELFORMAT
    case alphaPixels = 0x1      // Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
    case alpha = 0x2            // Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
    case fourCC = 0x4           // Texture contains compressed RGB data; dwFourCC contains valid data.
    case rgb = 0x40             // Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
    case yuv = 0x200            // Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
    case luminance = 0x20000    // Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
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
        var size = r.readLEUInt32()
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

// https://msdn.microsoft.com/en-us/library/windows/desktop/bb173059(v=vs.85).aspx
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

public static class DdsReader {
    public static func loadDDSTexture(filePath: String, flipVertically: Bool = false) -> Texture2DInfo {
        let r = BinaryReader(FileBaseStream(path: filePath))
        defer { r.close() }
        return loadDDSTexture(r, flipVertically)
    }

    public static func loadDDSTexture(_ r: binaryReader, flipVertically: Bool = false) -> Texture2DInfo {
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
        var textureData = Data()
        extractDDSTextureFormatAndData(header, r, hasMipmaps: &hasMipmaps, ddsMipmapLevelCount: &ddsMipmapLevelCount, textureFormat: &textureFormat, bytesPerPixel: &bytesPerPixel, textureData: &textureData)
        // Post-process the texture to generate missing mipmaps and possibly flip it vertically.
        postProcessDDSTexture(width: Int(header.dwWidth), height: int(header.dwHeight), bytesPerPixel: bytesPerPixel, hasMipmaps: hasMipmaps, ddsMipmapLevelCount: ddsMipmapLevelCount, textureData: textureData, flipVertically: flipVertically)
        return Texture2DInfo(width: Int(header.dwWidth), height: Int(header.dwHeight), format: textureFormat, hasMipmaps: hasMipmaps, bytesPerPixel: bytesPerPixel, data: textureData)
    }

    static func decodeDXT1TexelBlock(_ r: BinaryReader, colorTable: [Color]) -> [Color32] {
        assert(colorTable.count == 4)
        // Read pixel color indices.
        var colorIndices = [UInt32](); colorIndices.reserveCapacity(16)
        var colorIndexBytes = r.readBytes(4)
        let bitsPerColorIndex = 2
        for rowIndex in 0..<4 {
            let rowBaseColorIndexIndex = 4 * rowIndex
            let rowBaseBitOffset = 8 * rowIndex
            for columnIndex in 0..<4 {
                // Color indices are arranged from right to left.
                let bitOffset = rowBaseBitOffset + (bitsPerColorIndex * (3 - columnIndex))
                colorIndices[rowBaseColorIndexIndex + columnIndex] = Utils.getBits(bitOffset, bitsPerColorIndex, colorIndexBytes)
            }
        }
        // Calculate pixel colors.
        var colors = [Color32](); colors.allocateCapacity(16)
        for i 0..<16 {
            colors[i] = colorTable[colorIndices[i]]
        }
        return colors
    }

    static func decodeDXT1TexelBlock(_ r: BinaryReader, containsAlpha: Bool) -> [Color32] {
        // Create the color table.
        var colorTable = [Color](); colorTable.allocateCapacity(4)
        colorTable[0] = Color(b565: r.readLEUInt16())
        colorTable[1] = Color(b565: r.readLEUInt16())
        if !containsAlpha {
            colorTable[2] = Color.lerp(colorTable[0], colorTable[1], 1.0 / 3)
            colorTable[3] = Color.lerp(colorTable[0], colorTable[1], 2.0 / 3)
        }
        else {
            colorTable[2] = Color.lerp(colorTable[0], colorTable[1], 1.0 / 2)
            colorTable[3] = Color(0, 0, 0, 0)
        }
        // Calculate pixel colors.
        return decodeDXT1TexelBlock(r, colorTable: colorTable)
    }

    static func decodeDXT3TexelBlock(_ r: BinaryReader) -> [Color32] {
        // Read compressed pixel alphas.
        var compressedAlphas = [UInt8](); compressedAlphas.reserveCapacity(16)
        for rowIndex in 0..<4 {
            var compressedAlphaRow = r.readLEUInt16()
            for columnIndex = 0..<4 {
                // Each compressed alpha is 4 bits.
                compressedAlphas[(4 * rowIndex) + columnIndex] = UInt8((compressedAlphaRow >> (columnIndex * 4)) & 0xF)
            }
        }
        // Calculate pixel alphas.
        var alphas = [UInt8](); alphas.allocateCapacity(16)
        for i in 0..<16 {
            let alphaPercent = Float(compressedAlphas[i] / 15)
            alphas[i] = UInt8((alphaPercent * 255).roundToInt())
        }
        // Create the color table.
        var colorTable = [Color](); colorTable.allocateCapacity(4)
        colorTable[0] = Color(b565: r.readLEUInt16())
        colorTable[1] = Color(b565: r.readLEUInt16())
        colorTable[2] = Color.lerp(colorTable[0], colorTable[1], 1.0 / 3)
        colorTable[3] = Color.lerp(colorTable[0], colorTable[1], 2.0 / 3)
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
        alphaTable[0] = r.readByte()
        alphaTable[1] = r.readByte()
        if alphaTable[0] > alphaTable[1] {
            for i in 0..<6 {
                alphaTable[2 + i] = Float.lerp(alphaTable[0], alphaTable[1], Float(1 + i) / 7)
            }
        }
        else {
            for i in 0..<4 {
                alphaTable[2 + i] = Float.lerp(alphaTable[0], alphaTable[1], Float(1 + i) / 5)
            }
            alphaTable[6] = 0
            alphaTable[7] = 255
        }

        // Read pixel alpha indices.
        var alphaIndices = [UInt32](); alphaIndices.reserveCapacity(16)
        var alphaIndexBytesRow0 = r.readBytes(3)
        alphaIndexBytesRow0.reverse() // Take care of little-endianness.
        var alphaIndexBytesRow1 = r.readBytes(3) // Take care of little-endianness.
        alphaIndexBytesRow1.reverse() // Take care of little-endianness.
        let bitsPerAlphaIndex = 3
        alphaIndices[0] = UInt32(Utils.getBits(21, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[1] = UInt32(Utils.getBits(18, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[2] = UInt32(Utils.getBits(15, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[3] = UInt32(Utils.getBits(12, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[4] = UInt32(Utils.getBits(9, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[5] = UInt32(Utils.getBits(6, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[6] = UInt32(Utils.getBits(3, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[7] = UInt32(Utils.getBits(0, bitsPerAlphaIndex, alphaIndexBytesRow0))
        alphaIndices[8] = UInt32(Utils.getBits(21, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[9] = UInt32(Utils.getBits(18, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[10] = UInt32(Utils.getBits(15, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[11] = UInt32(Utils.getBits(12, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[12] = UInt32(Utils.getBits(9, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[13] = UInt32(Utils.getBits(6, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[14] = UInt32(Utils.getBits(3, bitsPerAlphaIndex, alphaIndexBytesRow1))
        alphaIndices[15] = UInt32(Utils.getBits(0, bitsPerAlphaIndex, alphaIndexBytesRow1))
        // Create the color table.
        var colorTable = [Color](); colorTable.reserveCapacity(4)
        colorTable[0] = Color(b565: r.readLEUInt16())
        colorTable[1] = Color(b565: r.readLEUInt16())
        colorTable[2] = Color.lerp(colorTable[0], colorTable[1], 1.0 / 3)
        colorTable[3] = Color.lerp(colorTable[0], colorTable[1], 2.0 / 3)
        // Calculate pixel colors.
        var colors = decodeDXT1TexelBlock(r, colorTable: colorTable)
        for i in 0..<16 {
            colors[i].alpha = UInt8(alphaTable[alphaIndices[i]].round())
        }
        return colors
    }

    static func copyDecodedTexelBlock(decodedTexels: [Color32], argb: [byte], baseARGBIndex: Int, baseRowIndex: Int, baseColumnIndex: Int, textureWidth: Int, textureHeight: Int) {
        for i = 0..<4 { // row
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

    static func decodeDXTToARGB(dxtVersion: Int, compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: UInt32) -> [UInt8] {
        let alphaFlag = Utils.containsBitFlags(pixelFormat.dwFlags.rawValue, DDSPixelFormats.alphaPixels)
        let containsAlpha = alphaFlag || (pixelFormat.dwRGBBitCount == 32 && pixelFormat.dwABitMask != 0)
        let r = BinaryReader(DataBaseStream(compressedData))
        defer { r.close() }
        var argb = [UInt8](count: TextureUtils.calculateMipMappedTextureDataSize(width, height, 4))
        var mipMapWidth = width
        var mipMapHeight = height
        var baseARGBIndex = 0
        for mipMapIndex in 0..<mipmapCount {
            for rowIndex in stride(from: 0, to: mipMapHeight, by: 4) {
                for columnIndex in stride(from: 0, to: mipMapWidth, by: 4) {
                    let colors: [Color32]
                    switch dxtVersion { // Doing a switch instead of using a delegate for speed.
                        case 1: colors = decodeDXT1TexelBlock(r, containsAlpha: containsAlpha)
                        case 3: colors = decodeDXT3TexelBlock(r)
                        case 5: colors = decodeDXT5TexelBlock(r)
                        default: fatalError("Tried decoding a DDS file using an unsupported DXT format: DXT \(dxtVersion))"
                    }
                    copyDecodedTexelBlock(colors: colors, argb: argb, baseARGBIndex: baseARGBIndex, rowIndex: rowIndex, columnIndex: columnIndex, mipMapWidth: mipMapWidth, mipMapHeight: mipMapHeight)
                }
            }
            baseARGBIndex += mipMapWidth * mipMapHeight * 4
            mipMapWidth /= 2
            mipMapHeight /= 2
        }
        return argb
    }
    static func decodeDXT1ToARGB(compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: UInt32) -> [UInt8] { return decodeDXTToARGB(dxtVersion: 1, compressedData: compressedData, width: width, height: height, pixelFormat: pixelFormat, mipmapCount: mipmapCount) }
    static func decodeDXT3ToARGB(compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: UInt32) -> [UInt8] { return decodeDXTToARGB(dxtVersion: 3, compressedData: compressedData, width: width, height: height, pixelFormat: pixelFormat, mipmapCount: mipmapCount) }
    static func decodeDXT5ToARGB(compressedData: Data, width: Int, height: Int, pixelFormat: DDSPixelFormat, mipmapCount: UInt32) -> [UInt8] { return decodeDXTToARGB(dxtVersion: 5, compressedData: compressedData, width: width, height: height, pixelFormat: pixelFormat, mipmapCount: mipmapCount) }

    static func extractDDSTextureFormatAndData(_ header: DDSHeader, _ r: BinaryReader r, hasMipmaps: inout Bool, ddsMipmapLevelCount: inout UInt, textureFormat: inout TextureFormat, bytesPerPixel: inout Int, textureData: inout [byte]) {
        hasMipmaps = Utils.containsBitFlags(Int(header.dwCaps), DDSCaps.mipmap)
        // Non-mipmapped textures still have one mipmap level: the texture itself.
        ddsMipmapLevelCount = hasMipmaps ? header.dwMipMapCount : 1
        // If the DDS file contains uncompressed data.
        if Utils.containsBitFlags(Int(header.ddspf.dwFlags), DDSPixelFormats.rgb) {
            // some permutation of RGB
            guard Utils.containsBitFlags(Int(header.ddspf.dwFlags), DDSPixelFormats.alphaPixels) else {
                fatalError("Unsupported DDS file pixel format.")
            }
            // There should be 32 bits per pixel.
            guard header.ddspf.dwRGBBitCount == 32 else {
                fatalError("Invalid DDS file pixel format.")
            }
            // BGRA32
            if header.ddspf.dwBBitMask == 0x000000FF && header.ddspf.dwGBitMask == 0x0000FF00 && header.ddspf.dwRBitMask == 0x00FF0000 && header.ddspf.dwABitMask == 0xFF000000 {
                textureFormat = TextureFormat.BGRA8
                bytesPerPixel = 4
            }
            // ARGB32
            else if header.ddspf.dwABitMask == 0x000000FF && header.ddspf.dwRBitMask == 0x0000FF00 && header.ddspf.dwGBitMask == 0x00FF0000 && header.ddspf.dwBBitMask == 0xFF000000 {
                textureFormat = TextureFormat.ARGB8
                bytesPerPixel = 4
            }
            else { fatalError("Unsupported DDS file pixel format.") }

            if !hasMipmaps { textureData = Data(count: header.dwPitchOrLinearSize * header.dwHeight) }
            // Create a data buffer to hold all mipmap levels down to 1x1.
            else { textureData = Data(count: TextureUtils.calculateMipMappedTextureDataSize(Int(header.dwWidth), Int(header.dwHeight), bytesPerPixel)) }
            r.readRestOfBytes(textureData, 0)
        }
        else if header.ddspf.dwFourCC == "DXT1" {
            textureFormat = TextureFormat.ARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXT1ToARGB(compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else if header.ddspf.dwFourCC == "DXT3" {
            textureFormat = TextureFormat.ARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXT3ToARGB(compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else if header.ddspf.dwFourCC == "DXT5" {
            textureFormat = TextureFormat.ARGB8
            bytesPerPixel = 4
            let compressedTextureData = r.readRestOfBytes()
            textureData = decodeDXT5ToARGB(compressedData: compressedTextureData, width: Int(header.dwWidth), height: Int(header.dwHeight), pixelFormat: header.ddspf, mipmapCount: ddsMipmapLevelCount)
        }
        else { fatalError("Unsupported DDS file pixel format.") }
    }

    static func postProcessDDSTexture(width: Int, height: Int, bytesPerPixel: Int, hasMipmaps: Bool, ddsMipmapLevelCount: Int, data: [UInt8], bool flipVertically) {
        assert(width > 0 && height > 0 && bytesPerPixel > 0 && DDSMipmapLevelCount > 0 && data != nil)
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
                data.flip2DSubArrayVertically(offsetBy: mipMapLevelDataOffset, rows: mipMapLevelHeight, bytesPerRow: mipMapLevelWidth * bytesPerPixel)
            }
            // Break after optionally flipping the first mipmap level if the DDS texture doesn't have mipmaps.
            guard hasMipmaps else {
                break
            }
            // Generate the next mipmap level's data if the DDS file doesn't contain it.
            if mipMapLevelIndex + 1 >= ddsMipmapLevelCount {
                TextureUtils.downscale4Component32BitPixelsX2(data, mipMapLevelDataOffset, mipMapLevelHeight, mipMapLevelWidth, data, mipMapLevelDataOffset + mipMapDataSize)
            }
            // Switch to the next mipmap level.
            mipMapLevelIndex += 1
            mipMapLevelWidth = mipMapLevelWidth > 1 ? (mipMapLevelWidth / 2) : mipMapLevelWidth
            mipMapLevelHeight = mipMapLevelHeight > 1 ? (mipMapLevelHeight / 2) : mipMapLevelHeight
            mipMapLevelDataOffset += mipMapDataSize
        }
    }
}
