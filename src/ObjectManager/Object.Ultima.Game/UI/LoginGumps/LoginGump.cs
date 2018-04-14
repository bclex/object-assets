using OA.Core;
using OA.Ultima.Resources;
using OA.Ultima.UI.Controls;
using System;
using System.Collections.Generic;
using System.Security;

namespace OA.Ultima.UI.LoginGumps
{
    public class LoginGump : Gump
    {
        static readonly List<Tuple<string, Action>> _buttons = new List<Tuple<string, Action>>();

        public static void AddButton(string caption, Action onClick)
        {
            _buttons.Add(new Tuple<string, Action>(caption, onClick));
        }

        public static void RemoveButton(Action onClick)
        {
            for (var i = 0; i < _buttons.Count; i++)
                if (_buttons[i].Item2 == onClick)
                    _buttons.RemoveAt(i--);
        }

        Action<string, int, string, SecureString> _onLogin;

        public LoginGump(Action<string, int, string, SecureString> onLogin)
            : base(0, 0)
        {
            _onLogin = onLogin;
            // get the resource provider
            var provider = Service.Get<IResourceProvider>();
            int hue = 902; // dark grey
            // backdrop
            AddControl(new GumpPicTiled(this, 0, 0, 800, 600, 0x0588));
            AddControl(new ResizePic(this, 96, 285, 0x13BE, 492, 190));
            AddControl(new GumpPic(this, 0, 0, 0x157C, 0)); // 0x2329 - upper-left border graphic
            AddControl(new GumpPic(this, 294, 42, 0x058A, 0)); // 0x2329 - castle graphic
            // quit button
            AddControl(new Button(this, 554, 2, 5513, 5515, ButtonTypes.Activate, 0, (int)LoginGumpButtons.QuitButton));
            ((Button)LastControl).GumpOverID = 5514;
            // Log in to Ultima Online
            AddControl(new TextLabelAscii(this, 230, 305, 2, hue, provider.GetString(3000038)));
            // Account Name
            AddControl(new TextLabelAscii(this, 181, 346, 2, hue, provider.GetString(3000099)));
            // Password
            AddControl(new TextLabelAscii(this, 181, 386, 2, hue, provider.GetString(3000103)));
            // name field
            var g1 = new TextEntry(this, 332, 346, 200, 20, 0, (int)LoginGumpTextFields.AccountName, 32, Settings.Login.UserName);
            g1.LeadingHtmlTag = "<basefont color=#000000><big>";
            AddControl(new ResizePic(this, g1));
            AddControl(g1);
            // password field
            var g2 = new TextEntry(this, 332, 386, 200, 20, 0, (int)LoginGumpTextFields.Password, 32, "");
            g2.IsPasswordField = true;
            g2.LeadingHtmlTag = "<basefont color=#000000><big>";
            AddControl(new ResizePic(this, g2));
            AddControl(g2);
            // login button
            AddControl(new Button(this, 550, 439, 5540, 5542, ButtonTypes.Activate, 0, (int)LoginGumpButtons.LoginButton));
            ((Button)LastControl).GumpOverID = 5541;
            // Version information
            AddControl(new HtmlGumpling(this, 120, 440, 400, 20, 0, 0, $"<center><medium><outline><font color='#DDDDDD'>{Utility.VersionString}</center></medium></outline>"));
            // flag graphic
            AddControl(new GumpPic(this, 0, 0, 0x15A0, 0));
            // buttons on the left side
            AddControl(new ButtonResizable(this, 10, 450, 100, 23, "CREDITS", OnClickCredits));
            var y = 420;
            foreach (var button in _buttons)
            {
                AddControl(new ButtonResizable(this, 10, y, 100, 23, button.Item1, button.Item2));
                y -= 30;
            }
            IsUncloseableWithRMB = true;
        }

        public override void OnButtonClick(int buttonID)
        {
            var accountName = GetTextEntry((int)LoginGumpTextFields.AccountName);
            var password = GetTextEntry((int)LoginGumpTextFields.Password);

            switch ((LoginGumpButtons)buttonID)
            {
                case LoginGumpButtons.QuitButton:
                    Service.Get<UltimaGame>().Quit();
                    break;
                case LoginGumpButtons.LoginButton:
                    {
                        var secureStr = new SecureString();
                        if (password.Length > 0)
                            foreach (char c in password)
                                secureStr.AppendChar(c);
                        _onLogin(UltimaGameSettings.Login.ServerAddress, UltimaGameSettings.Login.ServerPort, accountName, secureStr);
                        break;
                    }
            }
            UltimaGameSettings.Login.UserName = accountName;
        }

        public override void OnKeyboardReturn(int textID, string text)
        {
            OnButtonClick((int)LoginGumpButtons.LoginButton);
        }

        void OnClickCredits()
        {
            UserInterface.AddControl(new CreditsGump(), 0, 0);
        }

        enum LoginGumpButtons
        {
            QuitButton = 0,
            LoginButton = 1
        }
        enum LoginGumpTextFields
        {
            AccountName = 0,
            Password = 1
        }
    }
}
