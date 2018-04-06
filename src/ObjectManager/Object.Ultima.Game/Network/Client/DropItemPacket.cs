using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class DropItemPacket : SendPacket
    {
        public DropItemPacket(Serial serial, ushort x, ushort y, byte z, byte gridIndex, Serial containerSerial)
            : base(0x08, "Drop Item", 15)
        {
            Stream.Write(serial);
            Stream.Write((ushort)x);
            Stream.Write((ushort)y);
            Stream.Write((byte)z);
            Stream.Write((byte)gridIndex);
            Stream.Write(containerSerial);
        }
    }
}
