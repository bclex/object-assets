using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class DeathAnimationPacket : RecvPacket
    {
        public readonly Serial PlayerSerial;
        public readonly Serial CorpseSerial;
        public DeathAnimationPacket(PacketReader reader)
            : base(0xAF, "Death Animation")
        {
            PlayerSerial = reader.ReadInt32();
            CorpseSerial = reader.ReadInt32();
            reader.ReadInt32(); // unknown - all zero's.
        }
    }
}
