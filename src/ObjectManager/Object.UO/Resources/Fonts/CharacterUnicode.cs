using OA.Ultima.Core.UI.Fonts;
using System;
using System.IO;

namespace OA.Ultima.Resources.Fonts
{
    class CharacterUnicode : ACharacter
    {
        public CharacterUnicode() { }
        public CharacterUnicode(BinaryReader reader)
        {
            XOffset = reader.ReadSByte();
            YOffset = reader.ReadSByte();
            Width = reader.ReadByte();
            Height = reader.ReadByte();
            ExtraWidth = 1;

            // only read data if there is IO...
            if (Width > 0 && Height > 0)
            {
                _pixelData = new uint[Width * Height];
                // At this point, we know we have data, so go ahead and start reading!
                for (var y = 0; y < Height; ++y)
                {
                    var scanline = reader.ReadBytes(((Width - 1) / 8) + 1);
                    var bitX = 7;
                    var byteX = 0;
                    for (var x = 0; x < Width; ++x)
                    {
                        uint color = 0x00000000;
                        if ((scanline[byteX] & (byte)Math.Pow(2, bitX)) != 0)
                            color = 0xFFFFFFFF;
                        _pixelData[y * Width + x] = color;
                        bitX--;
                        if (bitX < 0)
                        {
                            bitX = 7;
                            byteX++;
                        }
                    }
                }
            }
        }
    }
}
