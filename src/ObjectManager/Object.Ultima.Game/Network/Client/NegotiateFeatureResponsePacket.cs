using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    class NegotiateFeatureResponsePacket : SendPacket
    {
        public NegotiateFeatureResponsePacket()
            : base(0xF0, "Negotiate Feature Response")
        {
            Stream.Write((byte)0xFF); // acknowledge handshake.
        }
    }
}
