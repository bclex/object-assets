using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Extensions
{
    /// <summary>
    /// MapUO Protocol: Requests the position of all party members.
    /// </summary>
    public class PartyLocationQueryPacket : SendPacket
    {
        public PartyLocationQueryPacket()
            : base(0xF0, "Query Party Member Locations")
        {
            Stream.Write((byte)0);
        }
    }
}