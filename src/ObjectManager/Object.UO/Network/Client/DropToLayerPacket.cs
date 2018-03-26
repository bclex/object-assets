using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class DropToLayerPacket : SendPacket
    {
        public DropToLayerPacket(Serial itemSerial, byte layer, Serial playerSerial)
            : base(0x13, "Drop To Layer", 10)
        {
            Stream.Write(itemSerial);
            Stream.Write((byte)layer);
            Stream.Write(playerSerial);
        }
    }
}
