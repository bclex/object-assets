using OA.Core;
using OA.Ultima.Core.Network;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x06: Party info.
    /// </summary>
    class PartyInfo : IGeneralInfo
    {
        public const byte CommandPartyList = 0x01;
        public const byte CommandRemoveMember = 0x02;
        public const byte CommandPrivateMessage = 0x03;
        public const byte CommandPublicMessage = 0x04;
        public const byte CommandInvitation = 0x07;
        public readonly byte SubsubCommand;
        public readonly IGeneralInfo Info;

        public PartyInfo(PacketReader reader)
        {
            SubsubCommand = reader.ReadByte();
            switch (SubsubCommand)
            {
                case CommandPartyList: Info = new PartyMemberListInfo(reader); break;
                case CommandRemoveMember: Info = new PartyRemoveMemberInfo(reader); break;
                case CommandPrivateMessage: Info = new PartyMessageInfo(reader, true); break;
                case CommandPublicMessage: Info = new PartyMessageInfo(reader, false); break;
                case CommandInvitation: Info = new PartyInvitationInfo(reader); break;//PARTY INVITE PROGRESS
                default:
                    Utils.Warning($"Unhandled Subsubcommand {SubsubCommand:X2} in PartyInfo.");
                    break;
            }
        }
    }
}
