using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class ObjectHelpResponsePacket : RecvPacket
    {
        public ObjectHelpResponsePacket(PacketReader reader)
            : base(0xB7, "Display Menu")
        {
            // TODO: Write this packet.
        }
    }
}
