using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class SingleClickPacket : SendPacket
    {
        public SingleClickPacket(Serial serial)
            : base(0x09, "Single Click", 5)
        {
            Stream.Write(serial);
        }
    }
}
