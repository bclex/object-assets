using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class ClientPingPacket : SendPacket
    {
        public ClientPingPacket()
            : base(0x73, "Ping Packet", 2)
        {
            Stream.Write((byte)0);
        }
    }
}
