using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Partying
{
    public class PartyPrivateMessagePacket : SendPacket
    {
        public PartyPrivateMessagePacket(Serial memberSerial, string msg)
            : base(0xbf, "Private Party Message")
        {
            Stream.Write((short)6);
            Stream.Write((byte)3);
            Stream.Write(memberSerial);
            Stream.WriteBigUniNull(msg);
        }
    }
}