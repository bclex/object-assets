using System;
using System.IO;
using System.Net;
using System.Text;

namespace OA.Ultima.Core.IO
{
    public class BinaryFileWriter : GenericWriter
    {
        const int BufferSize = 64 * 1024;
        const int LargeByteBufferSize = 256;

        readonly bool _prefixStrings;
        readonly byte[] _buffer;
        readonly Encoding _encoding;
        readonly Stream _file;
        readonly char[] _singleCharBuffer = new char[1];
        byte[] _characterBuffer;
        int _index;
        int _maxBufferChars;
        long _position;

        public BinaryFileWriter(Stream strm, bool prefixStr)
        {
            _prefixStrings = prefixStr;
            _encoding = Utility.UTF8;
            _buffer = new byte[BufferSize];
            _file = strm;
        }

        public BinaryFileWriter(string filename, bool prefixStr)
        {
            _prefixStrings = prefixStr;
            _buffer = new byte[BufferSize];
            _file = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            _encoding = Utility.UTF8WithEncoding;
        }

        public override long Position => _position + _index;

        public Stream UnderlyingStream
        {
            get
            {
                if (_index > 0)
                    Flush();
                return _file;
            }
        }

        public void Flush()
        {
            if (_index > 0)
            {
                _position += _index;
                _file.Write(_buffer, 0, _index);
                _index = 0;
            }
        }

        public override void Close()
        {
            if (_index > 0)
                Flush();
            _file.Close();
        }

        public override void WriteEncodedInt(int value)
        {
            var v = (uint)value;
            while (v >= 0x80)
            {
                if ((_index + 1) > _buffer.Length)
                    Flush();
                _buffer[_index++] = (byte)(v | 0x80);
                v >>= 7;
            }
            if ((_index + 1) > _buffer.Length)
                Flush();
            _buffer[_index++] = (byte)v;
        }

        internal void InternalWriteString(string value)
        {
            if (value == null)
                value = string.Empty;
            var length = _encoding.GetByteCount(value);
            WriteEncodedInt(length);
            if (_characterBuffer == null)
            {
                _characterBuffer = new byte[LargeByteBufferSize];
                _maxBufferChars = LargeByteBufferSize / _encoding.GetMaxByteCount(1);
            }
            if (length > LargeByteBufferSize)
            {
                var current = 0;
                var charsLeft = value.Length;
                while (charsLeft > 0)
                {
                    var charCount = (charsLeft > _maxBufferChars) ? _maxBufferChars : charsLeft;
                    var byteLength = _encoding.GetBytes(value, current, charCount, _characterBuffer, 0);
                    if ((_index + byteLength) > _buffer.Length)
                        Flush();
                    Buffer.BlockCopy(_characterBuffer, 0, _buffer, _index, byteLength);
                    _index += byteLength;
                    current += charCount;
                    charsLeft -= charCount;
                }
            }
            else
            {
                var byteLength = _encoding.GetBytes(value, 0, value.Length, _characterBuffer, 0);
                if ((_index + byteLength) > _buffer.Length)
                    Flush();
                Buffer.BlockCopy(_characterBuffer, 0, _buffer, _index, byteLength);
                _index += byteLength;
            }
        }

        public override void Write(string value)
        {
            if (_prefixStrings)
            {
                if (value == null)
                {
                    if ((_index + 1) > _buffer.Length)
                        Flush();
                    _buffer[_index++] = 0;
                }
                else
                {
                    if ((_index + 1) > _buffer.Length)
                        Flush();
                    _buffer[_index++] = 1;
                    InternalWriteString(value);
                }
            }
            else InternalWriteString(value);
        }

        public override void Write(DateTime value)
        {
            Write(value.Ticks);
        }

        public override void WriteDeltaTime(DateTime value)
        {
            var ticks = value.Ticks;
            var now = DateTime.Now.Ticks;
            TimeSpan d;
            try
            {
                d = new TimeSpan(ticks - now);
            }
            catch
            {
                if (ticks < now) d = TimeSpan.MaxValue;
                else d = TimeSpan.MaxValue;
            }
            Write(d);
        }

        public override void Write(IPAddress value)
        {
            Write(Utility.GetLongAddressValue(value));
        }

