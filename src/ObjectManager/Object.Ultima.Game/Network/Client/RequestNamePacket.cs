using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class RequestNamePacket : SendPacket
    {
        public RequestNamePacket(Serial serial)
            : base(0x98, "Request Name", 7)
        {
            Stream.Write((short)7);
            Stream.Write((int)serial);
        }
    }
}
