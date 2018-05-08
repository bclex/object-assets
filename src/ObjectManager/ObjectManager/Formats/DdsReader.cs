using OA.Core;
using System;
using System.IO;
using UnityEngine;

// https://msdn.microsoft.com/en-us/library/windows/desktop/bb943982(v=vs.85).aspx
namespace OA.Formats
{
    [Flags]
    public enum DDSFlags : uint //: DDSD
    {
        Caps = 0x1,             // Required in every .dds file.
        Height = 0x2,           // Required in every .dds file.
        Width = 0x4,            // Required in every .dds file.
        Pitch = 0x8,            // Required when pitch is provided for an uncompressed texture.
        PixelFormat = 0x1000,   // Required in every .dds file.
        MipmapCount = 0x20000,  // Required in a mipmapped texture.
        LinearSize = 0x80000,   // Required when pitch is provided for a compressed texture.
        Depth = 0x800000,       // Required in a depth texture.
        HEADER_FLAGS_TEXTURE = Caps | Height | Width | PixelFormat,
        HEADER_FLAGS_MIPMAP = MipmapCount,
        HEADER_FLAGS_VOLUME = Depth,
        HEADER_FLAGS_PITCH = Pitch,
        HEADER_FLAGS_LINEARSIZE = LinearSize,
    }

    [Flags]
    public enum DDSCaps : uint //: DDSCAPS
    {
        Complex = 0x8,          // Optional; must be used on any file that contains more than one surface (a mipmap, a cubic environment map, or mipmapped volume texture).
        Mipmap = 0x400000,      // Optional; should be used for a mipmap.
        Texture = 0x1000,       // Required
        SURFACE_FLAGS_MIPMAP = Complex | Mipmap,
        SURFACE_FLAGS_TEXTURE = Texture,
        SURFACE_FLAGS_CUBEMAP = Complex,
    }

    [Flags]
    public enum DDSCaps2 : uint //: DDSCAPS2
    {
        Cubemap = 0x200,            // Required for a cube map.
        CubemapPositiveX = 0x400,   // Required when these surfaces are stored in a cube map.	
        CubemapNegativeX = 0x800,   // Required when these surfaces are stored in a cube map.
        CubemapPositiveY = 0x1000,  // Required when these surfaces are stored in a cube map.
        CubemapNegativeY = 0x2000,  // Required when these surfaces are stored in a cube map.
        CubemapPositiveZ = 0x4000,  // Required when these surfaces are stored in a cube map.
        CubemapNegativeZ = 0x8000,  // Required when these surfaces are stored in a cube map.
        Volume = 0x200000,          // Required for a volume texture.
        CUBEMAP_POSITIVEX = Cubemap | CubemapPositiveX,
        CUBEMAP_NEGATIVEX = Cubemap | CubemapNegativeX,
        CUBEMAP_POSITIVEY = Cubemap | CubemapPositiveY,
        CUBEMAP_NEGATIVEY = Cubemap | CubemapNegativeY,
        CUBEMAP_POSITIVEZ = Cubemap | CubemapPositiveZ,
        CUBEMAP_NEGATIVEZ = Cubemap | CubemapNegativeZ,
        CUBEMAP_ALLFACES = CubemapPositiveX | CubemapNegativeX | CubemapPositiveY | CubemapNegativeY | CubemapPositiveZ | CubemapNegativeZ,
        FLAGS_VOLUME = Volume,
    }

    public struct DDSHeader //: DDS_HEADER
    {
        public uint dwSize => 124;
        public DDSFlags dwFlags;
        public uint dwHeight;
        public uint dwWidth;
        public uint dwPitchOrLinearSize;
        public uint dwDepth; // only if DDS_HEADER_FLAGS_VOLUME is set in flags
        public uint dwMipMapCount;
        public uint[] dwReserved1;
        public DDSPixelFormat ddspf;
        public DDSCaps dwCaps; // dwSurfaceFlags
        public DDSCaps2 dwCaps2; // dwCubemapFlags
        public uint dwCaps3;
        public uint dwCaps4;
        public uint dwReserved2;

