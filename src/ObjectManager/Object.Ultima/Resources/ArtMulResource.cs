using OA.Core.Diagnostics;
using OA.Ultima.Data;
using OA.Ultima.IO;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class ArtMulResource
    {
        readonly object _graphics;
        readonly AFileIndex _fileIndex;
        readonly PixelPicking _staticPicking;
        Texture2D[] _landTileTextureCache;
        Texture2D[] _staticTileTextureCache;

        public ArtMulResource(object graphics)
        {
            _graphics = graphics;
            _fileIndex = ClientVersion.InstallationIsUopFormat ? 
                FileManager.CreateFileIndex("artLegacyMUL.uop", 0x10000, false, ".tga") : 
                FileManager.CreateFileIndex("artidx.mul", "art.mul", 0x10000, -1); // !!! must find patch file reference for artdata.
            _staticPicking = new PixelPicking();
            _landTileTextureCache = new Texture2D[0x10000];
            _staticTileTextureCache = new Texture2D[0x10000];
        }

        public Texture2D GetLandTexture(int index)
        {
            index &= FileManager.ItemIDMask;
            if (_landTileTextureCache[index] == null)
                _landTileTextureCache[index] = ReadLandTexture(index);
            return _landTileTextureCache[index];
        }

        public Texture2D GetStaticTexture(int index)
        {
            index &= FileManager.ItemIDMask;
            if (_staticTileTextureCache[index] == null)
            {
                Texture2D texture;
                ReadStaticTexture(index + 0x4000, out texture);
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

        unsafe Texture2D ReadLandTexture(int index)
        {
            int length, extra;
            bool is_patched;
            var reader = _fileIndex.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return null;
            var pixels = new ushort[44 * 44];
            var data = reader.ReadUShorts(23 * 44); // land tile textures store only opaque pixels
            Metrics.ReportDataRead(data.Length);
            int i = 0;
            fixed (ushort* pData = pixels)
            {
                ushort* dataRef = pData;
                // fill the top half of the tile
                int count = 2;
                int offset = 21;
                for (int y = 0; y < 22; y++, count += 2, offset--, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;
                    while (start < end)
                    {
                        ushort color = data[i++];
                        *start++ = (ushort)(color | 0x8000);
                    }
                }
                // file the bottom half of the tile
                count = 44;
                offset = 0;
                for (int y = 0; y < 22; y++, count -= 2, offset++, dataRef += 44)
                {
                    ushort* start = dataRef + offset;
                    ushort* end = start + count;
                    while (start < end)
                    {
                        ushort color = data[i++];
                        *start++ = (ushort)(color | 0x8000);
                    }
                }
            }
            var texture = new Texture2D(44, 44, TextureFormat.Alpha8, false); // SurfaceFormat.Bgra5551
            //texture.LoadRawTextureData(pixels);
            texture.Apply();
            return texture;
        }

        unsafe void ReadStaticTexture(int index, out Texture2D texture)
        {
            texture = null;
            int length, extra;
            bool is_patched;
            // get a reader inside Art.Mul
            var reader = _fileIndex.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return;
            reader.ReadInt(); // don't need this, see Art.mul file format.
            // get the dimensions of the texture
            var width = reader.ReadShort();
            var height = reader.ReadShort();
            if (width <= 0 || height <= 0)
                return;
            // read the texture data!
            var lookups = reader.ReadUShorts(height);
            var data = reader.ReadUShorts(length - lookups.Length * 2 - 8);
            Metrics.ReportDataRead(sizeof(ushort) * (data.Length + lookups.Length + 2));
            var pixels = new ushort[width * height];
            fixed (ushort* pData = pixels)
            {
                ushort* dataRef = pData;
                int i;
                for (int y = 0; y < height; y++, dataRef += width)
                {
                    i = lookups[y];
                    ushort* start = dataRef;
                    int count, offset;
                    while (((offset = data[i++]) + (count = data[i++])) != 0)
                    {
                        start += offset;
                        ushort* end = start + count;
                        while (start < end)
                        {
                            ushort color = data[i++];
                            *start++ = (ushort)(color | 0x8000);
                        }
                    }
                }
            }
            texture = new Texture2D(width, height, TextureFormat.Alpha8, false); //: SurfaceFormat.Bgra5551
            //texture.LoadRawTextureData(pixels);
            texture.Apply();
            _staticPicking.Set(index, width, height, pixels);
            return;
        }
    }
}
