using System.IO;

namespace OA.Ultima.Core.Network.Packets
{
    /// <summary>
    /// A formatted unit of data used in point to point communications.  
    /// </summary>
    public abstract class SendPacket : ISendPacket
    {
        const int BufferSize = 4096;

        /// <summary>
        /// Used to create the a buffered datablock to be sent
        /// </summary>
        protected PacketWriter Stream;
        readonly int _id;
        int _length;
        readonly string _name;

        /// <summary>
        /// Gets the name of the packet
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// Gets the size in bytes of the packet
        /// </summary>
        public int Length
        {
            get { return _length; }
        }

        /// <summary>
        /// Gets the Id, or Command that identifies the packet.
        /// </summary>
        public int Id
        {
            get { return _id; }
        }

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        public SendPacket(int id, string name)
        {
            _id = id;
            _name = name;
            Stream = PacketWriter.CreateInstance(_length);
            Stream.Write((byte)id);
            Stream.Write((short)0);
        }

        /// <summary>
        /// Creates an instance of a packet
        /// </summary>
        /// <param name="id">the Id, or Command that identifies the packet</param>
        /// <param name="name">The name of the packet</param>
        /// <param name="length">The size in bytes of the packet</param>
        public SendPacket(int id, string name, int length)
        {
            _id = id;
            _name = name;
            _length = length;
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)id);
        }

        /// <summary>
        /// Resets the Packet Writer and ensures the packet's 2nd and 3rd bytes are used to store the length
        /// </summary>
        /// <param name="length"></param>
        public void EnsureCapacity(int length)
        {
            Stream = PacketWriter.CreateInstance(length);
            Stream.Write((byte)_id);
            Stream.Write((short)length);
        }

        /// <summary>
        /// Compiles the packet into a System.Byte[] and Disposes the underlying Stream
        /// </summary>
        /// <returns></returns>
        public byte[] Compile()
        {
            Stream.Flush();
            if (Length == 0)
            {
                _length = (int)Stream.Length;
                Stream.Seek((long)1, SeekOrigin.Begin);
                Stream.Write((ushort)_length);
            }
            return Stream.Compile();
        }

        public override string ToString()
        {
            return string.Format("Id: {0:X2} Name: {1} Length: {2}", _id, _name, _length);
        }
    }
}
