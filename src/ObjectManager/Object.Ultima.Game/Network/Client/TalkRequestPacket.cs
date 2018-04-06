using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{    
    public class TalkRequestPacket : SendPacket
    {
        public TalkRequestPacket(byte speechType, short color, short font, string message)
            : base(0x03, "Talk Request")
        {
            Stream.Write((byte)speechType);
            Stream.Write((short)color);
            Stream.Write((short)font);
            Stream.WriteAsciiNull(message);
        }
    }
}
