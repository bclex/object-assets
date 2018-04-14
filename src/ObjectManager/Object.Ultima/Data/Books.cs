using System.Linq;

namespace OA.Ultima.Data
{
    public class Books
    {
        ushort[] _gumpBaseIDs =
        {
            0x1F4, // Yellow Cornered Book
            0x1FE, // Regular Cornered Book
            0x898, // Funky Book?
            0x899, // Tan Book?
            0x89A, // Red Book?
            0x89B, // Blue Book?
            0x8AC, // SpellBook
            0x2B00, // Necromancy Book?
            0x2B01, // Ice Book?
            0x2B02, // Arms Book?
            0x2B06, // Bushido Book?
            0x2B07, // Another Crazy Kanji Thing
            0x2B2F // A Greenish Book
        };

        static readonly ushort[] _bookItemIDs = {
            0xFEF, // Brown Book
            0xFF0, // Tan Book
            0xFF1, // Red Book
            0xFF2  // Blue Book
        };

        public static bool IsBookItem(ushort itemID)
        {
            return _bookItemIDs.Contains(itemID);
        }
    }
}
