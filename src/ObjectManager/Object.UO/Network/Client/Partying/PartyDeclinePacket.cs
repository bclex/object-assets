using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Partying
{
    public class PartyDeclinePacket : SendPacket
    {
        public PartyDeclinePacket(Serial invitingPartyLeader)
            : base(0xbf, "Party Join Decline")
        {
            Stream.Write((short)6);
            Stream.Write((byte)9);
            Stream.Write(invitingPartyLeader);
        }
    }
}