using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class QuestArrowPacket : RecvPacket
    {
        public QuestArrowPacket(PacketReader reader)
            : base(0xBA, "Quest Arrow")
        {
            // TODO: Write this packet.
        }
    }
}
