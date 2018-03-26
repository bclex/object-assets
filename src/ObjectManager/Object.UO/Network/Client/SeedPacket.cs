using OA.Core;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class SeedPacket : SendPacket
    {
        public SeedPacket(int seed, byte[] version)
            : base(0xEF, "Seed", 21)
        {
            Stream.Write(seed);
            if (version.Length != 4)
                Utils.Critical("SeedPacket: version array is not the correct length (4).");
            Stream.Write((int)version[0]);
            Stream.Write((int)version[1]);
            Stream.Write((int)version[2]);
            Stream.Write((int)version[3]);
        }
    }
}