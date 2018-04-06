using OA.Core;
using OA.Ultima.Core.Network;
using OA.Ultima.Core.Network.Packets;
using OA.Ultima.Network.Server.GeneralInfo;

namespace OA.Ultima.Network.Server
{
    public class GeneralInfoPacket : RecvPacket
    {
        public const int CloseGump = 0x04;
        public const int Party = 0x06;
        public const int SetMap = 0x08;
        public const int ShowLabel = 0x10;
        public const int ContextMenu = 0x14;
        public const int MapDiff = 0x18;
        public const int ExtendedStats = 0x19;
        public const int SpellBookContents = 0x1B;
        public const int HouseRevision = 0x1D;
        public const int AOSAbilityIconConfirm = 0x21;

        public readonly short Subcommand;
        public readonly IGeneralInfo Info;

        public GeneralInfoPacket(PacketReader reader)
            : base(0xBF, "General Information")
        {
            Subcommand = reader.ReadInt16();
            switch (Subcommand)
            {
                case CloseGump: Info = new CloseGumpInfo(reader); break;
                case Party: Info = new PartyInfo(reader); break;
                case SetMap: Info = new MapIndexInfo(reader); break;
                case ShowLabel: Info = new ShowLabelInfo(reader); break;
                case ContextMenu: Info = new ContextMenuInfo(reader); break;
                case MapDiff: Info = new MapDiffInfo(reader); break;
                case ExtendedStats: Info = new ExtendedStatsInfo(reader); break;
                case SpellBookContents: Info = new SpellBookContentsInfo(reader); break;
                case HouseRevision: Info = new HouseRevisionInfo(reader); break;
                case AOSAbilityIconConfirm: break; // (AOS) Ability icon confirm. no data, just (bf 00 05 00 21)
                default: Utils.Warn($"Unhandled Subcommand {Subcommand:X2} in GeneralInfoPacket."); break;
            }
        }
    }
}
