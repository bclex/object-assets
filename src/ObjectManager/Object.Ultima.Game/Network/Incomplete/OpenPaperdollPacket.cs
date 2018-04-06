using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class OpenPaperdollPacket : RecvPacket
    {
        public int Serial { get; set; }
        public string MobileTitle { get; set; }
        public OpenPaperdollPacket(PacketReader reader)
            : base(0x88, "Open Paperdoll")
        {
            Serial = reader.ReadInt32();
            MobileTitle = reader.ReadStringSafe(60);
            //+flags
        }
    }
}