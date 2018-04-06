using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class DoubleClickPacket : SendPacket
    {
        public DoubleClickPacket(Serial serial)
            : base(0x06, "Double Click", 5)
        {
            Stream.Write(serial);
        }
    }
}
