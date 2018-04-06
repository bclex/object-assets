using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Partying
{
    public class PartyPublicMessagePacket : SendPacket
    {
        public PartyPublicMessagePacket(string msg)
            : base(0xbf, "Public Party Message")
        {
            Stream.Write((short)6);
            Stream.Write((byte)4);
            Stream.WriteBigUniNull(msg);
        }
    }
}