        public void Read(UnityBinaryReader r)
        {
            var size = r.ReadLEUInt32();
            if (size != 124)
                throw new FileFormatException($"Invalid DDS file header size: {size}.");
            dwFlags = (DDSFlags)r.ReadLEUInt32();
            if (!Utils.ContainsBitFlags((uint)dwFlags, (uint)DDSFlags.Height, (uint)DDSFlags.Width))
                throw new FileFormatException($"Invalid DDS file flags: {dwFlags}.");
            dwHeight = r.ReadLEUInt32();
            dwWidth = r.ReadLEUInt32();
            dwPitchOrLinearSize = r.ReadLEUInt32();
            dwDepth = r.ReadLEUInt32();
            dwMipMapCount = r.ReadLEUInt32();
            dwReserved1 = new uint[11];
            for (var i = 0; i < dwReserved1.Length; i++)
                dwReserved1[i] = r.ReadLEUInt32();
            ddspf = new DDSPixelFormat();
            ddspf.Read(r);
            dwCaps = (DDSCaps)r.ReadLEUInt32();
            if (!Utils.ContainsBitFlags((uint)dwCaps, (uint)DDSCaps.Texture))
                throw new FileFormatException($"Invalid DDS file caps: {dwCaps}.");
            dwCaps2 = (DDSCaps2)r.ReadLEUInt32();
            dwCaps3 = r.ReadLEUInt32();
            dwCaps4 = r.ReadLEUInt32();
            dwReserved2 = r.ReadLEUInt32();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(dwSize);
            w.Write((uint)dwFlags);
            w.Write(dwHeight);
            w.Write(dwWidth);
            w.Write(dwPitchOrLinearSize);
            w.Write(dwDepth);
            w.Write(dwMipMapCount);
            for (var i = 0; i < 11; i++)
                w.Write(dwReserved1[i]);
            ddspf.Write(w);
            w.Write((uint)dwCaps);
            w.Write((uint)dwCaps2);
            w.Write(dwCaps3);
            w.Write(dwCaps4);
            w.Write(dwReserved2);
        }
    }

    [Flags]
    public enum DDSDimension : uint //: D3D10_RESOURCE_DIMENSION
    {
        Texture1D = 2, // Resource is a 1D texture. The dwWidth member of DDS_HEADER specifies the size of the texture. Typically, you set the dwHeight member of DDS_HEADER to 1; you also must set the DDSD_HEIGHT flag in the dwFlags member of DDS_HEADER.
        Texture2D = 3, // Resource is a 2D texture with an area specified by the dwWidth and dwHeight members of DDS_HEADER. You can also use this type to identify a cube-map texture. For more information about how to identify a cube-map texture, see miscFlag and arraySize members.
        Texture3D = 4, // Resource is a 3D texture with a volume specified by the dwWidth, dwHeight, and dwDepth members of DDS_HEADER. You also must set the DDSD_DEPTH flag in the dwFlags member of DDS_HEADER.
    }

    public struct DDSHeader_DXT10 //: DDS_HEADER_DXT10
    {
        public int dxgiFormat;
        public DDSDimension resourceDimension;
        public uint miscFlag; // see D3D11_RESOURCE_MISC_FLAG
        public uint arraySize;
        public uint miscFlags2; // see DDS_MISC_FLAGS2

        public void Write(BinaryWriter w)
        {
            w.Write(dxgiFormat);
            w.Write((uint)resourceDimension);
            w.Write(miscFlag);
            w.Write(arraySize);
            w.Write(miscFlags2);
        }
    }

    [Flags]
    public enum DDSPixelFormats : uint //: DDS_PIXELFORMAT
    {
        AlphaPixels = 0x1,      // Texture contains alpha data; dwRGBAlphaBitMask contains valid data.
        Alpha = 0x2,            // Used in some older DDS files for alpha channel only uncompressed data (dwRGBBitCount contains the alpha channel bitcount; dwABitMask contains valid data)
        FourCC = 0x4,           // Texture contains compressed RGB data; dwFourCC contains valid data.
        RGB = 0x40,             // Texture contains uncompressed RGB data; dwRGBBitCount and the RGB masks (dwRBitMask, dwGBitMask, dwBBitMask) contain valid data.
        YUV = 0x200,            // Used in some older DDS files for YUV uncompressed data (dwRGBBitCount contains the YUV bit count; dwRBitMask contains the Y mask, dwGBitMask contains the U mask, dwBBitMask contains the V mask)
        Luminance = 0x20000,    // Used in some older DDS files for single channel color uncompressed data (dwRGBBitCount contains the luminance channel bit count; dwRBitMask contains the channel mask). Can be combined with DDPF_ALPHAPIXELS for a two channel DDS file.
    }

