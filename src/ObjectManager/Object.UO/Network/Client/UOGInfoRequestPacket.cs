using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class UOGInfoRequestPacket : SendPacket
    {
        public UOGInfoRequestPacket()
            : base(0xF1, "UOG Information Request", 4)
        {
            Stream.Write((short)4);
            Stream.Write((byte)0xFF);
        }
    }
}
