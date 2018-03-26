using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class OpenWebBrowserPacket : RecvPacket
    {
        readonly string _url;

        public string WebsiteUrl
        {
            get { return _url; }
        }

        public OpenWebBrowserPacket(PacketReader reader)
            : base(0xA5, "Open Web Browser")
        {
            _url = reader.ReadString(reader.Size);
        }
    }
}