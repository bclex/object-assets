using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class RequestNoticePacket : SendPacket
    {
        public RequestNoticePacket(short lastNoticeNumber)
            : base(0xA7, "Request Notice", 4)
        {
            Stream.Write((short)lastNoticeNumber);
            Stream.Write((byte)0x01);
        }
    }
}
