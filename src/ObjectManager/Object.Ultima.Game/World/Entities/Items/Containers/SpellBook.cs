using OA.Ultima.Data;
using OA.Ultima.World.Maps;
using System.Linq;

namespace OA.Ultima.World.Entities.Items.Containers
{
    class SpellBook : ContainerItem
    {
        static ushort[] _spellBookItemIDs = {
            0xE3B, // bugged or static item spellbook? Not wearable.
            0xEFA, // standard, wearable spellbook
            0x2252, // paladin
            0x2253, // necro
            0x238C, // bushido
            0x23A0, // ninjitsu
            0x2D50 // spellweaving
        };

        /// <summary>
        ///  Returns true if the parameter ItemID matches a Spellbook Item.
        /// </summary>
        /// <param name="itemID">An itemID to be tested</param>
        /// <returns>True if the itemID is a Spellbook ite, false otherwise.</returns>
        public static bool IsSpellBookItem(ushort itemID)
        {
            return _spellBookItemIDs.Contains<ushort>(itemID);
        }

        public SpellBookTypes BookType { get; private set; }

        ulong _spellsBitfield;
        public bool HasSpell(int circle, int index)
        {
            index = ((3 - circle % 4) + (circle / 4) * 4) * 8 + (index - 1);
            var flag = ((ulong)1) << index;
            return (_spellsBitfield & flag) == flag;
        }

        public SpellBook(Serial serial, Map map)
            : base(serial, map)
        {
            BookType = SpellBookTypes.Unknown;
            _spellsBitfield = 0;
        }

        public void ReceiveSpellData(SpellBookTypes sbType, ulong sbBitfield)
        {
            var entityUpdated = false;
            if (BookType != sbType)
            {
                BookType = sbType;
                entityUpdated = true;
            }
            if (_spellsBitfield != sbBitfield)
            {
                _spellsBitfield = sbBitfield;
                entityUpdated = true;
            }
            if (entityUpdated)
                _onUpdated?.Invoke(this);
        }
    }
}