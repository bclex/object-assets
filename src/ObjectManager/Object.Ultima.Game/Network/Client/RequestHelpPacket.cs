using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class RequestHelpPacket : SendPacket
    {
        public RequestHelpPacket()
            : base(0x9B, "Request Help", 258)
        {
            var empty = new byte[257];
            Stream.Write(empty, 0, 257);
        }
    }
}
