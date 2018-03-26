using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class RemoveEntityPacket : RecvPacket
    {
        readonly Serial _serial;

        public Serial Serial
        {
            get { return _serial; }
        }

        public RemoveEntityPacket(PacketReader reader)
            : base(0x1D, "Remove Entity")
        {
            _serial = reader.ReadInt32();
        }
    }
}
