using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Client
{
    public class LoginPacket : SendPacket
    {
        public LoginPacket(string username, string password)
            : base(0x80, "Account Login", 0x3E)
        {
            Stream.WriteAsciiFixed(username, 30);
            Stream.WriteAsciiFixed(password, 30);
            Stream.Write((byte)0x5D);
        }
    }
}
