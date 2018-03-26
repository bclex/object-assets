using OA.Ultima.Core.Network;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    public class PartyRemoveMemberInfo : IGeneralInfo
    {
        public readonly int Count;
        public readonly int RemovedMember;
        public readonly int[] Serials;

        public PartyRemoveMemberInfo(PacketReader reader)
        {
            Count = reader.ReadByte();
            RemovedMember = reader.ReadInt32();
            Serials = new int[Count];
            for (var i = 0; i < Count; i++)
                Serials[i] = reader.ReadInt32();
        }
    }
}
