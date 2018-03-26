using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client.Partying
{
    class PartyCanLootPacket : SendPacket
    {
        public PartyCanLootPacket(bool isLootable)
            : base(0xbf, "Party Can Loot")
        {
            Stream.Write((short)6);
            Stream.Write((byte)6);
            Stream.Write(isLootable);
        }
    }
}