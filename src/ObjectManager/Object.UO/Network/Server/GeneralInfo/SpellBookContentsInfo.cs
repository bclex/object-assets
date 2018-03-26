using OA.Ultima.Core.Network;

namespace OA.Ultima.Network.Server.GeneralInfo
{
    /// <summary>
    /// Subcommand 0x1B: the contents of a spellbook.
    /// </summary>
    class SpellBookContentsInfo : IGeneralInfo
    {
        public readonly SpellbookData Spellbook;

        public SpellBookContentsInfo(PacketReader reader)
        {
            var unknown = reader.ReadUInt16(); // always 1
            Serial serial = reader.ReadInt32();
            var itemID = reader.ReadUInt16();
            var spellbookType = reader.ReadUInt16(); // 1==regular, 101=necro, 201=paladin, 401=bushido, 501=ninjitsu, 601=spellweaving
            var spellBitfields = reader.ReadUInt32() + (((ulong)reader.ReadUInt32()) << 32); // first bit of first byte = spell #1, second bit of first byte = spell #2, first bit of second byte = spell #8, etc 
            Spellbook = new SpellbookData(serial, itemID, spellbookType, spellBitfields);
        }
    }
}
