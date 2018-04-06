using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class DeleteCharacterPacket : SendPacket
    {
        public DeleteCharacterPacket(int characterIndex, int clientIp)
            : base(0x83, "Delete Character", 39)
        {
            Stream.WriteAsciiFixed("", 30);
            Stream.Write(characterIndex);
            Stream.Write(clientIp);
        }
    }
}
