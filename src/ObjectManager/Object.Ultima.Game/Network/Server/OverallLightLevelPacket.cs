using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class OverallLightLevelPacket : RecvPacket
    {
        readonly byte _lightLevel;

        public byte LightLevel
        {
            get { return _lightLevel; }
        }

        public OverallLightLevelPacket(PacketReader reader)
            : base(0x4F, "OverallLightLevel")
        {
            _lightLevel = reader.ReadByte();
        }
    }
}
