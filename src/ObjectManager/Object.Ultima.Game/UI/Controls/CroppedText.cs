using OA.Core.UI;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class CroppedText : AControl
    {
        public int Hue;
        public string Text = string.Empty;
        RenderedText _texture;

        CroppedText(AControl parent)
            : base(parent)
        {
        }

        public CroppedText(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var width = int.Parse(arguements[3]);
            var height = int.Parse(arguements[4]);
            var hue = int.Parse(arguements[5]);
            var textIndex = int.Parse(arguements[6]);
            BuildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        public CroppedText(AControl parent, int x, int y, int width, int height, int hue, int textIndex, string[] lines)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, hue, textIndex, lines);
        }

        void BuildGumpling(int x, int y, int width, int height, int hue, int textIndex, string[] lines)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            Hue = hue;
            Text = lines[textIndex];
            _texture = new RenderedText(Text, width);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            _texture.Draw(spriteBatch, new RectInt(position.x, position.y, Width, Height), 0, 0);
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
