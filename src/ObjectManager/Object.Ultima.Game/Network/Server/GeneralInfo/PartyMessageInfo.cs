using OA.Ultima.Core.Network;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x06 / 0x03 and 0x06 / 0x04: Party message.
    /// </summary>
    public class PartyMessageInfo : IGeneralInfo
    {
        public readonly bool IsPrivate;
        public readonly Serial Source;
        public readonly string Message;

        public PartyMessageInfo(PacketReader reader, bool isPrivate)
        {
            IsPrivate = isPrivate;
            Source = (Serial)reader.ReadInt32();
            Message = reader.ReadUnicodeString();
        }
    }
}
