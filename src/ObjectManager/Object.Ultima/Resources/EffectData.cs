using System;
using System.IO;

namespace OA.Ultima.Resources
{
    public struct EffectData
    {
        readonly byte _unknown;

        public readonly sbyte[] Frames;
        public readonly byte FrameCount;
        public readonly byte FrameInterval;
        public readonly byte StartInterval;

        public EffectData(BinaryReader reader)
        {
            var data = reader.ReadBytes(0x40);
            Frames = Array.ConvertAll(data, b => unchecked((sbyte)b));
            _unknown = reader.ReadByte();
            FrameCount = reader.ReadByte();
            if (FrameCount == 0)
            {
                FrameCount = 1;
                Frames[0] = 0;
            }
            FrameInterval = reader.ReadByte();
            if (FrameInterval == 0)
                FrameInterval = 1;
            StartInterval = reader.ReadByte();
        }
    }
}
