using OA.Core;
using OA.Core.Diagnostics;
using OA.Ultima.Core.IO;
using UnityEngine;

namespace OA.Ultima.Resources
{
    public class AnimationFrame : AAnimationFrame
    {
        const int DoubleXor = (0x200 << 22) | (0x200 << 12);

        readonly int _animationIndex;

        public override bool IsPointInTexture(int x, int y) => _picking.Get(_animationIndex, x, y);

        public static readonly AnimationFrame NullFrame = new AnimationFrame();
        public static readonly AnimationFrame[] NullFrames = { NullFrame };
        static PixelPicking _picking = new PixelPicking();

        AnimationFrame()
        {
            var provider = Service.Get<IResourceProvider>();
            Texture = provider.GetItemTexture(1);
            Center = new Vector2Int(0, 0);
        }

        public unsafe AnimationFrame(int uniqueAnimationIndex, object graphics, ushort[] palette, BinaryFileReader reader, SittingTransformation sitting)
        {
            _animationIndex = uniqueAnimationIndex;
            int xCenter = reader.ReadShort();
            int yCenter = reader.ReadShort();
            int width = reader.ReadUShort();
            int height = reader.ReadUShort();
            // Fix for animations with no pixels.
            if (width == 0 || height == 0)
            {
                Texture = null;
                return;
            }
            if (sitting == SittingTransformation.StandSouth)
            {
                xCenter += 8;
                width += 8;
                height += 4;
            }
            var data = new ushort[width * height];
            // for sitting:
            // somewhere around the waist of a typical mobile animation, we take twelve rows of pixels,
            // discard every third, and shift every remaining row (total of eight) one pixel to the left
            // or right (depending on orientation), for a total skew of eight pixels.
            fixed (ushort* pData = data)
            {
                ushort* dataRef = (ushort*)pData;
                var dataRead = 0;
                int header;
                while ((header = reader.ReadInt()) != 0x7FFF7FFF)
                {
                    header ^= DoubleXor;
                    var x = ((header >> 22) & 0x3FF) + xCenter - 0x200;
                    var y = ((header >> 12) & 0x3FF) + yCenter + height - 0x200;
                    if (sitting == SittingTransformation.StandSouth)
                    {
                        const int skew_start = -17;
                        const int skew_end = skew_start - 16;
                        var iy = y - height - yCenter;
                        if (iy > skew_start)
                        {
                            // pixels below the skew
                            x -= 8;
                            y -= 4;
                        }
                        else if (iy > skew_end)
                        {
                            // pixels within the skew
                            if ((iy - skew_end) % 4 == 0)
                            {
                                reader.Position += (header & 0xFFF);
                                continue;
                            }
                            x -= (iy - skew_end) / 2;
                            y -= (iy - skew_end) / 4;
                        }
                    }
                    ushort* cur = dataRef + y * width + x;
                    ushort* end = cur + (header & 0xFFF);
                    var filecounter = 0;
                    var filedata = reader.ReadBytes(header & 0xFFF);
                    while (cur < end)
                        *cur++ = palette[filedata[filecounter++]];
                    dataRead += header & 0xFFF;
                }
                Metrics.ReportDataRead(dataRead);
            }
            Center = new Vector2Int(xCenter, yCenter);
            Texture = new Texture2D(width, height, TextureFormat.Alpha8, false); // SurfaceFormat.Bgra5551
            //Texture.LoadRawTextureData(data);
            Texture.Apply();
            _picking.Set(_animationIndex, width, height, data);
        }

        public enum SittingTransformation
        {
            None,
            StandSouth,
            MountNorth
        }
    }
}
