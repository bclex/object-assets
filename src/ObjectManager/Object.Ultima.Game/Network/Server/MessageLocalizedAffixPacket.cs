using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    public class MessageLocalizedAffixPacket : RecvPacket
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
        public readonly string Affix;

        private readonly byte flags;
        public bool Flag_IsPrefix { get { return (flags & 0x01) != 0x00; } }
        public bool Flag_IsSystem { get { return (flags & 0x02) != 0x00; } }
        public bool Flag_MessageDoesNotMove { get { return (flags & 0x04) != 0x00; } }

        public MessageLocalizedAffixPacket(PacketReader reader)
            : base(0xCC, "Message Localized Affix")
        {
            Serial = reader.ReadInt32(); // 0xffff for system message
            Body = reader.ReadInt16(); // (0xff for system message
            MessageType = (MessageTypes)reader.ReadByte(); // 6 - lower left, 7 on player
            Hue = reader.ReadUInt16();
            Font = reader.ReadInt16();
            CliLocNumber = reader.ReadInt32();
            flags = reader.ReadByte();
            SpeakerName = reader.ReadString(30);
            Affix = reader.ReadStringSafe();
            Arguements = reader.ReadUnicodeStringSafeReverse();
        }
    }
}
