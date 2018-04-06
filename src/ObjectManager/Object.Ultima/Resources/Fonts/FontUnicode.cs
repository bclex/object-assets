using OA.Ultima.Core.UI;
using OA.Ultima.Core.UI.Fonts;
using System.IO;

namespace OA.Ultima.Resources.Fonts
{
    class FontUnicode : AFont
    {
        BinaryReader _reader;
        CharacterUnicode[] _characters;
        CharacterUnicode NullCharacter = new CharacterUnicode();

        public FontUnicode()
        {
            _characters = new CharacterUnicode[0x10000];
        }

        public override void Initialize(BinaryReader reader)
        {
            _reader = reader;
            // space characters have no data in UniFont files.
            _characters[0] = new CharacterUnicode();
            // We load the first 96 characters to 'seed' the font with correct height values.
            for (var i = 33; i < 128; i++)
                GetCharacter((char)i);
            // Determine the width of the space character - arbitrarily .333 the width of capital M (.333 em?).
            GetCharacter(' ').Width = GetCharacter('M').Width / 3;
        }

        public override ICharacter GetCharacter(char character)
        {
            var index = ((int)character & 0xFFFFF) - 0x20;
            if (index < 0)
                return NullCharacter;
            if (_characters[index] == null)
            {
                var ch = loadCharacter(index + 0x20);
                var height = ch.Height + ch.YOffset;
                if (index < 128 && height > Height)
                    Height = height;
                _characters[index] = ch;
            }
            return _characters[index];
        }
        
        CharacterUnicode loadCharacter(int index)
        {
            // get the lookup table - 0x10000 ints.
            _reader.BaseStream.Position = index * 4;
            var lookup = _reader.ReadInt32();
            if (lookup == 0) return NullCharacter; // no character - so we just return null
            _reader.BaseStream.Position = lookup;
            var character = new CharacterUnicode(_reader);
            return character;
        }

        public int GetWidth(char ch)
        {
            return GetCharacter(ch).Width;
        }

        public int GetWidth(string text)
        {
            if (text == null || text.Length == 0) return 0;
            var width = 0;
            for (var i = 0; i < text.Length; ++i)
                width += GetCharacter(text[i]).Width;
            return width;
        }
    }
}
