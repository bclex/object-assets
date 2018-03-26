using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class SelectServerPacket : SendPacket
    {
        public SelectServerPacket(int id) :
            base(0xA0, "Select Server", 3)
        {
            Stream.Write((short)id);
        }
    }
}
