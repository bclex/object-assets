using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class QuestGumpRequestPacket : SendPacket
    {
        public QuestGumpRequestPacket(Serial serial)
            : base(0xD7, "Quest gump request")
        {
            Stream.Write(serial);
            Stream.Write((ushort)0x0032);
            Stream.Write((byte)0x00);
        }
    }
}
