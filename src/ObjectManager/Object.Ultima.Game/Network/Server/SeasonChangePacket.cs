using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    /// <summary>
    /// Seasonal Information packet.
    /// </summary>
    public class SeasonChangePacket : RecvPacket
    {
        public bool SeasonChanged { get; private set; }
        public Seasons Season { get; private set; }
        public SeasonChangePacket(PacketReader reader)
    : base(0xBC, "Seasonal Information")
        {
            Season = (Seasons)reader.ReadByte();
            SeasonChanged = reader.ReadByte() == 1;
        }
    }
}
