using System.Collections.Generic;

namespace OA.Ultima.Resources
{
    class PixelPicking
    {
        const int InitialDataCount = 0x40000; // 256kb

        Dictionary<int, int> _ids = new Dictionary<int, int>();
        readonly List<byte> _data = new List<byte>(InitialDataCount); // list<t> access is 10% slower than t[].

        public bool Get(int textureID, int x, int y, int extraRange = 0)
        {
            int index;
            if (!_ids.TryGetValue(textureID, out index))
                return false;
            var width = ReadIntegerFromData(ref index);
            if (x < 0 || x >= width)
                return false;
            var height = ReadIntegerFromData(ref index);
            if (y < 0 || y >= height)
                return false;
            var current = 0;
            var target = x + y * width;
            var inTransparentSpan = true;
            while (current < target)
            {
                var spanLength = ReadIntegerFromData(ref index);
                current += spanLength;
                if (extraRange == 0)
                {
                    if (target < current)
                        return !inTransparentSpan;
                }
                else
                {
                    if (!inTransparentSpan)
                    {
                        var y0 = current / width;
                        var x1 = current % width;
                        var x0 = x1 - spanLength;
                        for (var range = -extraRange; range <= extraRange; range++)
                            if (y + range == y0 && (x + extraRange >= x0) && (x - extraRange <= x1))
                                return true;
                    }
                }
                inTransparentSpan = !inTransparentSpan;
            }
            return false;
        }

        public void GetDimensions(int textureID, out int width, out int height)
        {
            int index;
            if (!_ids.TryGetValue(textureID, out index))
            {
                width = height = 0;
                return;
            }
            width = ReadIntegerFromData(ref index);
            height = ReadIntegerFromData(ref index);
        }

        public void Set(int textureID, int width, int height, ushort[] pixels)
        {
            var begin = _data.Count;
            WriteIntegerToData(width);
            WriteIntegerToData(height);
            var countingTransparent = true;
            var count = 0;
            for (var i = 0; i < pixels.Length; i++)
            {
                var isTransparent = pixels[i] == 0;
                if (countingTransparent != isTransparent)
                {
                    WriteIntegerToData(count);
                    countingTransparent = !countingTransparent;
                    count = 0;
                }
                count += 1;
            }
            WriteIntegerToData(count);
            _ids[textureID] = begin;
        }

        bool Has(int textureID)
        {
            return _ids.ContainsKey(textureID);
        }

        void WriteIntegerToData(int value)
        {
            while (value > 0x7f)
            {
                _data.Add((byte)((value & 0x7f) | 0x80));
                value >>= 7;
            }
            _data.Add((byte)value);
        }

        int ReadIntegerFromData(ref int index)
        {
            var value = 0;
            var shift = 0;
            while (true)
            {
                var data = _data[index++];
                value += (data & 0x7f) << shift;
                if ((data & 0x80) == 0x00)
                    return value;
                shift += 7;
            }
        }
    }
}
