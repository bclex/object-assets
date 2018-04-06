using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class VersionRequestPacket : RecvPacket
    {
        public VersionRequestPacket(PacketReader reader)
            : base(0xBD, "Client Version Request")
        {
            // no IO.
        }
    }
}
