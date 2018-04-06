using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class PlayerMovePacket : RecvPacket
    {
        readonly byte _direction;

        public byte Direction
        {
            get { return _direction; }
        }

        public PlayerMovePacket(PacketReader reader)
            : base(0x97, "Player Move")
        {
            _direction = reader.ReadByte();
        }
    }
}
