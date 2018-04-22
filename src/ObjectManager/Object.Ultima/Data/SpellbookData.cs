namespace OA.Ultima.Data
{
    public class SpellbookData
    {
        public readonly Serial Serial;
        public readonly ushort ItemID;
        public readonly SpellbookTypes BookType;
        public readonly ulong SpellsBitfield;

        public SpellbookData(Serial serial, ushort itemID, ushort bookTypePacketID, ulong spellBitFields)
        {
            Serial = serial;
            ItemID = itemID;
            SpellsBitfield = spellBitFields;
            switch (bookTypePacketID)
            {
                case 1: BookType = SpellbookTypes.Magic; break;
                case 101: BookType = SpellbookTypes.Necromancer; break;
                case 201: BookType = SpellbookTypes.Chivalry; break;
                case 401: BookType = SpellbookTypes.Bushido; break;
                case 501: BookType = SpellbookTypes.Ninjitsu; break;
                case 601: BookType = SpellbookTypes.Spellweaving; break;
                default: BookType = SpellbookTypes.Unknown; return;
            }
        }

        public static SpellbookTypes GetSpellBookTypeFromItemID(int itemID)
        {
            var bookType = SpellbookTypes.Unknown;
            switch (itemID)
            {
                case 0x0E3B: // spellbook
                case 0x0EFA: bookType = SpellbookTypes.Magic; break;
                case 0x2252: bookType = SpellbookTypes.Chivalry; break; // paladin spellbook
                case 0x2253: bookType = SpellbookTypes.Necromancer; break; // necromancer book
                case 0x238C: bookType = SpellbookTypes.Bushido; break; // book of bushido
                case 0x23A0: bookType = SpellbookTypes.Ninjitsu; break; // book of ninjitsu
                case 0x2D50: bookType = SpellbookTypes.Chivalry; break; // spell weaving book
            }
            return bookType;
        }

        public static int GetOffsetFromSpellBookType(SpellbookTypes spellbooktype)
        {
            switch (spellbooktype)
            {
                case SpellbookTypes.Magic: return 1;
                case SpellbookTypes.Necromancer: return 101;
                case SpellbookTypes.Chivalry: return 201;
                case SpellbookTypes.Bushido: return 401;
                case SpellbookTypes.Ninjitsu: return 501;
                case SpellbookTypes.Spellweaving: return 601;
                default: return 1;
            }
        }

        //public SpellbookData(ContainerItem spellbook, ContainerContentPacket contents)
        //{
        //    Serial = spellbook.Serial;
        //    ItemID = (ushort)spellbook.ItemID;
        //    BookType = GetSpellBookTypeFromItemID(spellbook.ItemID);
        //    if (BookType == SpellBookTypes.Unknown)
        //        return;
        //    var offset = GetOffsetFromSpellBookType(BookType);
        //    foreach (var i in contents.Items)
        //    {
        //        var index = ((i.Amount - offset) & 0x0000003F);
        //        var circle = (index / 8);
        //        index = index % 8;
        //        index = ((3 - circle % 4) + (circle / 4) * 4) * 8 + (index);
        //        var flag = ((ulong)1) << index;
        //        SpellsBitfield |= flag;
        //    }
        //}
    }
}
