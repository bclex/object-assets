using OA.Ultima.Core.Network;
using OA.Ultima.Data;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x14: A context menu.
    /// </summary>
    class ContextMenuInfo : IGeneralInfo
    {
        public readonly ContextMenuData Menu;

        public ContextMenuInfo(PacketReader reader)
        {
            reader.ReadByte(); // unknown, always 0x00
            var subcommand = reader.ReadByte(); // 0x01 for 2D, 0x02 for KR
            Menu = new ContextMenuData(reader.ReadInt32());
            var contextMenuChoiceCount = reader.ReadByte();
            for (var i = 0; i < contextMenuChoiceCount; i++)
            {
                var iUniqueID = reader.ReadUInt16();
                var iClilocID = reader.ReadUInt16() + 3000000;
                var iFlags = reader.ReadUInt16(); // 0x00=enabled, 0x01=disabled, 0x02=arrow, 0x20 = color
                var iColor = 0;
                if ((iFlags & 0x20) == 0x20)
                    iColor = reader.ReadUInt16();
                Menu.AddItem(iUniqueID, iClilocID, iFlags, iColor);
            }
        }
    }
}
