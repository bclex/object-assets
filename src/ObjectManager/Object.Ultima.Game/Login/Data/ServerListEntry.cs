using OA.Ultima.Core.Network;

namespace OA.Ultima.Login.Data
{
    public class ServerListEntry
    {
        public readonly ushort Index;
        public readonly string Name;
        public readonly byte PercentFull;
        public readonly byte Timezone;
        public readonly uint Address;

        public ServerListEntry(PacketReader reader)
        {
            Index = (ushort)reader.ReadInt16();
            Name = reader.ReadString(32);
            PercentFull = reader.ReadByte();
            Timezone = reader.ReadByte();
            Address = (uint)reader.ReadInt32();
        }
    }
}
