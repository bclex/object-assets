using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class MoveRequestPacket : SendPacket
    {
        public MoveRequestPacket(byte direction, byte sequence, int fastWalkPreventionKey)
            : base(0x02, "Move Request", 7)
        {
            Stream.Write((byte)direction);
            Stream.Write((byte)sequence);
            Stream.Write(fastWalkPreventionKey);
        }
    }
}
