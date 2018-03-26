using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server
{
    class ProtocolExtensionPacket : RecvPacket
    {
        public const byte SubcommandNegotiateFeatures = 0xFE;

        public readonly byte Subcommand;
        public readonly AssistantFeatures DisabledFeatures;

        public ProtocolExtensionPacket(PacketReader reader)
            : base(0xF0, "Protocol Extension Packet")
        {
            Subcommand = reader.ReadByte();
            switch (Subcommand)
            {
                case SubcommandNegotiateFeatures:
                    DisabledFeatures = (AssistantFeatures)reader.ReadUInt64();
                    break;
            }
        }
    }
}
