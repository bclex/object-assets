using OA.Core;
using OA.Core.Input;
using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.Core.Graphics;
using OA.Ultima.Core.Network;
using OA.Ultima.Data;
using OA.Ultima.Network.Client;
using OA.Ultima.Player;
using OA.Ultima.Resources;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    class ChatControl : AControl
    {
        const int MaxChatMessageLength = 96;

        TextEntry _textEntry;
        List<ChatLineTimed> _textEntries;
        List<Tuple<ChatMode, string>> _messageHistory;
        IInputService _input;
        WorldModel _world;
        int _messageHistoryIndex = -1;
        Serial _privateMessageSerial = Serial.Null;
        string _privateMessageName;

        ChatMode _mode = ChatMode.Default;
        ChatMode Mode
        {
            get { return _mode; }
            set
            {
                _mode = value;
                switch (value)
                {
                    case ChatMode.Default:
                        _textEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(UltimaGameSettings.UserInterface.SpeechColor, 1)));
                        _textEntry.LeadingText = string.Empty;
                        _textEntry.Text = string.Empty;
                        break;
                    case ChatMode.Whisper:
                        _textEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(UltimaGameSettings.UserInterface.SpeechColor, 1)));
                        _textEntry.LeadingText = "Whisper: ";
                        _textEntry.Text = string.Empty;
                        break;
                    case ChatMode.Emote:
                        _textEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(UltimaGameSettings.UserInterface.EmoteColor, 1)));
                        _textEntry.LeadingText = "Emote: ";
                        _textEntry.Text = string.Empty;
                        break;
                    case ChatMode.Party:
                        _textEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(UltimaGameSettings.UserInterface.PartyMsgColor, 1)));
                        _textEntry.LeadingText = "Party: ";
                        _textEntry.Text = string.Empty;
                        break;
                    case ChatMode.PartyPrivate:
                        _textEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(UltimaGameSettings.UserInterface.PartyPrivateMsgColor, 1)));
                        _textEntry.LeadingText = $"To {_privateMessageName}: ";
                        _textEntry.Text = string.Empty;
                        break;
                    case ChatMode.Guild:
                        _textEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(UltimaGameSettings.UserInterface.GuildMsgColor, 1)));
                        _textEntry.LeadingText = "Guild: ";
                        _textEntry.Text = string.Empty;
                        break;
                    case ChatMode.Alliance:
                        _textEntry.LeadingHtmlTag = string.Format("<outline color='#{0}' style='font-family: uni0;'>",
                            Utility.GetColorFromUshort(Resources.HueData.GetHue(UltimaGameSettings.UserInterface.AllianceMsgColor, 1)));
                        _textEntry.LeadingText = "Alliance: ";
                        _textEntry.Text = string.Empty;
                        break;
                }
            }
        }

        public ChatControl(AControl parent, int x, int y, int width, int height)
            : base(parent)
        {
            Position = new Vector2Int(x, y);
            Size = new Vector2Int(width, height);

            _textEntries = new List<ChatLineTimed>();
            _messageHistory = new List<Tuple<ChatMode, string>>();

            _input = Service.Get<IInputService>();
            _world = Service.Get<WorldModel>();

            IsUncloseableWithRMB = true;
        }

        public void SetModeToPartyPrivate(string name, Serial serial)
        {
            _privateMessageName = name;
            _privateMessageSerial = serial;
            Mode = ChatMode.PartyPrivate;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_textEntry == null)
            {
                var provider = Service.Get<IResourceProvider>();
                var font = provider.GetUnicodeFont(0);
                _textEntry = new TextEntry(this, 1, Height - font.Height, Width, font.Height, 0, 0, MaxChatMessageLength, string.Empty);
                _textEntry.LegacyCarat = true;
                Mode = ChatMode.Default;

                AddControl(new CheckerTrans(this, 0, Height - 20, Width, 20));
                AddControl(_textEntry);
            }

            for (var i = 0; i < _textEntries.Count; i++)
            {
                _textEntries[i].Update(totalMS, frameMS);
                if (_textEntries[i].IsExpired)
                {
                    _textEntries[i].Dispose();
                    _textEntries.RemoveAt(i);
                    i--;
                }
            }

            // Ctrl-Q = Cycle backwards through the things you have said today
            // Ctrl-W = Cycle forwards through the things you have said today
            if (_input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Q, false, false, true) && _messageHistoryIndex > -1)
            {
                if (_messageHistoryIndex > 0)
                    _messageHistoryIndex -= 1;
                {
                    Mode = _messageHistory[_messageHistoryIndex].Item1;
                    _textEntry.Text = _messageHistory[_messageHistoryIndex].Item2;
                }
            }
            else if (_input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.W, false, false, true))
            {
                if (_messageHistoryIndex < _messageHistory.Count - 1)
                {
                    _messageHistoryIndex += 1;
                    Mode = _messageHistory[_messageHistoryIndex].Item1;
                    _textEntry.Text = _messageHistory[_messageHistoryIndex].Item2;
                }
                else
                    _textEntry.Text = string.Empty;
            }
            // backspace when mode is not default and Text is empty = clear mode.
            else if (_input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Back, false, false, false) && _textEntry.Text == string.Empty)
            {
                Mode = ChatMode.Default;
            }

            // only switch mode if the single command char is the only char entered.
            if ((Mode == ChatMode.Default && _textEntry.Text.Length == 1) ||
                (Mode != ChatMode.Default && _textEntry.Text.Length == 1))
            {
                switch (_textEntry.Text[0])
                {
                    case ':':
                        Mode = ChatMode.Emote;
                        break;
                    case ';':
                        Mode = ChatMode.Whisper;
                        break;
                    case '/':
                        Mode = ChatMode.Party;
                        break;
                    case '\\':
                        Mode = ChatMode.Guild;
                        break;
                    case '|':
                        Mode = ChatMode.Alliance;
                        break;
                }
            }

            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Vector2Int position, double frameMS)
        {
            var y = _textEntry.y + position.y - 6;
            for (var i = _textEntries.Count - 1; i >= 0; i--)
            {
                y -= _textEntries[i].TextHeight;
                _textEntries[i].Draw(spriteBatch, new Vector2Int(position.x + 2, y));
            }
            base.Draw(spriteBatch, position, frameMS);
        }

        public override void OnKeyboardReturn(int textID, string text)
        {
            // local variables
            var sentMode = Mode;
            var speechType = MessageTypes.Normal;
            var hue = 0;
            // save this message and reset chat for next entry
            _textEntry.Text = string.Empty;
            _messageHistory.Add(new Tuple<ChatMode, string>(Mode, text));
            _messageHistoryIndex = _messageHistory.Count;
            Mode = ChatMode.Default;
            // send the message and display it locally.
            switch (sentMode)
            {
                case ChatMode.Default:
                    speechType = MessageTypes.Normal;
                    hue = UltimaGameSettings.UserInterface.SpeechColor;
                    break;
                case ChatMode.Whisper:
                    speechType = MessageTypes.Whisper;
                    hue = UltimaGameSettings.UserInterface.SpeechColor;
                    break;
                case ChatMode.Emote:
                    speechType = MessageTypes.Emote;
                    hue = UltimaGameSettings.UserInterface.EmoteColor;
                    break;
                case ChatMode.Party:
                    PlayerState.Partying.DoPartyCommand(text);
                    return;
                case ChatMode.PartyPrivate:
                    PlayerState.Partying.SendPartyPrivateMessage(_privateMessageSerial, text);
                    return;
                case ChatMode.Guild:
                    speechType = MessageTypes.Guild;
                    hue = UltimaGameSettings.UserInterface.GuildMsgColor;
                    break;
                case ChatMode.Alliance:
                    speechType = MessageTypes.Alliance;
                    hue = UltimaGameSettings.UserInterface.AllianceMsgColor;
                    break;
            }
            var network = Service.Get<INetworkClient>();
            network.Send(new AsciiSpeechPacket(speechType, 0, hue + 2, "ENU", text));
        }

        public void AddLine(string text, int font, int hue, bool asUnicode)
        {
            _textEntries.Add(new ChatLineTimed(string.Format("<outline color='#{3}' style='font-family:{1}{2};'>{0}",
                text, asUnicode ? "uni" : "ascii", font, Utility.GetColorFromUshort(Resources.HueData.GetHue(hue, -1))),
                Width));
        }

        class ChatLineTimed
        {
            readonly string _text;
            public string Text { get { return _text; } }
            float _createdTime = float.MinValue;
            bool _isExpired;
            public bool IsExpired { get { return _isExpired; } }
            float _alpha;
            public float Alpha { get { return _alpha; } }
            private int _width;

            const float Time_Display = 10000.0f;
            const float Time_Fadeout = 4000.0f;

            RenderedText _texture;
            public int TextHeight { get { return _texture.Height; } }

            public ChatLineTimed(string text, int width)
            {
                _text = text;
                _isExpired = false;
                _alpha = 1.0f;
                _width = width;

                _texture = new RenderedText(_text, _width);
            }

            public void Update(double totalMS, double frameMS)
            {
                if (_createdTime == float.MinValue)
                    _createdTime = (float)totalMS;
                var time = (float)totalMS - _createdTime;
                if (time > Time_Display)
                    _isExpired = true;
                else if (time > (Time_Display - Time_Fadeout))
                    _alpha = 1.0f - ((time) - (Time_Display - Time_Fadeout)) / Time_Fadeout;
            }

            public void Draw(SpriteBatchUI sb, Vector2Int position)
            {
                _texture.Draw(sb, position, Utility.GetHueVector(0, false, (_alpha < 1.0f), true));
            }

            public void Dispose()
            {
                _texture = null;
            }

            public override string ToString()
            {
                return _text;
            }
        }
    }
}
