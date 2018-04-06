using System;

namespace OA.Ultima.Core.Network
{
    class PacketChunk
    {
        readonly byte[] _buffer;
        int _length;

        public PacketChunk(byte[] buffer)
        {
            _buffer = buffer;
        }

        public int Length
        {
            get { return _length; }
        }

        public void Write(byte[] source, int offset, int length)
        {
            Buffer.BlockCopy(source, offset, _buffer, _length, length);
            _length += length;
        }

        public void Prepend(byte[] dest, int length)
        {
            // Offset the intial buffer by the amount we need to prepend
            if (length > 0)
                Buffer.BlockCopy(dest, 0, dest, _length, length);
            // Prepend the buffer to the destination buffer
            Buffer.BlockCopy(_buffer, 0, dest, 0, _length);
        }

        public void Clear()
        {
            _length = 0;
        }
    }
}
