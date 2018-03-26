using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    public class AsciiMessagePacket : RecvPacket
    {
        public readonly Serial Serial;
        public readonly short Model;
        public readonly MessageTypes MsgType;
        public readonly ushort Hue;
        public readonly short Font;
        public readonly string SpeakerName;
        public readonly string Text;

        public AsciiMessagePacket(PacketReader reader)
            : base(0x1C, "Ascii Message")
        {
            Serial = reader.ReadInt32();
            Model = reader.ReadInt16();
            MsgType = (MessageTypes)reader.ReadByte();
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            SpeakerName = reader.ReadString(30).Trim();
            Text = reader.ReadString();
        }
    }
}
