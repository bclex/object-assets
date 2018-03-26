using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Login.Data;

namespace OA.Ultima.Network.Server
{
    public class ServerListPacket : RecvPacket
    {
        readonly byte _flags;
        readonly ServerListEntry[] _servers;

        public byte Flags
        {
            get { return _flags; }
        }

        public ServerListEntry[] Servers
        {
            get { return _servers; }
        }

        public ServerListPacket(PacketReader reader)
            : base(0xA8, "Server List")
        {
            _flags = reader.ReadByte();
            var count = (ushort)reader.ReadInt16();
            _servers = new ServerListEntry[count];
            for (var i = 0; i < count; i++)
                _servers[i] = new ServerListEntry(reader);
        }
    }
}
