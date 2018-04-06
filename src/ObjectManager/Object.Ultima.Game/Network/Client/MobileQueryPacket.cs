using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class MobileQueryPacket : SendPacket
    {
        public enum StatusType : byte
        {
            GodClient = 0x00,
            BasicStatus = 0x04,
            Skills = 0x05
        }

        public MobileQueryPacket(StatusType type, Serial serial)
            : base(0x34, "Get Player Status", 10)
        {
            Stream.Write(0xEDEDEDED); // always 0xEDEDEDED in legacy client
            Stream.Write((byte)type);
            Stream.Write(serial);
        }
    }
}
