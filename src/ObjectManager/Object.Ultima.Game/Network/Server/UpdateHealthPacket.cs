using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class UpdateHealthPacket : RecvPacket
    {
        readonly Serial _serial;
        readonly short _current;
        readonly short _max;

        public Serial Serial
        {
            get { return _serial; }
        }

        public short Current
        {
            get { return _current; }
        }

        public short Max
        {
            get { return _max; }
        }

        public UpdateHealthPacket(PacketReader reader)
            : base(0xA1, "Update Health")
        {
            _serial = reader.ReadInt32();
            _max = reader.ReadInt16();
            _current = reader.ReadInt16();
        }
    }
}
