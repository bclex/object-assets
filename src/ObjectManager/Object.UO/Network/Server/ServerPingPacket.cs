using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class ServerPingPacket : RecvPacket
    {
        public readonly byte Sequence;

        public ServerPingPacket(PacketReader reader)
            : base(0x73, "Server Ping Response")
        {
            Sequence = reader.ReadByte();
        }
    }
}
