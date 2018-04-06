using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class OpenContainerPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly ushort _gumpId;

        public Serial Serial { get { return _serial; } }
        public ushort GumpId { get { return _gumpId; } }

        public OpenContainerPacket(PacketReader reader)
            : base(0x24, "Open Container")
        {
            _serial = reader.ReadInt32();
            _gumpId = reader.ReadUInt16();
        }
    }
}
