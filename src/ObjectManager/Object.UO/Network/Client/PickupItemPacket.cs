using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class PickupItemPacket : SendPacket
    {
        public PickupItemPacket(Serial serial, int count)
            : base(0x07, "Pickup Item", 7)
        {
            Stream.Write(serial);
            Stream.Write((short)count);
        }
    }
}
