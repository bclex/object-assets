using OA.Core;
using OA.Core.Input;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Login.Accounts;
using OA.Ultima.Resources;
using OA.Ultima.UI.Controls;
using System;

namespace OA.Ultima.UI.LoginGumps
{
    class CharacterListGump : Gump
    {
        Action _onBackToSelectServer;
        Action<int> _onDeleteCharacter;
        Action<int> _onLoginWithCharacter;
        Action _onNewCharacter;

        bool _isWorldLoading;
        int _charSelected = -1;
        int _charListUpdate = -1;
        HtmlGumpling[] _characterNames;
        GumpPicTiled _background;

        enum Buttons
        {
            QuitButton,
            BackButton,
            ForwardButton,
            NewCharacterButton,
            DeleteCharacterButton
        }

        public CharacterListGump(Action onBack, Action<int> onLogin, Action<int> onDelete, Action onNew)
            : base(0, 0)
        {
            _onBackToSelectServer = onBack;
            _onLoginWithCharacter = onLogin;
            _onDeleteCharacter = onDelete;
            _onNewCharacter = onNew;

            // get the resource provider
            var provider = Service.Get<IResourceProvider>();

            // backdrop
            AddControl(_background = new GumpPicTiled(this, 0, 0, 800, 600, 9274));
            AddControl(new GumpPic(this, 0, 0, 5500, 0));
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)Buttons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;

            // Page 1 - select a character
            // back button
            AddControl(new Button(this, 586, 435, 5537, 5539, ButtonTypes.Activate, 0, (int)Buttons.BackButton), 1);
            ((Button)LastControl).GumpOverID = 5538;
            // forward button
            AddControl(new Button(this, 610, 435, 5540, 5542, ButtonTypes.Activate, 0, (int)Buttons.ForwardButton), 1);
            ((Button)LastControl).GumpOverID = 5541;
            // center message window backdrop
            AddControl(new ResizePic(this, 160, 70, 2600, 408, 390), 1);
            AddControl(new TextLabelAscii(this, 266, 112, 2, 2016, provider.GetString(3000050)), 1);
            // delete button
            AddControl(new Button(this, 224, 398, 5530, 5532, ButtonTypes.Activate, 0, (int)Buttons.DeleteCharacterButton), 1);
            ((Button)LastControl).GumpOverID = 5531;
            // new button
            AddControl(new Button(this, 442, 398, 5533, 5535, ButtonTypes.Activate, 0, (int)Buttons.NewCharacterButton), 1);
            ((Button)LastControl).GumpOverID = 5534;

            // Page 2 - logging in to server
            // center message window backdrop
            AddControl(new ResizePic(this, 116, 95, 2600, 408, 288), 2);
            AddControl(new TextLabelAscii(this, 166, 143, 2, 2016, provider.GetString(3000001)), 2);

            IsUncloseableWithRMB = true;
        }

        public override void Update(double totalMS, double frameMS)
        {
            var sb = Service.Get<SpriteBatch3D>();
            if (_background.Width != sb.GraphicsDevice.Viewport.Width || _background.Height != sb.GraphicsDevice.Viewport.Height)
            {
                _background.Width = sb.GraphicsDevice.Viewport.Width;
                _background.Height = sb.GraphicsDevice.Viewport.Height;
            }

            if (Characters.UpdateValue != _charListUpdate)
            {
                int entryIndex = 0;
                _characterNames = new HtmlGumpling[Characters.Length];
                foreach (var e in Characters.List)
                {
                    if (e.Name != string.Empty)
                    {
                        _characterNames[entryIndex] = new HtmlGumpling(this, 228, 154 + 40 * entryIndex, 272, 22, 0, 0, formatHTMLCharName(entryIndex, e.Name, (_charSelected == entryIndex ? 431 : 1278)));
                        AddControl(new ResizePic(this, _characterNames[entryIndex]), 1);
                        AddControl(_characterNames[entryIndex], 1);
                    }
                    entryIndex++;
                }
                _charListUpdate = Characters.UpdateValue;
            }

            var input = Service.Get<IInputService>();
            if (input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Enter, false, false, false) && !_isWorldLoading)
            {
                if (_characterNames.Length > 0)
                {
                    _onLoginWithCharacter(0);
                    _isWorldLoading = true;
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void OnButtonClick(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.QuitButton:
                    Service.Get<UltimaGame>().Quit();
                    break;
                case Buttons.BackButton:
                    _onBackToSelectServer();
                    break;
                case Buttons.ForwardButton:
                    _onLoginWithCharacter(_charSelected);
                    break;
                case Buttons.NewCharacterButton:
                    _onNewCharacter();
                    break;
                case Buttons.DeleteCharacterButton:
                    _onDeleteCharacter(_charSelected);
                    break;
            }
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            int charIndex;
            if (href.Length > 5 && href.StartsWith("CHAR="))
                charIndex = int.Parse(href.Substring(5));
            else
                return;

            if (e == MouseEvent.Click)
            {
                if (href.Length > 5 && href.StartsWith("CHAR="))
                {
                    if ((_charSelected >= 0) && (_charSelected < Characters.Length))
                        _characterNames[_charSelected].Text = formatHTMLCharName(_charSelected, Characters.List[_charSelected].Name, 1278);
                    _charSelected = charIndex;
                    if ((_charSelected >= 0) && (_charSelected < Characters.Length))
                        _characterNames[_charSelected].Text = formatHTMLCharName(_charSelected, Characters.List[_charSelected].Name, 431);
                }
            }
            else if (e == MouseEvent.DoubleClick)
            {
                if (charIndex == _charSelected)
                    _onLoginWithCharacter(charIndex);
            }
        }

        string formatHTMLCharName(int index, string name, int hue)
        {
            // add a single char to the left so the width doesn't change.
            return string.Format("<left> </left><center><big><a href=\"CHAR={0}\" color='#543' hovercolor='#345' activecolor='#222' style=\"text-decoration: none\">{1}</a></big></center>",
                index, name);
        }
    }
}
