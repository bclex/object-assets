using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class TextLabelAscii : AControl
    {
        public int Hue;
        public int FontID;

        readonly RenderedText _rendered;
        string _text;
        int _width;

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    _rendered.Text = string.Format("<span style=\"font-family:ascii{0}\">{1}", FontID, _text);
                }
            }
        }

        public TextLabelAscii(AControl parent, int width = 400)
            : base(parent)
        {
            _width = width;
            _rendered = new RenderedText(string.Empty, _width, true);
        }

        public TextLabelAscii(AControl parent, int x, int y, int font, int hue, string text, int width = 400)
            : this(parent, width)
        {
            BuildGumpling(x, y, font, hue, text);
        }

        void BuildGumpling(int x, int y, int font, int hue, string text)
        {
            Position = new Vector2Int(x, y);
            Hue = hue;
            FontID = font;
            Text = text;
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            _rendered.Draw(spriteBatch, position, Utility.GetHueVector(Hue, true, false, true));
            base.Draw(spriteBatch, position, frameMS);
        }
    }
}
