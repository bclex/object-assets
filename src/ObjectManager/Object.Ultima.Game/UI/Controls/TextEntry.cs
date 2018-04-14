using OA.Core.Input;
using OA.Core.UI;
using OA.Core.Windows;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class TextEntry : AControl
    {
        const float MSBetweenCaratBlinks = 500f;
        
        bool _isFocused;
        bool _caratBlinkOn;
        float _msSinceLastCaratBlink;
        RenderedText _renderedText;
        RenderedText _renderedCarat;

        public int Hue;
        public int EntryID;
        public int MaxCharCount;
        public bool IsPasswordField;
        public bool ReplaceDefaultTextOnFirstKeypress;
        public bool NumericOnly;
        public string LeadingHtmlTag;
        public string LeadingText;
        public string Text;
        public bool LegacyCarat;

        public override bool HandlesMouseInput => base.HandlesMouseInput & IsEditable;
        public override bool HandlesKeyboardFocus => base.HandlesKeyboardFocus & IsEditable;

        // ============================================================================================================
        // Ctors and BuildGumpling Methods
        // ============================================================================================================

        public TextEntry(AControl parent, string[] arguements, string[] lines)
            : this(parent)
        {
            var maxCharCount = 0;
            var x = int.Parse(arguements[1]);
            var y = int.Parse(arguements[2]);
            var width = int.Parse(arguements[3]);
            var height = int.Parse(arguements[4]);
            var hue = ServerRecievedHueTransform(int.Parse(arguements[5]));
            var entryID = int.Parse(arguements[6]);
            var textIndex = int.Parse(arguements[7]);
            if (arguements[0] == "textentrylimited")
                maxCharCount = int.Parse(arguements[8]);
            BuildGumpling(x, y, width, height, hue, entryID, maxCharCount, lines[textIndex]);
        }

        public TextEntry(AControl parent, int x, int y, int width, int height, int hue, int entryID, int maxCharCount, string text)
            : this(parent)
        {
            BuildGumpling(x, y, width, height, hue, entryID, maxCharCount, text);
        }

        TextEntry(AControl parent)
            : base(parent)
        {
            base.HandlesMouseInput = true;
            base.HandlesKeyboardFocus = true;
            IsEditable = true;
        }

        void BuildGumpling(int x, int y, int width, int height, int hue, int entryID, int maxCharCount, string text)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            Hue = hue;
            EntryID = entryID;
            Text = text == null ? string.Empty : text;
            MaxCharCount = maxCharCount;
            _caratBlinkOn = false;
            _renderedText = new RenderedText(string.Empty, 2048, true);
            _renderedCarat = new RenderedText(string.Empty, 16, true);
        }

        // ============================================================================================================
        // Update and Draw
        // ============================================================================================================

        public override void Update(double totalMS, double frameMS)
        {
            if (UserInterface.KeyboardFocusControl == this)
            {
                // if we're not already focused, turn the carat on immediately.
                if (!_isFocused)
                {
                    _isFocused = true;
                    _caratBlinkOn = true;
                    _msSinceLastCaratBlink = 0f;
                }
                // if we're using the legacy carat, keep it visible. Else blink it every x seconds.
                if (LegacyCarat)
                    _caratBlinkOn = true;
                else
                {
                    _msSinceLastCaratBlink += (float)frameMS;
                    if (_msSinceLastCaratBlink >= MSBetweenCaratBlinks)
                    {
                        _msSinceLastCaratBlink -= MSBetweenCaratBlinks;
                        if (_caratBlinkOn == true) _caratBlinkOn = false;
                        else _caratBlinkOn = true;
                    }
                }
            }
            else
            {
                _isFocused = false;
                _caratBlinkOn = false;
            }
            var text = Text == null ? null : (IsPasswordField ? new string('*', Text.Length) : Text);
            _renderedText.Text = $"{LeadingHtmlTag}{LeadingText}{text}";
            _renderedCarat.Text = $"{LeadingHtmlTag}{(LegacyCarat ? "_" : "|")}";
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var caratPosition = new Vector2Int(position.x, position.y);
            if (IsEditable)
            {
                if (_renderedText.Width + _renderedCarat.Width <= Width)
                {
                    _renderedText.Draw(spriteBatch, position, Utility.GetHueVector(Hue));
                    caratPosition.x += _renderedText.Width;
                }
                else
                {
                    int textOffset = _renderedText.Width - (Width - _renderedCarat.Width);
                    _renderedText.Draw(spriteBatch, new RectInt(position.x, position.y, _renderedText.Width - textOffset, _renderedText.Height), textOffset, 0, Utility.GetHueVector(Hue));
                    caratPosition.x += (Width - _renderedCarat.Width);
                }
            }
            else
            {
                caratPosition.x = 0;
                _renderedText.Draw(spriteBatch, new RectInt(position.x, position.y, Width, Height), 0, 0, Utility.GetHueVector(Hue));
            }
            if (_caratBlinkOn)
                _renderedCarat.Draw(spriteBatch, caratPosition, Utility.GetHueVector(Hue));
            base.Draw(spriteBatch, position, frameMS);
        }

        // ============================================================================================================
        // Input
        // ============================================================================================================

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            switch (e.KeyCode)
            {
                case WinKeys.Tab:
                    Parent.KeyboardTabToNextFocus(this);
                    break;
                case WinKeys.Enter:
                    Parent.OnKeyboardReturn(EntryID, Text);
                    break;
                case WinKeys.Back:
                    if (ReplaceDefaultTextOnFirstKeypress)
                    {
                        Text = string.Empty;
                        ReplaceDefaultTextOnFirstKeypress = false;
                    }
                    else if (!string.IsNullOrEmpty(Text))
                    {
                        int escapedLength;
                        if (EscapeCharacters.TryFindEscapeCharacterBackwards(Text, Text.Length - 1, out escapedLength))
                            Text = Text.Substring(0, Text.Length - escapedLength);
                        else Text = Text.Substring(0, Text.Length - 1);
                    }
                    break;
                default:
                    // place a char, so long as it's within the char count limit.
                    if (MaxCharCount != 0 && Text.Length >= MaxCharCount)
                        return;
                    if (NumericOnly && !char.IsNumber(e.KeyChar))
                        return;
                    if (ReplaceDefaultTextOnFirstKeypress)
                    {
                        Text = string.Empty;
                        ReplaceDefaultTextOnFirstKeypress = false;
                    }
                    if (e.IsChar && e.KeyChar >= 32)
                    {
                        string escapedCharacter;
                        if (EscapeCharacters.TryMatchChar(e.KeyChar, out escapedCharacter))
                            Text += escapedCharacter;
                        else Text += e.KeyChar;
                    }
                    break;
            }
        }
    }
}