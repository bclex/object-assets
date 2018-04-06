using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class GuildGumpRequestPacket : SendPacket
    {
        public GuildGumpRequestPacket(Serial serial)
            : base(0xD7, "Guild gump request")
        {
            Stream.Write(serial);
            Stream.Write((ushort)0x0028);
            Stream.Write((byte)0x0A);
        }
    }
}
