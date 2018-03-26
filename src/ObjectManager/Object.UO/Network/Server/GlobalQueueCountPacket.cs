using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class GlobalQueuePacket : RecvPacket
    {
        readonly byte _unk;
        readonly short _count;

        public byte Unknown
        {
            get { return _unk; }
        }

        public short Count
        {
            get { return _count; }
        }

        public GlobalQueuePacket(PacketReader reader)
            : base(0xCB, "Global Queue")
        {
            _unk = reader.ReadByte();
            _count = reader.ReadInt16();
        }
    }
}
