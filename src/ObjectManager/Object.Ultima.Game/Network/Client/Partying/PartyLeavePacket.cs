using OA.Ultima.Core.Network.Packets;
using OA.Ultima.World;

namespace OA.Ultima.Network.Client.Partying
{
    public class PartyLeavePacket : SendPacket
    {
        public PartyLeavePacket()
            : base(0xbf, "Leave Party")
        {
            Stream.Write((short)6);
            Stream.Write((byte)2);
            Stream.Write(WorldModel.PlayerSerial);
        }
    }
}