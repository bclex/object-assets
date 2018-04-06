using OA.Core;
using OA.Core.Diagnostics;
using OA.Ultima.Data;
using OA.Ultima.IO;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class GumpMulResource
    {
        AFileIndex _fileIndex = ClientVersion.InstallationIsUopFormat ?
            FileManager.CreateFileIndex("gumpartLegacyMUL.uop", 0xFFFF, true, ".tga") :
            FileManager.CreateFileIndex("Gumpidx.mul", "Gumpart.mul", 0x10000, 12);
        object _graphicsDevice;
        readonly PixelPicking _picking = new PixelPicking();
        Texture2D[] _textureCache = new Texture2D[0x10000];

        public AFileIndex FileIndex => _fileIndex;

        public GumpMulResource(object graphics)
        {
            _graphicsDevice = graphics;
        }

        public unsafe Texture2D GetGumpTexture(int textureID, bool replaceMask080808 = false)
        {
            if (textureID < 0)
                return null;
            if (_textureCache[textureID] == null)
            {
                int length, extra;
                bool patched;
                var reader = _fileIndex.Seek(textureID, out length, out extra, out patched);
                if (reader == null)
                    return null;
                var width = (extra >> 16) & 0xFFFF;
                var height = extra & 0xFFFF;
                if (width == 0 || height == 0)
                    return null;
                var shortsToRead = length - (height * 2);
                if (reader.Stream.Length - reader.Position < (shortsToRead * 2))
                {
                    Utils.Error($"Could not read gump {textureID:X4}: not enough data. Gump texture file truncated?");
                    return null;
                }
                var lookups = reader.ReadInts(height);
                var metrics_dataread_start = (int)reader.Position;
                var fileData = reader.ReadUShorts(shortsToRead);
                var pixels = new ushort[width * height];
                fixed (ushort* line = &pixels[0])
                {
                    fixed (ushort* data = &fileData[0])
                    {
                        for (int y = 0; y < height; ++y)
                        {
                            ushort* dataRef = data + (lookups[y] - height) * 2;
                            ushort* cur = line + (y * width);
                            ushort* end = cur + width;
                            while (cur < end)
                            {
                                ushort color = *dataRef++;
                                ushort* next = cur + *dataRef++;
                                if (color == 0)
                                    cur = next;
                                else
                                {
                                    color |= 0x8000;
                                    while (cur < next)
                                        *cur++ = color;
                                }
                            }
                        }
                    }
                }
                Metrics.ReportDataRead(length);
                if (replaceMask080808)
                    for (var i = 0; i < pixels.Length; i++)
                        if (pixels[i] == 0x8421)
                            pixels[i] = 0xFC1F;
                var texture = new Texture2D(width, height, TextureFormat.Alpha8, false); //: SurfaceFormat.Bgra5551
                //texture.SetData(pixels);
                _textureCache[textureID] = texture;
                _picking.Set(textureID, width, height, pixels);
            }
            return _textureCache[textureID];
        }

        public bool IsPointInGumpTexture(int textureID, int x, int y)
        {
            return _picking.Get(textureID, x, y);
        }
    }
}