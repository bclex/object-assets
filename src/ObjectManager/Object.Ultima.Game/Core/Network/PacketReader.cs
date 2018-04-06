using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace OA.Ultima.Core.Network
{
    public class PacketReader : IDisposable
    {
        static Stack<PacketReader> _pool = new Stack<PacketReader>();
        byte[] _buffer;
        int _length;
        int _index;

        public static PacketReader CreateInstance(byte[] buffer, int length, bool fixedSize)
        {
            PacketReader reader = null;
            lock (_pool)
            {
                if (_pool.Count > 0)
                {
                    reader = _pool.Pop();
                    if (reader != null)
                    {
                        reader._buffer = buffer;
                        reader._length = length;
                        reader._index = fixedSize ? 1 : 3;
                    }
                }
            }
            if (reader == null)
                reader = new PacketReader(buffer, length, fixedSize);
            return reader;
        }

        public static void ReleaseInstance(PacketReader reader)
        {
            lock (_pool)
            {
                if (!_pool.Contains(reader))
                    _pool.Push(reader);
                else
                {
                    ////log.Warn("Instance pool already contains reader");
                }
            }
        }

        public int Index
        {
            get { return _index; }
        }

        public PacketReader(byte[] data, int size, bool fixedSize)
        {
            _buffer = data;
            _length = size;
            _index = fixedSize ? 1 : 3;
        }

        public byte[] Buffer
        {
            get { return _buffer; }
        }

        public int Size
        {
            get { return _length; }
        }

        public int Seek(int offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin: _index = offset; break;
                case SeekOrigin.Current: _index += offset; break;
                case SeekOrigin.End: _index = _length - offset; break;
            }
            return _index;
        }

        public int ReadInt32()
        {
            if ((_index + 4) > _length)
                return 0;
            return (_buffer[_index++] << 24)
                 | (_buffer[_index++] << 16)
                 | (_buffer[_index++] << 8)
                 | _buffer[_index++];
        }

        public short ReadInt16()
        {
            if ((_index + 2) > _length)
                return 0;
            return (short)((_buffer[_index++] << 8) | _buffer[_index++]);
        }

        public byte ReadByte()
        {
            if ((_index + 1) > _length)
                return 0;
            return _buffer[_index++];
        }

        public byte[] ReadBytes(int length)
        {
            if ((_index + length) > _length)
                return new byte[0];
            var b = new byte[length];
            Array.Copy(_buffer, _index, b, 0, length);
            _index += length;
            return b;
        }

        public ulong ReadUInt64()
        {
            if ((_index + 8) > _length)
                return 0;
            return (ulong)(
                ((ulong)_buffer[_index++] << 56) | ((ulong)_buffer[_index++] << 48) | ((ulong)_buffer[_index++] << 40) | ((ulong)_buffer[_index++] << 32) |
                ((ulong)_buffer[_index++] << 24) | ((ulong)_buffer[_index++] << 16) | ((ulong)_buffer[_index++] << 8) | (ulong)_buffer[_index++]);
        }

        public uint ReadUInt32()
        {
            if ((_index + 4) > _length)
                return 0;
            return (uint)((_buffer[_index++] << 24) | (_buffer[_index++] << 16) | (_buffer[_index++] << 8) | _buffer[_index++]);
        }

        public ushort ReadUInt16()
        {
            if ((_index + 2) > _length)
                return 0;
            return (ushort)((_buffer[_index++] << 8) | _buffer[_index++]);
        }

        public sbyte ReadSByte()
        {
            if ((_index + 1) > _length)
                return 0;
            return (sbyte)_buffer[_index++];
        }

        public bool ReadBoolean()
        {
            if ((_index + 1) > _length)
                return false;
            return (_buffer[_index++] != 0);
        }

        public string ReadUnicodeStringLE()
        {
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < _length && (c = (_buffer[_index++] | (_buffer[_index++] << 8))) != 0)
                b.Append((char)c);
            return b.ToString();
        }

        public string ReadUnicodeStringLESafe(int fixedLength)
        {
            var bound = _index + (fixedLength << 1);
            var end = bound;
            if (bound > _length)
                bound = _length;
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < bound && (c = (_buffer[_index++] | (_buffer[_index++] << 8))) != 0)
                if (IsSafeChar(c))
                    b.Append((char)c);
            _index = end;
            return b.ToString();
        }

        public string ReadUnicodeStringLESafe()
        {
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < _length && (c = (_buffer[_index++] | (_buffer[_index++] << 8))) != 0)
                if (IsSafeChar(c))
                    b.Append((char)c);
            return b.ToString();
        }

        public string ReadUnicodeStringSafe()
        {
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < _length && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
                if (IsSafeChar(c))
                    b.Append((char)c);
            return b.ToString();
        }

        public string ReadUnicodeString()
        {
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < _length && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
                b.Append((char)c);
            return b.ToString();
        }

        public bool IsSafeChar(int c)
        {
            return ((c >= 0x20 && c < 0xFFFE) || (c == 0x09));
        }

        public string ReadUTF8StringSafe(int fixedLength)
        {
            if (_index >= _length)
            {
                _index += fixedLength;
                return String.Empty;
            }
            var bound = _index + fixedLength;
            //var end   = bound;
            if (bound > _length)
                bound = _length;
            var count = 0;
            var index = _index;
            var start = _index;
            while (index < bound && _buffer[index++] != 0)
                ++count;
            index = 0;
            var buffer = new byte[count];
            var value = 0;
            while (_index < bound && (value = _buffer[_index++]) != 0)
                buffer[index++] = (byte)value;
            var s = Utility.UTF8.GetString(buffer);
            var isSafe = true;
            for (var i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar((int)s[i]);
            _index = start + fixedLength;
            if (isSafe)
                return s;
            var b = new StringBuilder(s.Length);
            for (var i = 0; i < s.Length; ++i)
                if (IsSafeChar((int)s[i]))
                    b.Append(s[i]);
            return b.ToString();
        }

        public string ReadUTF8StringSafe()
        {
            if (_index >= _length)
                return String.Empty;
            var count = 0;
            var index = _index;
            while (index < _length && _buffer[index++] != 0)
                ++count;
            index = 0;
            var buffer = new byte[count];
            var value = 0;
            while (_index < _length && (value = _buffer[_index++]) != 0)
                buffer[index++] = (byte)value;
            var s = Utility.UTF8.GetString(buffer);
            var isSafe = true;
            for (var i = 0; isSafe && i < s.Length; ++i)
                isSafe = IsSafeChar((int)s[i]);
            if (isSafe)
                return s;
            var b = new StringBuilder(s.Length);
            for (var i = 0; i < s.Length; ++i)
                if (IsSafeChar((int)s[i]))
                    b.Append(s[i]);
            return b.ToString();
        }

        public string ReadUTF8String()
        {
            if (_index >= _length)
                return String.Empty;
            var count = 0;
            var index = _index;
            while (index < _length && _buffer[index++] != 0)
                ++count;
            index = 0;
            var buffer = new byte[count];
            var value = 0;
            while (_index < _length && (value = _buffer[_index++]) != 0)
                buffer[index++] = (byte)value;
            return Utility.UTF8.GetString(buffer);
        }

        public string ReadString()
        {
            var b = new StringBuilder();
            int c;
            while (_index < _length && (c = _buffer[_index++]) != 0)
                b.Append((char)c);
            return b.ToString();
        }

        public string ReadStringSafe()
        {
            var b = new StringBuilder();
            int c;
            while (_index < _length && (c = _buffer[_index++]) != 0)
                if (IsSafeChar(c))
                    b.Append((char)c);
            return b.ToString();
        }

        public string ReadUnicodeStringSafe(int fixedLength)
        {
            var bound = _index + (fixedLength << 1);
            var end = bound;
            if (bound > _length)
                bound = _length;
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < bound && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
                if (IsSafeChar(c))
                    b.Append((char)c);
            _index = end;
            return b.ToString();
        }

        public string ReadUnicodeStringSafeReverse()
        {
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < _length && (c = ((_buffer[_index++]) | _buffer[_index++] << 8)) != 0)
                if (IsSafeChar(c))
                    b.Append((char)c);
            return b.ToString();
        }

        public string ReadUnicodeStringReverse(int fixedLength)
        {
            var bound = _index + (fixedLength << 1);
            var end = bound;
            if (bound > _length)
                bound = _length;
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < bound && (c = ((_buffer[_index++]) | _buffer[_index++] << 8)) != 0)
                b.Append((char)c);
            _index = end;
            return b.ToString();
        }

        public string ReadUnicodeString(int fixedLength)
        {
            var bound = _index + (fixedLength << 1);
            var end = bound;
            if (bound > _length)
                bound = _length;
            var b = new StringBuilder();
            int c;
            while ((_index + 1) < bound && (c = ((_buffer[_index++] << 8) | _buffer[_index++])) != 0)
                b.Append((char)c);
            _index = end;
            return b.ToString();
        }

        public string ReadStringSafe(int fixedLength)
        {
            var bound = _index + fixedLength;
            var end = bound;
            if (bound > _length)
                bound = _length;
            var b = new StringBuilder();
            int c;
            while (_index < bound && (c = _buffer[_index++]) != 0)
                if (IsSafeChar(c))
                    b.Append((char)c);
            _index = end;
            return b.ToString();
        }

        public string ReadString(int fixedLength)
        {
            var bound = _index + fixedLength;
            var end = bound;
            if (bound > _length)
                bound = _length;
            var b = new StringBuilder();
            int c;
            while (_index < bound && (c = _buffer[_index++]) != 0)
                b.Append((char)c);
            _index = end;
            return b.ToString();
        }

        public void Dispose()
        {
            _buffer = null;
            _length = 0;
            _index = 0;
            ReleaseInstance(this);
        }
    }
}
