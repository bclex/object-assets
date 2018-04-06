using OA.Core;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class ClientVersionPacket : SendPacket
    {
        public ClientVersionPacket(byte[] version)
            : base(0xBD, "Client Version")
        {
            if (version.Length != 4)
                Utils.Critical("SeedPacket: Incorrect length of version array.");
            Stream.WriteAsciiNull(string.Format("{0}.{1}.{2}.{3}", version[0], version[1], version[2], version[3]));
        }
    }
}
