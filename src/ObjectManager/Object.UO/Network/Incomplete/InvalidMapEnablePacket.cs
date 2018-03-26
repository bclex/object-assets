using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class InvalidMapEnablePacket : RecvPacket
    {
        public InvalidMapEnablePacket(PacketReader reader)
            : base(0xC6, "Invalid Map Enable")
        {
            // TODO: Write this packet.
        }
    }
}