        public override void Write(TimeSpan value)
        {
            Write(value.Ticks);
        }

        public override void Write(decimal value)
        {
            var bits = Decimal.GetBits(value);
            for (var i = 0; i < bits.Length; ++i)
                Write(bits[i]);
        }

        public override void Write(long value)
        {
            if ((_index + 8) > _buffer.Length)
                Flush();
            _buffer[_index] = (byte)value;
            _buffer[_index + 1] = (byte)(value >> 8);
            _buffer[_index + 2] = (byte)(value >> 16);
            _buffer[_index + 3] = (byte)(value >> 24);
            _buffer[_index + 4] = (byte)(value >> 32);
            _buffer[_index + 5] = (byte)(value >> 40);
            _buffer[_index + 6] = (byte)(value >> 48);
            _buffer[_index + 7] = (byte)(value >> 56);
            _index += 8;
        }

        public override void Write(ulong value)
        {
            if ((_index + 8) > _buffer.Length)
                Flush();
            _buffer[_index] = (byte)value;
            _buffer[_index + 1] = (byte)(value >> 8);
            _buffer[_index + 2] = (byte)(value >> 16);
            _buffer[_index + 3] = (byte)(value >> 24);
            _buffer[_index + 4] = (byte)(value >> 32);
            _buffer[_index + 5] = (byte)(value >> 40);
            _buffer[_index + 6] = (byte)(value >> 48);
            _buffer[_index + 7] = (byte)(value >> 56);
            _index += 8;
        }

        public override void Write(int value)
        {
            if ((_index + 4) > _buffer.Length)
                Flush();
            _buffer[_index] = (byte)value;
            _buffer[_index + 1] = (byte)(value >> 8);
            _buffer[_index + 2] = (byte)(value >> 16);
            _buffer[_index + 3] = (byte)(value >> 24);
            _index += 4;
        }

        public override void Write(uint value)
        {
            if ((_index + 4) > _buffer.Length)
                Flush();
            _buffer[_index] = (byte)value;
            _buffer[_index + 1] = (byte)(value >> 8);
            _buffer[_index + 2] = (byte)(value >> 16);
            _buffer[_index + 3] = (byte)(value >> 24);
            _index += 4;
        }

        public override void Write(short value)
        {
            if ((_index + 2) > _buffer.Length)
                Flush();
            _buffer[_index] = (byte)value;
            _buffer[_index + 1] = (byte)(value >> 8);
            _index += 2;
        }

        public override void Write(ushort value)
        {
            if ((_index + 2) > _buffer.Length)
                Flush();
            _buffer[_index] = (byte)value;
            _buffer[_index + 1] = (byte)(value >> 8);
            _index += 2;
        }

        public override unsafe void Write(double value)
        {
            if ((_index + 8) > _buffer.Length)
                Flush();
            fixed (byte* pBuffer = _buffer)
            {
                *((double*)(pBuffer + _index)) = value;
            }
            _index += 8;
        }

        public override unsafe void Write(float value)
        {
            if ((_index + 4) > _buffer.Length)
                Flush();
            fixed (byte* pBuffer = _buffer)
            {
                *((float*)(pBuffer + _index)) = value;
            }
            _index += 4;
        }

        public override void Write(char value)
        {
            if ((_index + 8) > _buffer.Length)
                Flush();
            _singleCharBuffer[0] = value;
            int byteCount = _encoding.GetBytes(_singleCharBuffer, 0, 1, _buffer, _index);
            _index += byteCount;
        }

        public override void Write(byte value)
        {
            if ((_index + 1) > _buffer.Length)
                Flush();
            _buffer[_index++] = value;
        }

        public override void Write(byte[] value)
        {
            for (int i = 0; i < value.Length; i++)
                Write(value[i]);
        }

        public override void Write(sbyte value)
        {
            if ((_index + 1) > _buffer.Length)
                Flush();
            _buffer[_index++] = (byte)value;
        }

        public override void Write(bool value)
        {
            if ((_index + 1) > _buffer.Length)
                Flush();
            _buffer[_index++] = (byte)(value ? 1 : 0);
        }
    }
}