using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;

namespace OA.Ultima.Network.Server
{
    public class ChatPacket : RecvPacket
    {
        readonly string _language;
        readonly byte _commandtype;

        public string Language
        {
            get { return _language; }
        }

        public byte CommandType
        {
            get { return _commandtype; }
        } 

        public ChatPacket(PacketReader reader)
            : base(0xB3, "Chat Packet")
        {
            _language = reader.ReadString(3);
            reader.ReadInt16(); // unknown.
            _commandtype = reader.ReadByte();
        }
    }
}
