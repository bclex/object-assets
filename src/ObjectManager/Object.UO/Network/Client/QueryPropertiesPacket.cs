using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class QueryPropertiesPacket : SendPacket
    {
        public QueryPropertiesPacket(Serial serial)
            : base(0xD6, "Query Properties", 7)
        {
            Stream.Write((short)7);
            Stream.Write((int)serial);
        }
    }
}
