using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class PlayMusicPacket : RecvPacket
    {
        readonly short _id;

        public short MusicID
        {
            get { return _id; }
        }

        public PlayMusicPacket(PacketReader reader)
            : base(0x6D, "Play Music")
        {
            _id = reader.ReadInt16();
        }
    }
}
