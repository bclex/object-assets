using OA.Core.Input;
using OA.Core.UI;
using OA.Core.Windows;
using OA.Ultima.Core.Graphics;
using System;
using UnityEngine;

namespace OA.Ultima.UI.Controls
{
    class TextEntryPage : AControl
    {
        const float MSBetweenCaratBlinks = 500f;
        
        string _text;
        bool _isFocused;
        bool _caratBlinkOn;
        float _msSinceLastCaratBlink;
        readonly RenderedText _renderedText;
        RenderedText _renderedCarat;
        int _caratAt;
        int? _caratKeyUpDownX;
        Action<int, string> _onPageOverflow;
        Action<int> _onPageUnderflow;
        Action<int> _onPreviousPage;
        Action<int> _onNextPage;

        public int PageIndex;
        public string LeadingHtmlTag;
        public string Text
        {
            get { return _text == null ? string.Empty : _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    _renderedText.Text = $"{LeadingHtmlTag}{Text}";
                }
            }
        }

        public string TextWithLineBreaks
        {
            get
            {
                if (string.IsNullOrEmpty(_text))
                    return string.Empty;
                return _renderedText.Document.TextWithLineBreaks;
            }
        }

        public int CaratAt
        {
            get
            {
                if (_caratAt < 0)
                    _caratAt = 0;
                if (Text != null && _caratAt > Text.Length)
                    _caratAt = Text.Length;
                return _caratAt;
            }
            set
            {
                _caratAt = value;
                if (_caratAt < 0)
                    _caratAt = 0;
                if (_caratAt > Text.Length)
                    _caratAt = Text.Length;
            }
        }

        public override bool HandlesMouseInput => base.HandlesMouseInput & IsEditable;
        public override bool HandlesKeyboardFocus => base.HandlesKeyboardFocus & IsEditable;

        // ============================================================================================================
        // Ctors and BuildGumpling Methods
        // ============================================================================================================

        public TextEntryPage(AControl parent, int x, int y, int width, int height, int pageIndex)
            : base(parent)
        {
            base.HandlesMouseInput = true;
            base.HandlesKeyboardFocus = true;
            IsEditable = true;
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);
            PageIndex = pageIndex;
            _caratBlinkOn = false;
            _renderedText = new RenderedText(string.Empty, Width, true);
            _renderedCarat = new RenderedText(string.Empty, 16, true);
        }

        public void SetMaxLines(int maxLines, Action<int, string> onPageOverflow, Action<int> onPageUnderflow)
        {
            _renderedText.Document.SetMaxLines(maxLines, OnDocumentSplitPage);
            _onPageOverflow = onPageOverflow;
            _onPageUnderflow = onPageUnderflow;
        }

        public void SetKeyboardPageControls(Action<int> onPrevious, Action<int> onNext)
        {
            _onPreviousPage = onPrevious;
            _onNextPage = onNext;
        }

        void OnDocumentSplitPage(int index)
        {
            string overflowText = Text.Substring(index);
            _text = Text.Substring(0, index);
            _onPageOverflow?.Invoke(PageIndex, overflowText);
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
                _msSinceLastCaratBlink += (float)frameMS;
                if (_msSinceLastCaratBlink >= MSBetweenCaratBlinks)
                {
                    _msSinceLastCaratBlink -= MSBetweenCaratBlinks;
                    if (_caratBlinkOn == true) _caratBlinkOn = false;
                    else _caratBlinkOn = true;
                }
            }
            else
            {
                _isFocused = false;
                _caratBlinkOn = false;
            }
            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            _renderedText.Draw(spriteBatch, new RectInt(position.x, position.y, Width, Height), 0, 0);
            if (IsEditable)
            {
                _renderedCarat.Text = $"{LeadingHtmlTag}|";
                _renderedText.Draw(spriteBatch, position);
                if (_caratBlinkOn)
                {
                    var caratPosition = _renderedText.Document.GetCaratPositionByIndex(_caratAt);
                    caratPosition.X += position.x;
                    caratPosition.Y += position.y;
                    _renderedCarat.Draw(spriteBatch, caratPosition);
                }
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        void SetBlinkOn()
        {
            _caratBlinkOn = true;
            _msSinceLastCaratBlink = 0;
        }

        // ============================================================================================================
        // Text Editing Functions
        // ============================================================================================================

        public void InsertCharacter(int index, char ch)
        {
            string escapedCharacter;
            int caratAt = index;
            string text = Text;
            if (EscapeCharacters.TryMatchChar(ch, out escapedCharacter))
            {
                text = text.Insert(CaratAt, escapedCharacter);
                caratAt += escapedCharacter.Length;
            }
            else
            {
                text = text.Insert(CaratAt, ch.ToString());
                caratAt += 1;
            }
            Text = text;
            CaratAt = caratAt;
        }

        public void RemoveCharacter(int index)
        {
            int escapedLength;
            if (EscapeCharacters.TryFindEscapeCharacterBackwards(Text, index, out escapedLength))
            {
                Text = Text.Substring(0, Text.Length - escapedLength);
                var carat = index - 1;
                var before = index == 0 ? null : Text.Substring(0, index - 1);
                var after = Text.Substring(index);
                Text = before + after;
                CaratAt = carat;
            }
            else
            {
                var carat = index - 1;
                var before = index == 0 ? null : Text.Substring(0, index - 1);
                var after = Text.Substring(index);
                Text = before + after;
                CaratAt = carat;
            }
        }

        // ============================================================================================================
        // Input
        // ============================================================================================================

        protected override void OnKeyboardInput(InputEventKeyboard e)
        {
            if (e.KeyCode == WinKeys.Up || e.KeyCode == WinKeys.Down)
            {
                var current = _renderedText.Document.GetCaratPositionByIndex(CaratAt);
                if (_caratKeyUpDownX == null)
                    _caratKeyUpDownX = current.X;
                var next = new Vector2Int(_caratKeyUpDownX.Value, current.Y + (e.KeyCode == WinKeys.Up ? -18 : 18));
                var carat = _renderedText.Document.GetCaratIndexByPosition(next);
                if (carat != -1)
                    CaratAt = carat;
            }
            else
            {
                _caratKeyUpDownX = null;
                switch (e.KeyCode)
                {
                    case WinKeys.Tab:
                        Parent.KeyboardTabToNextFocus(this);
                        break;
                    case WinKeys.Enter:
                        InsertCharacter(CaratAt, '\n');
                        break;
                    case WinKeys.Back:
                        if (CaratAt == 0)
                            _onPageUnderflow.Invoke(PageIndex);
                        else RemoveCharacter(CaratAt);
                        break;
                    case WinKeys.Left:
                        if (CaratAt == 0)
                            _onPreviousPage?.Invoke(PageIndex);
                        else CaratAt = CaratAt - 1;
                        break;
                    case WinKeys.Right:
                        if (CaratAt == Text.Length)
                            _onNextPage?.Invoke(PageIndex);
                        else CaratAt = CaratAt + 1;
                        break;
                    default:
                        if (e.IsChar && e.KeyChar >= 32)
                            InsertCharacter(CaratAt, e.KeyChar);
                        break;
                }
            }
            SetBlinkOn();
        }

        protected override void OnMouseClick(int x, int y, MouseButton button)
        {
            if (button == MouseButton.Left)
            {
                var carat = _renderedText.Document.GetCaratIndexByPosition(new Vector2Int(x, y));
                if (carat != -1)
                {
                    CaratAt = carat;
                    SetBlinkOn();
                    return;
                }
            }
            base.OnMouseClick(x, y, button);
        }
    }
}