using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class RenameCharacterPacket : SendPacket
    {
        public RenameCharacterPacket(Serial serial, string name)
            : base(0x75, "Rename Request", 35)
        {
            Stream.Write(serial);
            Stream.WriteAsciiFixed(name, 30);
        }
    }
}
