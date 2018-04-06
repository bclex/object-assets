using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    public class MessageLocalizedPacket : RecvPacket
    {
        public bool IsSystemMessage { get { return (Serial == 0xFFFF); } }
        public readonly Serial Serial;
        public readonly int Body;
        public readonly MessageTypes MessageType;
        public readonly ushort Hue;
        public readonly int Font;
        public readonly int CliLocNumber;
        public readonly string SpeakerName;
        public readonly string Arguements;

        public MessageLocalizedPacket(PacketReader reader)
            : base(0xC1, "Message Localized")
        {
            Serial = reader.ReadInt32(); // 0xffff for system message
            Body = reader.ReadInt16(); // (0xff for system message
            MessageType = (MessageTypes)reader.ReadByte(); // 6 - lower left, 7 on player
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            CliLocNumber = reader.ReadInt32();
            SpeakerName = reader.ReadString(30).Trim();
            Arguements = reader.ReadUnicodeStringSafeReverse();
        }
    }
}
