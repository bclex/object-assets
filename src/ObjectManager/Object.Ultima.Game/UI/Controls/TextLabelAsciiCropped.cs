using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class TextLabelAsciiCropped : AControl
    {
        public int Hue;
        public int FontID;

        RenderedText _rendered;
        string _text;

        public string Text
        {
            get { return _text; }
            set
            {
                _text = value;
                _rendered.Text = string.Format("<span style=\"font-family=ascii{0}\">{1}", FontID, _text);
            }
        }

        TextLabelAsciiCropped(AControl parent)
            : base(parent)
        {
        }

        public TextLabelAsciiCropped(AControl parent, int x, int y, int width, int height, int font, int hue, string text)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, font, hue, text);
        }

        void BuildGumpling(int x, int y, int width, int height, int font, int hue, string text)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            _rendered = new RenderedText(string.Empty, width);
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
