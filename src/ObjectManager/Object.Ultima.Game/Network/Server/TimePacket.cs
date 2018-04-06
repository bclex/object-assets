using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class TimePacket : RecvPacket
    {
        readonly byte _hour, _minute, _second;
        public byte Hour { get { return _hour; } }
        public byte Minute { get { return _minute; } }
        public byte Second { get { return _second; } }

        public TimePacket(PacketReader reader)
            : base(0x5B, "Time")
        {
            _hour = reader.ReadByte();
            _minute = reader.ReadByte();
            _second = reader.ReadByte();
        }
    }
}
