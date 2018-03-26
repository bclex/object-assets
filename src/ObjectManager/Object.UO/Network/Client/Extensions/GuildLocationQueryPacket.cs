using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Extensions
{
    /// <summary>
    /// MapUO Protocol: Requests the position of all guild members.
    /// </summary>
    public class GuildLocationQueryPacket : SendPacket
    {
        public GuildLocationQueryPacket()
            : base(0xF0, "Query Guild Member Locations")
        {
            Stream.Write((byte)0);
        }
    }
}