using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Partying
{
    class PartyRequestAddTargetPacket : SendPacket
    {
        public PartyRequestAddTargetPacket()
            : base(0xbf, "Add Party Member")
        {
            Stream.Write((short)6);
            Stream.Write((byte)1);
            Stream.Write(0);
        }
    }
}