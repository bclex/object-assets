using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class LoginCompletePacket : RecvPacket
    {
        public LoginCompletePacket(PacketReader reader)
            : base(0x55, "Login Complete")
        {
            // No data in this packet.
        }
    }
}
