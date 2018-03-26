using OA.Ultima.Core.Network;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x06 / 0x01: Party menber list.
    /// </summary>
    public class PartyMemberListInfo : IGeneralInfo
    {
        public readonly int Count;
        public readonly int[] Serials;

        public PartyMemberListInfo(PacketReader reader)
        {
            Count = reader.ReadByte();
            Serials = new int[Count];
            for (var i = 0; i < Count; i++)
                Serials[i] = reader.ReadInt32();
        }
    }
}
