using System.IO;

namespace OA.Ultima.Core.Network.Packets
{
    public abstract class SendRecvPacket : ISendPacket, IRecvPacket
    {
        readonly int _id;
        readonly int _length;
        readonly string _name;

        public int Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
        }

        public int Length
        {
            get { return _length; }
        }

        const int BufferSize = 4096;

        protected PacketWriter Stream;

        public SendRecvPacket(int id, string name)
        {
            _id = id;
            _name = name;
            Stream = PacketWriter.CreateInstance(_length);
            Stream.Write(id);
            Stream.Write((short)0);
        }

        public SendRecvPacket(int id, string name, int length)
        {
            _id = id;
            _name = name;
            _length = length;
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)id);
        }

        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)_id);
            Stream.Write((short)length);
        }

        public byte[] Compile()
        {
            Stream.Flush();
            if (Length == 0)
            {
                var length = Stream.Length;
                Stream.Seek((long)1, SeekOrigin.Begin);
                Stream.Write((ushort)length);
                Stream.Flush();
            }
            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", _id, _name, _length);
        }
    }
}
