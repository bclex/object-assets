using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Partying
{
    public class PartyAcceptPacket : SendPacket
    {
        public PartyAcceptPacket(Serial invitingPartyLeader)
            : base(0xbf, "Party Join Accept")
        {
            Stream.Write((short)6);
            Stream.Write((byte)8);
            Stream.Write(invitingPartyLeader);
        }
    }
}