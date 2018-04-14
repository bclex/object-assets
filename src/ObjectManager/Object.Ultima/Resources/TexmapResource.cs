using OA.Core;
using OA.Core.Diagnostics;
using OA.Ultima.IO;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class TexmapResource
    {
        Texture2D[] _cache = new Texture2D[0x4000];
        readonly AFileIndex _index = FileManager.CreateFileIndex("texidx.mul", "texmaps.mul", 0x4000, -1); // !!! must find patch file reference for texmap.
        readonly object _graphics;

        const int DEFAULT_TEXTURE = 0x007F; // index 127 is the first 'unused' texture.

        public TexmapResource(object graphics)
        {
            _graphics = graphics;
        }

        public Texture2D GetTexmapTexture(int index)
        {
            index &= 0x3FFF;
            if (_cache[index] == null)
            {
                _cache[index] = readTexmapTexture(index);
                if (_cache[index] == null)
                    _cache[index] = GetTexmapTexture(127);
            }
            return _cache[index];
        }

        private unsafe Texture2D readTexmapTexture(int index)
        {
            int length, extra;
            bool is_patched;
            var reader = _index.Seek(index, out length, out extra, out is_patched);
            if (reader == null)
                return null;
            if (reader.Stream.Length == 0)
            {
                Utils.Warning($"Requested texmap texture #{index} does not exist. Replacing with 'unused' graphic.");
                return GetTexmapTexture(DEFAULT_TEXTURE);
            }
            var metrics_dataread_start = (int)reader.Position;
            var textureSize = (extra == 0) ? 64 : 128;
            var pixelData = new ushort[textureSize * textureSize];
            var fileData = reader.ReadUShorts(textureSize * textureSize);
            fixed (ushort* pData = pixelData)
            {
                ushort* pDataRef = pData;
                int count = 0;
                int max = textureSize * textureSize;
                while (count < max)
                {
                    ushort color = (ushort)(fileData[count] | 0x8000);
                    *pDataRef++ = color;
                    count++;
                }
            }
            var texture = new Texture2D(textureSize, textureSize, TextureFormat.Alpha8, false); //: SurfaceFormat.Bgra5551
            //texture.SetData(pixelData);
            Metrics.ReportDataRead((int)reader.Position - metrics_dataread_start);
            return texture;
        }
    }
}