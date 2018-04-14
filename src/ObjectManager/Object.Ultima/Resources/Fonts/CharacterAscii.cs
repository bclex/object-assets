using OA.Ultima.Core.UI;
using System.IO;

namespace OA.Ultima.Resources.Fonts
{
    class CharacterAscii : ACharacter
    {
        public CharacterAscii() { }
        public CharacterAscii(BinaryReader reader)
        {
            Width = reader.ReadByte();
            Height = reader.ReadByte();
            HuePassedColor = true;
            reader.ReadByte(); // byte delimeter?
            var startY = Height;
            var endY = -1;
            uint[] pixels = null;
            if (Width > 0 && Height > 0)
            {
                pixels = new uint[Width * Height];
                var i = 0;
                for (var y = 0; y < Height; y++)
                {
                    var rowHasData = false;
                    for (var x = 0; x < Width; x++)
                    {
                        var pixel = (ushort)(reader.ReadByte() | (reader.ReadByte() << 8));
                        if (pixel != 0)
                        {
                            //if (pixel == 0x4e73) // off-grey color, normalize to white
                            //    pixel = 0xffff;
                            pixels[i] = (uint)(0xFF000000 + (
                                ((((pixel >> 10) & 0x1F) * 0xFF / 0x1F)) |
                                ((((pixel >> 5) & 0x1F) * 0xFF / 0x1F) << 8) |
                                (((pixel & 0x1F) * 0xFF / 0x1F) << 16)
                                ));
                            rowHasData = true;
                        }
                        i++;
                    }
                    if (rowHasData)
                    {
                        if (startY > y)
                            startY = y;
                        endY = y;
                    }
                }
            }

            endY += 1;
            if (endY == 0)
                _pixelData = null;
            else if (endY == Height)
                _pixelData = pixels;
            else
            {
                _pixelData = new uint[Width * endY];
                var i = 0;
                for (var y = 0; y < endY; y++)
                    for (var x = 0; x < Width; x++)
                        _pixelData[i++] = pixels[y * Width + x];
                YOffset = Height - endY;
                Height = endY;
            }
        }
    }
}
