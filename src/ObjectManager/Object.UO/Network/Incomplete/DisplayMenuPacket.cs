using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class DisplayMenuPacket : RecvPacket
    {
        public DisplayMenuPacket(PacketReader reader)
            : base(0x7C, "Display Menu")
        {
            // TODO: Write this packet.
        }
    }
}
