using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class Extended0x78Packet : RecvPacket
    {
        public Extended0x78Packet(PacketReader reader)
            : base(0xD3, "Extended 0x78")
        {
            // TODO: Write this packet.
        }
    }
}
