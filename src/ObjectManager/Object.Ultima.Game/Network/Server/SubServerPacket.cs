using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class SubServerPacket : RecvPacket
    {
        public readonly short X;
        public readonly short Y;
        public readonly short Z;
        public readonly short MapWidth;
        public readonly short MapHeight;

        public SubServerPacket(PacketReader reader)
            : base(0x76, "Move to subserver")
        {
            X = reader.ReadInt16();
            Y = reader.ReadInt16();
            Z = reader.ReadInt16();
            reader.ReadByte(); // unknown - always 0
            reader.ReadInt16(); // server boundary x
            reader.ReadInt16(); // server boundary y
            MapWidth = reader.ReadInt16();
            MapHeight = reader.ReadInt16();
        }
    }
}
