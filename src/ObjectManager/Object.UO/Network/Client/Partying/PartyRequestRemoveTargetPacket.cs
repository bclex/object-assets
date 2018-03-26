using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Partying
{
    public class PartyRequestRemoveTargetPacket : SendPacket
    {
        public PartyRequestRemoveTargetPacket()
            : base(0xbf, "Remove Party Member")
        {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(0);
        }
    }
}