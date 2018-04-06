using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class AttackRequestPacket : SendPacket
    {
        public AttackRequestPacket(Serial serial)
            : base(0x05, "Attack Request", 5)
        {
            Stream.Write(serial);
        }
    }
}
