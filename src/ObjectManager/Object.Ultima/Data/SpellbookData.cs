namespace OA.Ultima.Data
{
    public class SpellbookData
    {
        public readonly Serial Serial;
        public readonly ushort ItemID;
        public readonly SpellBookTypes BookType;
        public readonly ulong SpellsBitfield;

        public SpellbookData(Serial serial, ushort itemID, ushort bookTypePacketID, ulong spellBitFields)
        {
            Serial = serial;
            ItemID = itemID;
            SpellsBitfield = spellBitFields;
            switch (bookTypePacketID)
            {
                case 1: BookType = SpellBookTypes.Magic; break;
                case 101: BookType = SpellBookTypes.Necromancer; break;
                case 201: BookType = SpellBookTypes.Chivalry; break;
                case 401: BookType = SpellBookTypes.Bushido; break;
                case 501: BookType = SpellBookTypes.Ninjitsu; break;
                case 601: BookType = SpellBookTypes.Spellweaving; break;
                default: BookType = SpellBookTypes.Unknown; return;
            }
        }

        public static SpellBookTypes GetSpellBookTypeFromItemID(int itemID)
        {
            var bookType = SpellBookTypes.Unknown;
            switch (itemID)
            {
                case 0x0E3B: // spellbook
                case 0x0EFA: bookType = SpellBookTypes.Magic; break;
                case 0x2252: bookType = SpellBookTypes.Chivalry; break; // paladin spellbook
                case 0x2253: bookType = SpellBookTypes.Necromancer; break; // necromancer book
                case 0x238C: bookType = SpellBookTypes.Bushido; break; // book of bushido
                case 0x23A0: bookType = SpellBookTypes.Ninjitsu; break; // book of ninjitsu
                case 0x2D50: bookType = SpellBookTypes.Chivalry; break; // spell weaving book
            }
            return bookType;
        }

        public static int GetOffsetFromSpellBookType(SpellBookTypes spellbooktype)
        {
            switch (spellbooktype)
            {
                case SpellBookTypes.Magic: return 1;
                case SpellBookTypes.Necromancer: return 101;
                case SpellBookTypes.Chivalry: return 201;
                case SpellBookTypes.Bushido: return 401;
                case SpellBookTypes.Ninjitsu: return 501;
                case SpellBookTypes.Spellweaving: return 601;
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
