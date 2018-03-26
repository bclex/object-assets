using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    public class UnicodeMessagePacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly short Model;
        public readonly MessageTypes MsgType;
        public readonly ushort Hue;
        public readonly short Font;
        public readonly string Language;
        public readonly string SpeakerName;
        public readonly string Text;
        
        public UnicodeMessagePacket(PacketReader reader)
            : base(0xAE, "Unicode Message")
        {
            Serial = reader.ReadInt32();
            Model = reader.ReadInt16();
            MsgType = (MessageTypes)reader.ReadByte();
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            Language = reader.ReadString(4).Trim();
            SpeakerName = reader.ReadString(30).Trim();
            Text = reader.ReadUnicodeString((reader.Buffer.Length - 48) / 2);
        }
    }
}
