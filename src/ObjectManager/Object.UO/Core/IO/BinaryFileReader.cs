using System;
using System.IO;
using System.Net;
using System.Text;

namespace OA.Ultima.Core.IO
{
    public sealed class BinaryFileReader : GenericReader
    {
        readonly BinaryReader _file;

        public BinaryFileReader(MemoryStream stream)
        {
            _file = new BinaryReader(stream);
        }

        public BinaryFileReader(BinaryReader br)
        {
            _file = br;
        }

        public long Position
        {
            get { return _file.BaseStream.Position; }
            set { _file.BaseStream.Position = value; }
        }

        public Stream Stream
        {
            get { return _file.BaseStream; }
        }

        public void Close()
        {
            _file.Close();
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            return _file.BaseStream.Seek(offset, origin);
        }

        public string ReadLine()
        {
            var sb = new StringBuilder();
            var reading = true;
            while (reading && !End())
            {
                var c = this.ReadChar();
                if (c == '\n') reading = false;
                else if (c == '\r') { } // discard
                else sb.Append(c);
            }
            return sb.ToString();
        }

        public override string ReadString()
        {
            return _file.ReadString();
        }

        public override DateTime ReadDeltaTime()
        {
            var ticks = _file.ReadInt64();
            var now = DateTime.Now.Ticks;
            if (ticks > 0 && (ticks + now) < 0)
                return DateTime.MaxValue;
            if (ticks < 0 && (ticks + now) < 0)
                return DateTime.MinValue;
            try
            {
                return new DateTime(now + ticks);
            }
            catch
            {
                if (ticks > 0)
                    return DateTime.MaxValue;
                return DateTime.MinValue;
            }
        }

        public override IPAddress ReadIPAddress()
        {
            return new IPAddress(_file.ReadInt64());
        }

        public override int ReadEncodedInt()
        {
            int v = 0, shift = 0;
            byte b;
            do
            {
                b = _file.ReadByte();
                v |= (b & 0x7F) << shift;
                shift += 7;
            } while (b >= 0x80);
            return v;
        }

        public override DateTime ReadDateTime()
        {
            return new DateTime(_file.ReadInt64());
        }

        public override TimeSpan ReadTimeSpan()
        {
            return new TimeSpan(_file.ReadInt64());
        }

        public override decimal ReadDecimal()
        {
            return _file.ReadDecimal();
        }

        public override long ReadLong()
        {
            return _file.ReadInt64();
        }

        public override ulong ReadULong()
        {
            return _file.ReadUInt64();
        }

        public override int ReadInt()
        {
            return _file.ReadInt32();
        }

        public override uint ReadUInt()
        {
            return _file.ReadUInt32();
        }

        public override short ReadShort()
        {
            return _file.ReadInt16();
        }

        public override ushort ReadUShort()
        {
            return _file.ReadUInt16();
        }

        public override double ReadDouble()
        {
            return _file.ReadDouble();
        }

        public override float ReadFloat()
        {
            return _file.ReadSingle();
        }

        public override char ReadChar()
        {
            return _file.ReadChar();
        }

        public override byte ReadByte()
        {
            return _file.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            return _file.ReadBytes(count);
        }

        public ushort[] ReadUShorts(int count)
        {
            var data = ReadBytes(count * 2);
            var data_out = new ushort[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 2);
            return data_out;
        }

        public int[] ReadInts(int count)
        {
            var data = ReadBytes(count * 4);
            var data_out = new int[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 4);
            return data_out;
        }

        public uint[] ReadUInts(int count)
        {
            var data = ReadBytes(count * 4);
            var data_out = new uint[count];
            Buffer.BlockCopy(data, 0, data_out, 0, count * 4);
            return data_out;
        }

        public int Read7BitEncodedInt()
        {
            var value = 0;
            while (true)
            {
                var temp = ReadByte();
                value += temp & 0x7F;
                if ((temp & 0x80) == 0x80) value = (value << 7);
                else return value;
            }
        }

        /// <summary>
        /// WARNING: INCOMPLETE, ONLY READS 2-byte UTF8 chars.
        /// </summary>
        /// <returns></returns>
        public char ReadCharUTF8()
        {
            var value = 0;
            var b0 = ReadByte();
            if ((b0 & 0x80) == 0x00)
                value = (b0 & 0x7F);
            else
            {
                value = (b0 & 0x3F);
                var b1 = ReadByte();
                if ((b1 & 0xE0) == 0xC0)
                    value += (b1 & 0x1F) << 6;
            }
            return (char)value;
        }

        public override sbyte ReadSByte()
        {
            return _file.ReadSByte();
        }

        public override bool ReadBool()
        {
            return _file.ReadBoolean();
        }

        public override bool End()
        {
            return _file.PeekChar() == -1;
        }
    }
}