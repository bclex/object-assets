using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    public class TextLabel : AControl
    {
        const int DefaultLabelWidth = 300;

        public int Hue { get; set; }

        string _text;
        public string Text
        {
            get { return _text; }
            set
            {
                if (_textRenderer == null)
                    _textRenderer = new RenderedText(_text, DefaultLabelWidth);
                _textRenderer.Text = _text = value;
            }
        }

        RenderedText _textRenderer;

        TextLabel(AControl parent)
            : base(parent)
        {
        }

        public TextLabel(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var hue = ServerRecievedHueTransform(int.Parse(arguements[3]));
            var textIndex = int.Parse(arguements[4]);
            BuildGumpling(x, y, hue, lines[textIndex]);
        }

        public TextLabel(AControl parent, int x, int y, int hue, string text)
            : this(parent)
        {
            BuildGumpling(x, y, hue, text);
        }

        void BuildGumpling(int x, int y, int hue, string text)
        {
            Position = new Vector2Int(x, y);
            Text = string.Format("{0}{1}", hue == 0 ? string.Empty : "<outline>", text);
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            _textRenderer.Draw(spriteBatch, position, Utility.GetHueVector(Hue, false, false, true));
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