    public struct DDSPixelFormat  //: DDS_PIXELFORMAT
    {
        public uint dwSize => 32;
        public DDSPixelFormats dwFlags;
        public byte[] dwFourCC;
        public uint dwRGBBitCount;
        public uint dwRBitMask;
        public uint dwGBitMask;
        public uint dwBBitMask;
        public uint dwABitMask;

        public void Read(UnityBinaryReader r)
        {
            var size = r.ReadLEUInt32();
            if (size != 32)
                throw new FileFormatException($"Invalid DDS file pixel format size: {size}.");
            dwFlags = (DDSPixelFormats)r.ReadLEUInt32();
            dwFourCC = r.ReadBytes(4);
            dwRGBBitCount = r.ReadLEUInt32();
            dwRBitMask = r.ReadLEUInt32();
            dwGBitMask = r.ReadLEUInt32();
            dwBBitMask = r.ReadLEUInt32();
            dwABitMask = r.ReadLEUInt32();
        }

        public void Write(BinaryWriter w)
        {
            w.Write(dwSize);
            w.Write((uint)dwFlags);
            w.Write(dwFourCC);
            w.Write(dwRGBBitCount);
            w.Write(dwRBitMask);
            w.Write(dwGBitMask);
            w.Write(dwBBitMask);
            w.Write(dwABitMask);
        }
    }

    // https://msdn.microsoft.com/en-us/library/windows/desktop/bb173059(v=vs.85).aspx
    public enum DXGIFormat : byte //: DXGI_FORMAT
    {
        DXGI_FORMAT_UNKNOWN = 0,
        DXGI_FORMAT_R8_UNORM = 61,
        BC1_UNORM = 71,
        BC2_UNORM = 74,
        BC3_UNORM = 77,
        BC5_UNORM = 83,
        DXGI_FORMAT_B8G8R8A8_UNORM = 87,
        BC7_UNORM = 98,
    }

    public static class DdsReader
    {
        /// <summary>
        /// Loads a DDS texture from a file.
        /// </summary>
        public static Texture2DInfo LoadDDSTexture(string filePath, bool flipVertically = false)
        {
            return LoadDDSTexture(File.Open(filePath, FileMode.Open, FileAccess.Read), flipVertically);
        }

        /// <summary>
        /// Loads a DDS texture from an input stream.
        /// </summary>
        public static Texture2DInfo LoadDDSTexture(Stream inputStream, bool flipVertically = false)
        {
            using (var r = new UnityBinaryReader(inputStream))
            {
                // Check the magic string.
                var magicString = r.ReadBytes(4);
                if (!StringUtils.Equals(magicString, "DDS "))
                    throw new FileFormatException("Invalid DDS file magic string: \"" + System.Text.Encoding.ASCII.GetString(magicString) + "\".");
                // Deserialize the DDS file header.
                var header = new DDSHeader();
                header.Read(r);
                // Figure out the texture format and load the texture data.
                ExtractDDSTextureFormatAndData(header, r, out bool hasMipmaps, out uint ddsMipmapLevelCount, out TextureFormat textureFormat, out int bytesPerPixel, out byte[] textureData);
                // Post-process the texture to generate missing mipmaps and possibly flip it vertically.
                PostProcessDDSTexture((int)header.dwWidth, (int)header.dwHeight, bytesPerPixel, hasMipmaps, (int)ddsMipmapLevelCount, textureData, flipVertically);
                return new Texture2DInfo((int)header.dwWidth, (int)header.dwHeight, textureFormat, hasMipmaps, textureData);
            }
        }

        /// <summary>
        /// Decodes a DXT1-compressed 4x4 block of texels using a prebuilt 4-color color table.
        /// </summary>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC1 </remarks>
        static Color32[] DecodeDXT1TexelBlock(UnityBinaryReader r, Color[] colorTable)
        {
            Debug.Assert(colorTable.Length == 4);
            // Read pixel color indices.
            var colorIndices = new uint[16];
            var colorIndexBytes = new byte[4];
            r.Read(colorIndexBytes, 0, colorIndexBytes.Length);
            const uint bitsPerColorIndex = 2;
            for (uint rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                var rowBaseColorIndexIndex = 4 * rowIndex;
                var rowBaseBitOffset = 8 * rowIndex;
                for (uint columnIndex = 0; columnIndex < 4; columnIndex++)
                {
                    // Color indices are arranged from right to left.
                    var bitOffset = rowBaseBitOffset + (bitsPerColorIndex * (3 - columnIndex));
                    colorIndices[rowBaseColorIndexIndex + columnIndex] = (uint)Utils.GetBits(bitOffset, bitsPerColorIndex, colorIndexBytes);
                }
            }
            // Calculate pixel colors.
            var colors = new Color32[16];
            for (var i = 0; i < 16; i++)
                colors[i] = colorTable[colorIndices[i]];
            return colors;
        }

