using OA.Core.Input;
using OA.Core.UI;
using OA.Core.Windows;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class KeyPressControl : AControl
    {
        public string LeadingHtmlTag = string.Empty;
        public WinKeys Key = WinKeys.None;
        public bool IsChanged { get; set; }
        private RenderedText _renderedText;

        public override bool HandlesMouseInput
        {
            get { return base.HandlesMouseInput & IsEditable; }
            set { base.HandlesMouseInput = value; }
        }

        public override bool HandlesKeyboardFocus
        {
            get { return base.HandlesKeyboardFocus & IsEditable; }
            set { base.HandlesKeyboardFocus = value; }
        }

        public KeyPressControl(AControl parent, int x, int y, int width, int height, int entryID, WinKeys key)
                : base(parent)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            Key = key;
            _renderedText = new RenderedText(string.Empty, width);
            IsChanged = false;
            base.HandlesMouseInput = true;
            base.HandlesKeyboardFocus = true;
            LeadingHtmlTag = "<center><span color='#fff' style='font-family: uni2;'>";
        }

        public override void Update(double totalMS, double frameMS)
        {
            var text = Key == WinKeys.None ? "Press Any" : Key.ToString();
            _renderedText.Text = LeadingHtmlTag + text;
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var hue = Key == WinKeys.None ? 33 : 2;
            if (IsEditable)
            {
                if (_renderedText.Width <= Width)
                    _renderedText.Draw(spriteBatch, position, Utility.GetHueVector(hue));
                else
                {
                    var textOffset = _renderedText.Width - (Width);
                    _renderedText.Draw(spriteBatch, new RectInt(position.x, position.y, _renderedText.Width - textOffset, _renderedText.Height), textOffset, 0, Utility.GetHueVector(hue));
                }
            }
            else _renderedText.Draw(spriteBatch, new RectInt(position.x, position.y, Width, Height), 0, 0, Utility.GetHueVector(hue));
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            if (e.Alt || e.Shift || e.Control || IsChanged)
                return;
            if (((int)e.KeyCode >= (int)WinKeys.A && (int)e.KeyCode <= (int)WinKeys.Z) || ((int)e.KeyCode >= (int)WinKeys.F1 && (int)e.KeyCode <= (int)WinKeys.F12)) Key = e.KeyCode;
            else if ((int)e.KeyCode >= (int)WinKeys.A + 32 && (int)e.KeyCode <= (int)WinKeys.Z + 32) // lower case?
            {
                if (e.KeyCode.ToString().StartsWith("NumPad")) Key = e.KeyCode;
                else if (e.KeyCode.ToString().Length == 1) Key = e.KeyCode;
            }
            else if (e.KeyCode >= WinKeys.D0 && e.KeyCode <= WinKeys.D9) Key = e.KeyCode;
            else if (e.KeyCode == WinKeys.NumPad0) Key = e.KeyCode; //interesting :)
            else return;
            IsChanged = true;
        }

        protected override void OnMouseDoubleClick(int x, int y, MouseButton button)
        {
            IsChanged = false;
            Key = WinKeys.None;
        }
    }
}