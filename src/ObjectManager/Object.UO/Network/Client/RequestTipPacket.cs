using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class RequestTipPacket : SendPacket
    {
        public RequestTipPacket(short lastTipNumber)
            : base(0xA7, "Request Tip", 4)
        {
            Stream.Write((short)lastTipNumber);
            Stream.Write((byte)0x00);
        }
    }
}