        /// <summary>
        /// Builds a 4-color color table for a DXT1-compressed 4x4 block of texels and then decodes the texels.
        /// </summary>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC1 </remarks>
        static Color32[] DecodeDXT1TexelBlock(UnityBinaryReader r, bool containsAlpha)
        {
            // Create the color table.
            var colorTable = new Color[4];
            colorTable[0] = ColorUtils.R5G6B5ToColor(r.ReadLEUInt16());
            colorTable[1] = ColorUtils.R5G6B5ToColor(r.ReadLEUInt16());
            if (!containsAlpha)
            {
                colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
                colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);
            }
            else
            {
                colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 2);
                colorTable[3] = new Color(0, 0, 0, 0);
            }
            // Calculate pixel colors.
            return DecodeDXT1TexelBlock(r, colorTable);
        }

        /// <summary>
        /// Decodes a DXT3-compressed 4x4 block of texels.
        /// </summary>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC2 </remarks>
        static Color32[] DecodeDXT3TexelBlock(UnityBinaryReader r)
        {
            // Read compressed pixel alphas.
            var compressedAlphas = new byte[16];
            for (var rowIndex = 0; rowIndex < 4; rowIndex++)
            {
                var compressedAlphaRow = r.ReadLEUInt16();
                for (var columnIndex = 0; columnIndex < 4; columnIndex++)
                    // Each compressed alpha is 4 bits.
                    compressedAlphas[(4 * rowIndex) + columnIndex] = (byte)((compressedAlphaRow >> (columnIndex * 4)) & 0xF);
            }
            // Calculate pixel alphas.
            var alphas = new byte[16];
            for (var i = 0; i < 16; i++)
            {
                var alphaPercent = (float)compressedAlphas[i] / 15;
                alphas[i] = (byte)Mathf.RoundToInt(alphaPercent * 255);
            }
            // Create the color table.
            var colorTable = new Color[4];
            colorTable[0] = ColorUtils.R5G6B5ToColor(r.ReadLEUInt16());
            colorTable[1] = ColorUtils.R5G6B5ToColor(r.ReadLEUInt16());
            colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
            colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);
            // Calculate pixel colors.
            var colors = DecodeDXT1TexelBlock(r, colorTable);
            for (var i = 0; i < 16; i++)
                colors[i].a = alphas[i];
            return colors;
        }

        /// <summary>
        /// Decodes a DXT5-compressed 4x4 block of texels.
        /// </summary>
        /// <remarks>See https://msdn.microsoft.com/en-us/library/windows/desktop/bb694531(v=vs.85).aspx#BC3 </remarks>
        static Color32[] DecodeDXT5TexelBlock(UnityBinaryReader r)
        {
            // Create the alpha table.
            var alphaTable = new float[8];
            alphaTable[0] = r.ReadByte();
            alphaTable[1] = r.ReadByte();
            if (alphaTable[0] > alphaTable[1])
            {
                for (var i = 0; i < 6; i++)
                    alphaTable[2 + i] = Mathf.Lerp(alphaTable[0], alphaTable[1], (float)(1 + i) / 7);
            }
            else
            {
                for (var i = 0; i < 4; i++)
                    alphaTable[2 + i] = Mathf.Lerp(alphaTable[0], alphaTable[1], (float)(1 + i) / 5);
                alphaTable[6] = 0;
                alphaTable[7] = 255;
            }

            // Read pixel alpha indices.
            var alphaIndices = new uint[16];
            var alphaIndexBytesRow0 = new byte[3];
            r.Read(alphaIndexBytesRow0, 0, alphaIndexBytesRow0.Length);
            Array.Reverse(alphaIndexBytesRow0); // Take care of little-endianness.
            var alphaIndexBytesRow1 = new byte[3];
            r.Read(alphaIndexBytesRow1, 0, alphaIndexBytesRow1.Length);
            Array.Reverse(alphaIndexBytesRow1); // Take care of little-endianness.
            const uint bitsPerAlphaIndex = 3;
            alphaIndices[0] = (uint)Utils.GetBits(21, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[1] = (uint)Utils.GetBits(18, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[2] = (uint)Utils.GetBits(15, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[3] = (uint)Utils.GetBits(12, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[4] = (uint)Utils.GetBits(9, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[5] = (uint)Utils.GetBits(6, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[6] = (uint)Utils.GetBits(3, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[7] = (uint)Utils.GetBits(0, bitsPerAlphaIndex, alphaIndexBytesRow0);
            alphaIndices[8] = (uint)Utils.GetBits(21, bitsPerAlphaIndex, alphaIndexBytesRow1);
            alphaIndices[9] = (uint)Utils.GetBits(18, bitsPerAlphaIndex, alphaIndexBytesRow1);
            alphaIndices[10] = (uint)Utils.GetBits(15, bitsPerAlphaIndex, alphaIndexBytesRow1);
            alphaIndices[11] = (uint)Utils.GetBits(12, bitsPerAlphaIndex, alphaIndexBytesRow1);
            alphaIndices[12] = (uint)Utils.GetBits(9, bitsPerAlphaIndex, alphaIndexBytesRow1);
            alphaIndices[13] = (uint)Utils.GetBits(6, bitsPerAlphaIndex, alphaIndexBytesRow1);
            alphaIndices[14] = (uint)Utils.GetBits(3, bitsPerAlphaIndex, alphaIndexBytesRow1);
            alphaIndices[15] = (uint)Utils.GetBits(0, bitsPerAlphaIndex, alphaIndexBytesRow1);
            // Create the color table.
            var colorTable = new Color[4];
            colorTable[0] = ColorUtils.R5G6B5ToColor(r.ReadLEUInt16());
            colorTable[1] = ColorUtils.R5G6B5ToColor(r.ReadLEUInt16());
            colorTable[2] = Color.Lerp(colorTable[0], colorTable[1], 1.0f / 3);
            colorTable[3] = Color.Lerp(colorTable[0], colorTable[1], 2.0f / 3);
            // Calculate pixel colors.
            var colors = DecodeDXT1TexelBlock(r, colorTable);
            for (var i = 0; i < 16; i++)
                colors[i].a = (byte)Mathf.Round(alphaTable[alphaIndices[i]]);
            return colors;
        }

        /// <summary>
        /// Copies a decoded texel block to a texture's data buffer. Takes into account DDS mipmap padding.
        /// </summary>
        /// <param name="decodedTexels">The decoded DDS texels.</param>
        /// <param name="argb">The texture's data buffer.</param>
        /// <param name="baseARGBIndex">The desired offset into the texture's data buffer. Used for mipmaps.</param>
        /// <param name="baseRowIndex">The base row index in the texture where decoded texels are copied.</param>
        /// <param name="baseColumnIndex">The base column index in the texture where decoded texels are copied.</param>
        /// <param name="textureWidth">The width of the texture.</param>
        /// <param name="textureHeight">The height of the texture.</param>
        static void CopyDecodedTexelBlock(Color32[] decodedTexels, byte[] argb, int baseARGBIndex, int baseRowIndex, int baseColumnIndex, int textureWidth, int textureHeight)
        {
            for (var i = 0; i < 4; i++) // row
                for (var j = 0; j < 4; j++) // column
                {
                    var rowIndex = baseRowIndex + i;
                    var columnIndex = baseColumnIndex + j;
                    // Don't copy padding on mipmaps.
                    if (rowIndex < textureHeight && columnIndex < textureWidth)
                    {
                        var decodedTexelIndex = (4 * i) + j;
                        var color = decodedTexels[decodedTexelIndex];
                        var ARGBPixelOffset = (textureWidth * rowIndex) + columnIndex;
                        var basePixelARGBIndex = baseARGBIndex + (4 * ARGBPixelOffset);
                        argb[basePixelARGBIndex] = color.a;
                        argb[basePixelARGBIndex + 1] = color.r;
                        argb[basePixelARGBIndex + 2] = color.g;
                        argb[basePixelARGBIndex + 3] = color.b;
                    }
                }
        }

        /// <summary>
        /// Decodes DXT data to ARGB.
        /// </summary>
        static byte[] DecodeDXTToARGB(int DXTVersion, byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount)
        {
            var alphaFlag = Utils.ContainsBitFlags(pixelFormat.dwFlags, DDSPixelFormats.AlphaPixels);
            var containsAlpha = alphaFlag || ((pixelFormat.dwRGBBitCount == 32) && (pixelFormat.dwABitMask != 0));
            var reader = new UnityBinaryReader(new MemoryStream(compressedData));
            var argb = new byte[TextureUtils.CalculateMipMappedTextureDataSize((int)width, (int)height, 4)];
            var mipMapWidth = (int)width;
            var mipMapHeight = (int)height;
            var baseARGBIndex = 0;
            for (var mipMapIndex = 0; mipMapIndex < mipmapCount; mipMapIndex++)
            {
                for (var rowIndex = 0; rowIndex < mipMapHeight; rowIndex += 4)
                    for (var columnIndex = 0; columnIndex < mipMapWidth; columnIndex += 4)
                    {
                        Color32[] colors = null;
                        switch (DXTVersion) // Doing a switch instead of using a delegate for speed.
                        {
                            case 1: colors = DecodeDXT1TexelBlock(reader, containsAlpha); break;
                            case 3: colors = DecodeDXT3TexelBlock(reader); break;
                            case 5: colors = DecodeDXT5TexelBlock(reader); break;
                            default: throw new NotImplementedException("Tried decoding a DDS file using an unsupported DXT format: DXT" + DXTVersion.ToString());
                        }
                        CopyDecodedTexelBlock(colors, argb, baseARGBIndex, rowIndex, columnIndex, mipMapWidth, mipMapHeight);
                    }
                baseARGBIndex += (mipMapWidth * mipMapHeight * 4);
                mipMapWidth /= 2;
                mipMapHeight /= 2;
            }
            return argb;
        }

        static byte[] DecodeDXT1ToARGB(byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount) { return DecodeDXTToARGB(1, compressedData, width, height, pixelFormat, mipmapCount); }
        static byte[] DecodeDXT3ToARGB(byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount) { return DecodeDXTToARGB(3, compressedData, width, height, pixelFormat, mipmapCount); }
        static byte[] DecodeDXT5ToARGB(byte[] compressedData, uint width, uint height, DDSPixelFormat pixelFormat, uint mipmapCount) { return DecodeDXTToARGB(5, compressedData, width, height, pixelFormat, mipmapCount); }

        /// <summary>
        /// Extracts a DDS file's texture format and pixel data.
        /// </summary>
        static void ExtractDDSTextureFormatAndData(DDSHeader header, UnityBinaryReader r, out bool hasMipmaps, out uint DDSMipmapLevelCount, out TextureFormat textureFormat, out int bytesPerPixel, out byte[] textureData)
        {
            hasMipmaps = Utils.ContainsBitFlags(header.dwCaps, DDSCaps.Mipmap);
            // Non-mipmapped textures still have one mipmap level: the texture itself.
            DDSMipmapLevelCount = hasMipmaps ? header.dwMipMapCount : 1;
            // If the DDS file contains uncompressed data.
            if (Utils.ContainsBitFlags(header.ddspf.dwFlags, DDSPixelFormats.RGB))
            {
                // some permutation of RGB
                if (!Utils.ContainsBitFlags(header.ddspf.dwFlags, DDSPixelFormats.AlphaPixels))
                    throw new NotImplementedException("Unsupported DDS file pixel format.");
                // some permutation of RGBA
                else
                {
                    // There should be 32 bits per pixel.
                    if (header.ddspf.dwRGBBitCount != 32)
                        throw new FileFormatException("Invalid DDS file pixel format.");
                    // BGRA32
                    if (header.ddspf.dwBBitMask == 0x000000FF && header.ddspf.dwGBitMask == 0x0000FF00 && header.ddspf.dwRBitMask == 0x00FF0000 && header.ddspf.dwABitMask == 0xFF000000)
                    {
                        textureFormat = TextureFormat.BGRA32;
                        bytesPerPixel = 4;
                    }
                    // ARGB32
                    else if (header.ddspf.dwABitMask == 0x000000FF && header.ddspf.dwRBitMask == 0x0000FF00 && header.ddspf.dwGBitMask == 0x00FF0000 && header.ddspf.dwBBitMask == 0xFF000000)
                    {
                        textureFormat = TextureFormat.ARGB32;
                        bytesPerPixel = 4;
                    }
                    else throw new NotImplementedException("Unsupported DDS file pixel format.");

                    if (!hasMipmaps) textureData = new byte[header.dwPitchOrLinearSize * header.dwHeight];
                    // Create a data buffer to hold all mipmap levels down to 1x1.
                    else textureData = new byte[TextureUtils.CalculateMipMappedTextureDataSize((int)header.dwWidth, (int)header.dwHeight, bytesPerPixel)];
                    r.ReadRestOfBytes(textureData, 0);
                }
            }
            else if (StringUtils.Equals(header.ddspf.dwFourCC, "DXT1"))
            {
                textureFormat = TextureFormat.ARGB32;
                bytesPerPixel = 4;
                var compressedTextureData = r.ReadRestOfBytes();
                textureData = DecodeDXT1ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, header.ddspf, DDSMipmapLevelCount);
            }
            else if (StringUtils.Equals(header.ddspf.dwFourCC, "DXT3"))
            {
                textureFormat = TextureFormat.ARGB32;
                bytesPerPixel = 4;
                var compressedTextureData = r.ReadRestOfBytes();
                textureData = DecodeDXT3ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, header.ddspf, DDSMipmapLevelCount);
            }
            else if (StringUtils.Equals(header.ddspf.dwFourCC, "DXT5"))
            {
                textureFormat = TextureFormat.ARGB32;
                bytesPerPixel = 4;
                var compressedTextureData = r.ReadRestOfBytes();
                textureData = DecodeDXT5ToARGB(compressedTextureData, header.dwWidth, header.dwHeight, header.ddspf, DDSMipmapLevelCount);
            }
            else throw new NotImplementedException("Unsupported DDS file pixel format.");
        }

        /// <summary>
        /// Generates missing mipmap levels for a DDS texture and optionally flips it.
        /// </summary>
        /// <param name="width">The width of the texture.</param>
        /// <param name="height">The height of the texture.</param>
        /// <param name="bytesPerPixel">The number of bytes per pixel.</param>
        /// <param name="hasMipmaps">Does the DDS texture have mipmaps?</param>
        /// <param name="DDSMipmapLevelCount">The number of mipmap levels in the DDS file. 1 if the DDS file doesn't have mipmaps.</param>
        /// <param name="data">The texture's data.</param>
        /// <param name="flipVertically">Should the texture be flipped vertically?</param>
        static void PostProcessDDSTexture(int width, int height, int bytesPerPixel, bool hasMipmaps, int DDSMipmapLevelCount, byte[] data, bool flipVertically)
        {
            Debug.Assert(width > 0 && height > 0 && bytesPerPixel > 0 && DDSMipmapLevelCount > 0 && data != null);
            // Flip mip-maps if necessary and generate missing mip-map levels.
            var mipMapLevelWidth = width;
            var mipMapLevelHeight = height;
            var mipMapLevelIndex = 0;
            var mipMapLevelDataOffset = 0;
            // While we haven't processed all of the mipmap levels we should process.
            while (mipMapLevelWidth > 1 || mipMapLevelHeight > 1)
            {
                var mipMapDataSize = (mipMapLevelWidth * mipMapLevelHeight * bytesPerPixel);
                // If the DDS file contains the current mipmap level, flip it vertically if necessary.
                if (flipVertically && (mipMapLevelIndex < DDSMipmapLevelCount))
                    ArrayUtils.Flip2DSubArrayVertically(data, mipMapLevelDataOffset, mipMapLevelHeight, mipMapLevelWidth * bytesPerPixel);
                // Break after optionally flipping the first mipmap level if the DDS texture doesn't have mipmaps.
                if (!hasMipmaps)
                    break;
                // Generate the next mipmap level's data if the DDS file doesn't contain it.
                if (mipMapLevelIndex + 1 >= DDSMipmapLevelCount)
                    TextureUtils.Downscale4Component32BitPixelsX2(data, mipMapLevelDataOffset, mipMapLevelHeight, mipMapLevelWidth, data, mipMapLevelDataOffset + mipMapDataSize);
                // Switch to the next mipmap level.
                mipMapLevelIndex++;
                mipMapLevelWidth = mipMapLevelWidth > 1 ? (mipMapLevelWidth / 2) : mipMapLevelWidth;
                mipMapLevelHeight = mipMapLevelHeight > 1 ? (mipMapLevelHeight / 2) : mipMapLevelHeight;
                mipMapLevelDataOffset += mipMapDataSize;
            }
        }
    }
}