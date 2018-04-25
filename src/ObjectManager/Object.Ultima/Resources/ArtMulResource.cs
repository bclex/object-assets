using OA.Core;
using OA.Core.Diagnostics;
using OA.Ultima.Data;
using OA.Ultima.IO;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class ArtMulResource
    {
        readonly AFileIndex _fileIndex;
        readonly PixelPicking _staticPicking;
        Texture2DInfo[] _landTileTextureCache;
        Texture2DInfo[] _staticTileTextureCache;

        public ArtMulResource(object graphics)
        {
            _fileIndex = ClientVersion.InstallationIsUopFormat ?
                FileManager.CreateFileIndex("artLegacyMUL.uop", 0x10000, false, ".tga") :
                FileManager.CreateFileIndex("artidx.mul", "art.mul", 0x10000, -1); // !!! must find patch file reference for artdata.
            _staticPicking = new PixelPicking();
            _landTileTextureCache = new Texture2DInfo[0x10000];
            _staticTileTextureCache = new Texture2DInfo[0x10000];
        }

        public Texture2DInfo GetLandTexture(int index)
        {
            index &= FileManager.ItemIDMask;
            if (_landTileTextureCache[index] == null)
                _landTileTextureCache[index] = ReadLandTexture(index);
            return _landTileTextureCache[index];
        }

        public Texture2DInfo GetStaticTexture(int index)
        {
            index &= FileManager.ItemIDMask;
            if (_staticTileTextureCache[index] == null)
            {
                ReadStaticTexture(index + 0x4000, out Texture2DInfo texture);
                _staticTileTextureCache[index] = texture;
            }
            return _staticTileTextureCache[index];
        }

        public void GetStaticDimensions(int index, out int width, out int height)
        {
            index &= FileManager.ItemIDMask;
            if (_staticTileTextureCache[index] == null)
                GetStaticTexture(index);
            _staticPicking.GetDimensions(index + 0x4000, out width, out height);
        }

        public bool IsPointInItemTexture(int index, int x, int y, int extraRange = 0)
        {
            if (_staticTileTextureCache[index] == null)
                GetStaticTexture(index);
            return _staticPicking.Get(index + 0x4000, x, y, extraRange);
        }

        unsafe Texture2DInfo ReadLandTexture(int index)
        {
            var r = _fileIndex.Seek(index, out int length, out int extra, out bool patched);
            if (r == null)
                return null;
            var pixels = new byte[44 * 44 * 4];
            var fileData = r.ReadUShorts(23 * 44); // land tile textures store only opaque pixels
            Metrics.ReportDataRead(fileData.Length);
            var i = 0;
            fixed (byte* pData = pixels)
            {
                uint* dataRef = (uint*)pData;
                // fill the top half of the tile
                int count = 2;
                int offset = 21;
                for (int y = 0; y < 22; y++, count += 2, offset--, dataRef += 44)
                {
                    uint* start = dataRef + offset;
                    uint* end = start + count;
                    while (start < end)
                    {
                        uint color = ConvertUtils.FromBGR555(fileData[i++]);
                        *start++ = color;
                    }
                }
                // file the bottom half of the tile
                count = 44;
                offset = 0;
                for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                {
                    uint* start = dataRef + offset;
                    uint* end = start + count;
                    while (start < end)
                    {
                        uint color = ConvertUtils.FromBGR555(fileData[i++]);
                        *start++ = color;
                    }
                }
            }
            var texture = new Texture2DInfo(44, 44, TextureFormat.BGRA32, false, pixels);
            return texture;
        }

        unsafe void ReadStaticTexture(int index, out Texture2DInfo texture)
        {
            texture = null;
            // get a reader inside Art.Mul
            var r = _fileIndex.Seek(index, out int length, out int extra, out bool patched);
            if (r == null)
                return;
            r.ReadInt(); // don't need this, see Art.mul file format.
            // get the dimensions of the texture
            var width = r.ReadShort();
            var height = r.ReadShort();
            if (width <= 0 || height <= 0)
                return;
            // read the texture data!
            var lookups = r.ReadUShorts(height);
            var fileData = r.ReadUShorts(length - lookups.Length * 2 - 8);
            Metrics.ReportDataRead(sizeof(ushort) * (fileData.Length + lookups.Length + 2));
            var pixels = new byte[width * height * 4];
            fixed (byte* pData = pixels)
            {
                uint* dataRef = (uint*)pData;
                int i;
                for (int y = 0; y < height; y++, dataRef += width)
                {
                    i = lookups[y];
                    uint* start = dataRef;
                    int count, offset;
                    while (((offset = fileData[i++]) + (count = fileData[i++])) != 0)
                    {
                        start += offset;
                        uint* end = start + count;
                        while (start < end)
                        {
                            uint color = ConvertUtils.FromBGR555(fileData[i++]);
                            *start++ = color;
                        }
                    }
                }
            }
            texture = new Texture2DInfo(width, height, TextureFormat.BGRA32, false, pixels);
            _staticPicking.Set(index, width, height, pixels);
            return;
        }
    }
}
