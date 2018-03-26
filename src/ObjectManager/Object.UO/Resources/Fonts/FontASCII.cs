using OA.Ultima.Core.UI;
using OA.Ultima.Core.UI.Fonts;
using System.IO;

namespace OA.Ultima.Resources.Fonts
{
    class FontAscii : AFont
    {
        private CharacterAscii[] _characters;

        public FontAscii()
        {
            _characters = new CharacterAscii[224];
        }

        public override void Initialize(BinaryReader reader)
        {
            var header = reader.ReadByte();
            // space characters have no data in AFont files.
            _characters[0] = new CharacterAscii();
            // We load all 224 characters; this seeds the font with correct height values.
            for (var i = 0; i < 224; i++)
            {
                var ch = loadCharacter(reader);
                var height = ch.Height;
                if (i > 32 && i < 90 && height > Height)
                    Height = height;
                _characters[i] = ch;
            }
            for (var i = 0; i < 224; i++)
                _characters[i].YOffset = Height - (_characters[i].Height + _characters[i].YOffset);
            // ascii fonts are so tall! why?
            Height -= 2;
            // Determine the width of the space character - arbitrarily .333 the width of capital M (.333 em?).
            GetCharacter(' ').Width = GetCharacter('M').Width / 3;
        }

        public override ICharacter GetCharacter(char character)
        {
            var index = ((int)character & 0xFFFFF) - 0x20;
            if (index < 0) return NullCharacter;
            if (index >= _characters.Length) return NullCharacter;
            return _characters[index];
        }

        private CharacterAscii NullCharacter = new CharacterAscii();

        CharacterAscii loadCharacter(BinaryReader reader)
        {
            var character = new CharacterAscii(reader);
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
