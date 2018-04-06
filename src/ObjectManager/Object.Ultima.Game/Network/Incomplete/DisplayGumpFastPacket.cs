using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class DisplayGumpFastPacket : RecvPacket
    {
        public DisplayGumpFastPacket(PacketReader reader)
            : base(0xB0, "Display Gump Fast")
        {
            // TODO: Write this packet.
        }
    }
}
