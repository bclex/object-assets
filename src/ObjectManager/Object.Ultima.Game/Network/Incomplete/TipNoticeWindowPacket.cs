using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class TipNoticePacket : RecvPacket
    {
        public TipNoticePacket(PacketReader reader)
            : base(0xA6, "Tip Notice")
        {
            // TODO: Write this packet.
        }
    }
}
