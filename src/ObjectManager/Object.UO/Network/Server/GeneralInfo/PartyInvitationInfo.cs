using OA.Ultima.Core.Network;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x06 / 0x07: Invitation to joint a party.
    /// </summary>
    public class PartyInvitationInfo : IGeneralInfo
    {
        public readonly int PartyLeaderSerial;

        public PartyInvitationInfo(PacketReader reader)
        {
            PartyLeaderSerial = reader.ReadInt32();
        }
    }
}
