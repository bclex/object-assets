using OA.Core;
using OA.Core.UI;
using OA.Ultima.Core;
using OA.Ultima.UI.Controls;
using OA.Ultima.World;
using UnityEngine;

namespace OA.Ultima.UI.WorldGumps
{
    public class OptionsGump : Gump
    {
        UserInterfaceService _userInterface;
        WorldModel _world;
        HSliderBar _musicVolume;
        HSliderBar _soundVolume;
        CheckBox _musicOn;
        CheckBox _soundOn;
        CheckBox _footStepSoundOn;
        CheckBox _alwaysRun;
        CheckBox _menuBarDisabled;

        CheckBox _isVSyncEnabled;
        CheckBox _showFps;
        CheckBox _isConsoleEnabled;
        ColorPickerBox _speechColor;
        ColorPickerBox _emoteColor;
        ColorPickerBox _partyMsgColor;
        ColorPickerBox _guildMsgColor;
        CheckBox _ignoreGuildMsg;
        ColorPickerBox _allianceMsgColor;
        CheckBox _ignoreAllianceMsg;
        CheckBox _crimeQuery;

        DropDownList _dropDownFullScreenResolutions;
        DropDownList _dropDownPlayWindowResolutions;

        // MACROS
        const int MACRO_CAPACITY = 10;
        KeyPressControl _macroKeyPress;
        CheckBox _chkShift;
        CheckBox _chkAlt;
        CheckBox _chkCtrl;
        MacroDropDownList[] _actionTypeList = new MacroDropDownList[MACRO_CAPACITY];
        TextEntry[] _actionText = new TextEntry[MACRO_CAPACITY];
        MacroDropDownList[] _actionDropDown = new MacroDropDownList[MACRO_CAPACITY];
        int _currentMacro;

        double _nextRefreshAt;
        const double REFRESH_INTERVAL = 0.4d;
        TextLabelAscii[] _labels = new TextLabelAscii[2];

        private enum Labels
        {
            SoundVolume,
            MusicVolume
        }

        public OptionsGump()
            : base(0, 0)
        {
            IsMoveable = true;

            AddControl(new ResizePic(this, 40, 0, 2600, 550, 450));
            //left column
            AddControl(new Button(this, 0, 40, 218, 217, ButtonTypes.SwitchPage, 1, (int)Buttons.Sound));
            AddControl(new Button(this, 0, 110, 220, 219, ButtonTypes.SwitchPage, 2, (int)Buttons.Help));
            AddControl(new Button(this, 0, 250, 224, 223, ButtonTypes.SwitchPage, 3, (int)Buttons.Chat));
            AddControl(new Button(this, 0, 320, 237, 236, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros));
            //right column
            AddControl(new Button(this, 576, 40, 226, 225, ButtonTypes.SwitchPage, 5, (int)Buttons.Interface));
            AddControl(new Button(this, 576, 110, 228, 227, ButtonTypes.SwitchPage, 6, (int)Buttons.Display));
            AddControl(new Button(this, 576, 180, 230, 229, ButtonTypes.SwitchPage, 7, (int)Buttons.Reputation));
            AddControl(new Button(this, 576, 250, 232, 231, ButtonTypes.SwitchPage, 8, (int)Buttons.Miscellaneous));
            AddControl(new Button(this, 576, 320, 235, 234, ButtonTypes.SwitchPage, 9, (int)Buttons.Filters));
            //bottom buttons
            AddControl(new Button(this, 140, 410, 243, 241, ButtonTypes.Activate, 0, (int)Buttons.Cancel));
            AddControl(new Button(this, 240, 410, 239, 240, ButtonTypes.Activate, 0, (int)Buttons.Apply));
            AddControl(new Button(this, 340, 410, 246, 244, ButtonTypes.Activate, 0, (int)Buttons.Default));
            AddControl(new Button(this, 440, 410, 249, 248, ButtonTypes.Activate, 0, (int)Buttons.Okay));

            _userInterface = Service.Get<UserInterfaceService>();
            _world = Service.Get<WorldModel>();

            // page 1 Sound and Music
            AddControl(new Button(this, 0, 40, 217, 217, ButtonTypes.SwitchPage, 1, (int)Buttons.Sound), 1);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Sound and Music"), 1);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settings affect the sound and music you will hear while playing Ultima Online."), 1);

            AddControl(new TextLabelAscii(this, 85, 85, 9, 1, @"Sound on/off"), 1);
            _soundOn = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, Settings.Audio.SoundOn, 61), 1);

            AddControl(new TextLabelAscii(this, 60, 110, 9, 1, @"Sound volume"), 1);
            _soundVolume = AddControl<HSliderBar>(new HSliderBar(this, 60, 130, 150, 0, 100, Settings.Audio.SoundVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);
            _labels[(int)Labels.SoundVolume] = AddControl<TextLabelAscii>(new TextLabelAscii(this, 220, 130, 9, 1, Settings.Audio.SoundVolume.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 155, 9, 1, @"Music on/off"), 1);
            _musicOn = AddControl<CheckBox>(new CheckBox(this, 60, 150, 210, 211, Settings.Audio.MusicOn, 62), 1);

            AddControl(new TextLabelAscii(this, 60, 180, 9, 1, @"Music volume"), 1);
            _musicVolume = AddControl<HSliderBar>(new HSliderBar(this, 60, 200, 150, 0, 100, Settings.Audio.MusicVolume, HSliderBarStyle.MetalWidgetRecessedBar), 1);
            _labels[(int)Labels.MusicVolume] = AddControl<TextLabelAscii>(new TextLabelAscii(this, 220, 200, 9, 1, _musicVolume.Value.ToString()), 1);

            AddControl(new TextLabelAscii(this, 85, 225, 9, 1, @"Play footstep sound"), 1);
            _footStepSoundOn = AddControl<CheckBox>(new CheckBox(this, 60, 220, 210, 211, Settings.Audio.FootStepSoundOn, 62), 1);

            // page 2 Pop-up Help
            AddControl(new Button(this, 0, 110, 219, 219, ButtonTypes.SwitchPage, 2, (int)Buttons.Help), 2);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Pop-up Help"), 2);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settings configure the behavior of the pop-up help."), 2);

            // page 3 Chat
            AddControl(new Button(this, 0, 250, 223, 223, ButtonTypes.SwitchPage, 3, (int)Buttons.Chat), 3);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Chat"), 3);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settings affect the interface display for chat system."), 3);

            // page 4 Macro Options
            AddControl(new Button(this, 0, 320, 236, 236, ButtonTypes.SwitchPage, 4, (int)Buttons.Macros), 4);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Macro Options"), 4);
            AddControl(new TextLabelAscii(this, 60, 40, 9, 1, @""), 4);
            AddControl(new Button(this, 180, 60, 2460, 2461, ButtonTypes.Activate, 4, (int)Buttons.MAdd), 4); // add
            AddControl(new Button(this, 234, 60, 2463, 2464, ButtonTypes.Activate, 4, (int)Buttons.MDelete), 4); // delete
            AddControl(new Button(this, 302, 60, 2466, 2467, ButtonTypes.Activate, 4, (int)Buttons.MPrevious), 4); // previous
            AddControl(new Button(this, 386, 60, 2469, 2470, ButtonTypes.Activate, 4, (int)Buttons.MNext), 4); // next
            AddControl(new TextLabelAscii(this, 125, 85, 9, 1, @"Keystroke"), 4);
            
            // key press event
            var myKeyPress = new KeyPressControl(this, 130, 100, 57, 14, 4000, WinKeys.None);
            AddControl(new ResizePic(this, myKeyPress), 4);
            _macroKeyPress = AddControl<KeyPressControl>(myKeyPress, 4);
            ///
            AddControl(new TextLabelAscii(this, 195, 100, 9, 1, @"Key"), 4);

            _chkShift = AddControl<CheckBox>(new CheckBox(this, 260, 90, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //shift
            _chkAlt = AddControl<CheckBox>(new CheckBox(this, 330, 90, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //alt
            _chkCtrl = AddControl<CheckBox>(new CheckBox(this, 400, 90, 2151, 2153, Settings.Audio.FootStepSoundOn, 62), 4); //ctrl
            AddControl(new TextLabelAscii(this, 285, 90, 9, 1, @"Shift"), 4);
            AddControl(new TextLabelAscii(this, 355, 90, 9, 1, @"Alt"), 4);
            AddControl(new TextLabelAscii(this, 425, 90, 9, 1, @"Ctrl"), 4);

            AddControl(new TextLabelAscii(this, 180, 135, 9, 1, @"ACTION"), 4);
            AddControl(new TextLabelAscii(this, 420, 135, 9, 1, @"VALUE"), 4);
            
            // macro's action type and controlling another dropdown list for visual
            var y = 0;
            for (var i = 0; i < MACRO_CAPACITY; i++)
            {
                //number of action
                AddControl(new TextLabelAscii(this, 84, 155 + y, 9, 1, (i + 1).ToString()), 4);
                
                // action dropdown list (i need ID variable for find in controls)
                _actionTypeList[i] = AddControl<MacroDropDownList>(new MacroDropDownList(
                    this, 100, 150 + y, 215, Utility.CreateStringLinesFromList(Macros.Types), 10, 0, false, (i + 1000), true), 4);
                
                // value dropdown list (i need ID variable for find in controls)
                _actionDropDown[i] = AddControl<MacroDropDownList>(new MacroDropDownList(
                    this, 330, 150 + y, 190, new string[] { }, 10, 0, false, (i + 2000), false), 4);
                
                //visual control about resizable picture
                _actionDropDown[i].IsVisible = false;
                
                //here is textentry for example: Say,Emote,Yell (i need ID variable for find in controls)
                _actionText[i] = AddControl<TextEntry>(new TextEntry(this, 340, 150 + y, 160, 20, 1, (3000 + i), 80, string.Empty), 4);
                _actionText[i].IsEditable = false;
                _actionText[i].IsVisible = false;
                y += 25;
            }

            // page 5 Interface
            AddControl(new Button(this, 576, 40, 225, 225, ButtonTypes.SwitchPage, 5, (int)Buttons.Interface), 5);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Interface"), 5);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These options affect your interface."), 5);

            AddControl(new TextLabelAscii(this, 85, 85, 9, 1, @"Your character will always run if this is checked"), 5);
            _alwaysRun = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, UltimaGameSettings.UserInterface.AlwaysRun, 61), 5);

            AddControl(new TextLabelAscii(this, 85, 115, 9, 1, @"Disable the Menu Bar"), 5);
            _menuBarDisabled = AddControl<CheckBox>(new CheckBox(this, 60, 110, 210, 211, UltimaGameSettings.UserInterface.MenuBarDisabled, 61), 5);

            // page 6 Display
            AddControl(new Button(this, 576, 110, 227, 227, ButtonTypes.SwitchPage, 6, (int)Buttons.Display), 6);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Display"), 6);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These options affect your display, and adjusting some of them may improve your graphics performance.", 430), 6);

            AddControl(new TextLabelAscii(this, 85, 80, 9, 1, @"Enable vertical synchronization"), 6);
            _isVSyncEnabled = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, UltimaGameSettings.Engine.IsVSyncEnabled, 61), 6);

            AddControl(new TextLabelAscii(this, 85, 100, 9, 1, @"Some unused option"), 6);
            AddControl(new CheckBox(this, 60, 100, 210, 211, false, 62), 6);

            AddControl(new TextLabelAscii(this, 85, 120, 9, 1, @"Use full screen display"), 6);
            AddControl(new CheckBox(this, 60, 120, 210, 211, UltimaGameSettings.UserInterface.IsMaximized, 61), 6);

            AddControl(new TextLabelAscii(this, 60, 150, 9, 1, @"Full Screen Resolution:"), 6);
            _dropDownFullScreenResolutions = AddControl<DropDownList>(new DropDownList(this, 60, 165, 122, Utility.CreateStringLinesFromList(Resolutions.FullScreenResolutionsList), 10, GetCurrentFullScreenIndex(), false), 6);

            AddControl(new TextLabelAscii(this, 60, 190, 9, 1, @"Play Window Resolution:"), 6);
            _dropDownPlayWindowResolutions = AddControl<DropDownList>(new DropDownList(this, 60, 205, 122, Utility.CreateStringLinesFromList(Resolutions.FullScreenResolutionsList), 10, GetCurrentPlayWindowIndex(), false), 6);

            AddControl(new TextLabelAscii(this, 85, 235, 9, 1, @"Speech color"), 6);
            _speechColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new RectInt(60, 235, 15, 15), new RectInt(60, 235, 450, 150), 30, 10, Hues.TextTones, UltimaGameSettings.UserInterface.SpeechColor), 6);

            AddControl(new TextLabelAscii(this, 85, 255, 9, 1, @"Emote color"), 6);
            _emoteColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new RectInt(60, 255, 15, 15), new RectInt(60, 255, 450, 150), 30, 10, Hues.TextTones, UltimaGameSettings.UserInterface.EmoteColor), 6);

            AddControl(new TextLabelAscii(this, 85, 275, 9, 1, @"Party message color"), 6);
            _partyMsgColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new RectInt(60, 275, 15, 15), new RectInt(60, 275, 450, 150), 30, 10, Hues.TextTones, UltimaGameSettings.UserInterface.PartyMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 295, 9, 1, @"Guild message color"), 6);
            _guildMsgColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new RectInt(60, 295, 15, 15), new RectInt(60, 295, 450, 150), 30, 10, Hues.TextTones, UltimaGameSettings.UserInterface.GuildMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 315, 9, 1, @"Ignore guild messages"), 6);
            _ignoreGuildMsg = AddControl<CheckBox>(new CheckBox(this, 60, 315, 210, 211, Settings.UserInterface.IgnoreGuildMsg, 62), 6);

            AddControl(new TextLabelAscii(this, 85, 335, 9, 1, @"Alliance message color"), 6);
            _allianceMsgColor = AddControl<ColorPickerBox>(new ColorPickerBox(this, new RectInt(60, 335, 15, 15), new RectInt(60, 335, 450, 150), 30, 10, Hues.TextTones, UltimaGameSettings.UserInterface.AllianceMsgColor), 6);

            AddControl(new TextLabelAscii(this, 85, 355, 9, 1, @"Ignore alliance messages"), 6);
            _ignoreAllianceMsg = AddControl<CheckBox>(new CheckBox(this, 60, 355, 210, 211, UltimaGameSettings.UserInterface.IgnoreAllianceMsg, 62), 6);

            // page 7 Reputation system
            AddControl(new Button(this, 576, 180, 229, 229, ButtonTypes.SwitchPage, 7, (int)Buttons.Reputation), 7);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Reputation system"), 7);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"These settings affect the reputation system, which is Ultima Online's system for controlling antisocial behavior."), 7);

            AddControl(new TextLabelAscii(this, 85, 100, 9, 1, @"Query before performing criminal actions"), 7);
            _crimeQuery = AddControl<CheckBox>(new CheckBox(this, 60, 100, 210, 211, UltimaGameSettings.UserInterface.CrimeQuery, 61), 7);

            // page 8 Miscellaneous
            AddControl(new Button(this, 576, 250, 231, 231, ButtonTypes.SwitchPage, 8, (int)Buttons.Miscellaneous), 8);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Miscellaneous"), 8);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @"Miscellaneous options."), 8);

            AddControl(new TextLabelAscii(this, 85, 80, 9, 1, @"Enable debug console"), 8);
            _isConsoleEnabled = AddControl<CheckBox>(new CheckBox(this, 60, 80, 210, 211, UltimaGameSettings.Debug.IsConsoleEnabled, 61), 8);

            AddControl(new TextLabelAscii(this, 85, 100, 9, 1, @"Show FPS"), 8);
            _showFps = AddControl<CheckBox>(new CheckBox(this, 60, 100, 210, 211, UltimaGameSettings.Debug.ShowFps, 61), 8);

            // page 9 Filter Options
            AddControl(new Button(this, 576, 320, 234, 234, ButtonTypes.SwitchPage, 9, (int)Buttons.Filters), 9);
            AddControl(new TextLabelAscii(this, 250, 20, 2, 1, @"Filter Options"), 9);
            AddControl(new TextLabelAscii(this, 60, 45, 9, 1, @""), 9);
            
            ChangeCurrentMacro(Macros.Player.Count - 1);
        }

        public void ChangeCurrentMacro(int index)
        {
            setDefaultDropdownList(false);

            if (index < 0 || index >= Macros.Player.Count)
                return;

            var action = Macros.Player.All[index];

            _currentMacro = index;
            _macroKeyPress.Key = action.Keystroke;

            _chkShift.IsChecked = action.Shift;
            _chkAlt.IsChecked = action.Alt;
            _chkCtrl.IsChecked = action.Ctrl;

            for (var i = 0; i < action.Macros.Count; i++)
            {
                _actionTypeList[i].Index = (int)action.Macros[i].Type;

                if (action.Macros[i].ValueType == Macro.ValueTypes.None)
                {
                }
                else if (action.Macros[i].ValueType == Macro.ValueTypes.Integer)
                {
                    if (!_actionDropDown[i].IsFirstvisible)
                        _actionDropDown[i].CreateVisual();//ACTIVATED VISUAL
                    _actionDropDown[i].setIndex((int)action.Macros[i].Type, action.Macros[i].ValueInteger);
                    _actionText[i].IsEditable = false;
                }
                else
                {
                    if (!_actionDropDown[i].IsFirstvisible)
                        _actionDropDown[i].CreateVisual();//ACTIVATED VISUAL
                    //_action1List[i].m_scrollButton.IsVisible = false;//SCROLL İCON İT'S REALLY PROBLEM FOR ME :( İ CAN'T TO MYSELF SO I USED SELF METHOD
                    _actionDropDown[i].ScrollButton.IsVisible = true;
                    _actionDropDown[i].Items.Clear();
                    _actionDropDown[i].IsVisible = true;
                    _actionText[i].IsEditable = true;
                    _actionText[i].IsVisible = true;
                    _actionText[i].Text = action.Macros[i].ValueString;
                }
            }
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("options");
            base.OnInitialize();
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (_nextRefreshAt + REFRESH_INTERVAL < totalMS) //need to update
            {
                _nextRefreshAt = totalMS + REFRESH_INTERVAL;
                _labels[(int)Labels.MusicVolume].Text = _musicVolume.Value.ToString();
                _labels[(int)Labels.SoundVolume].Text = _soundVolume.Value.ToString();
            }

            base.Update(totalMS, frameMS);
        }

        public int GetCurrentFullScreenIndex()
        {
            var res = string.Format("{0}x{1}", UltimaGameSettings.UserInterface.FullScreenResolution.Width, UltimaGameSettings.UserInterface.FullScreenResolution.Height);
            for (var i = 0; i < Resolutions.FullScreenResolutionsList.Count; i++)
                if (Resolutions.FullScreenResolutionsList[i].Width == UltimaGameSettings.UserInterface.FullScreenResolution.Width && Resolutions.FullScreenResolutionsList[i].Height == UltimaGameSettings.UserInterface.FullScreenResolution.Height)
                    return i;
            return -1;
        }

        public int GetCurrentPlayWindowIndex()
        {
            var res = string.Format("{0}x{1}", UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width, UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height);
            for (var i = 0; i < Resolutions.PlayWindowResolutionsList.Count; i++)
                if (Resolutions.PlayWindowResolutionsList[i].Width == UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Width && Resolutions.PlayWindowResolutionsList[i].Height == UltimaGameSettings.UserInterface.PlayWindowGumpResolution.Height)
                    return i;
            return -1;
        }

        public void SaveSettings()
        {
            //macros  
            SaveCurrentMacro();  // save the currently displayed macro, if any
            Macros.Player.Save();

            //audio
            UltimaGameSettings.Audio.MusicVolume = _musicVolume.Value;
            UltimaGameSettings.Audio.SoundVolume = _soundVolume.Value;
            UltimaGameSettings.Audio.MusicOn = _musicOn.IsChecked;
            UltimaGameSettings.Audio.SoundOn = _soundOn.IsChecked;
            UltimaGameSettings.Audio.FootStepSoundOn = _footStepSoundOn.IsChecked;

            //interface
            UltimaGameSettings.UserInterface.AlwaysRun = _alwaysRun.IsChecked;
            UltimaGameSettings.UserInterface.MenuBarDisabled = _menuBarDisabled.IsChecked;
            UltimaGameSettings.UserInterface.FullScreenResolution = new ResolutionProperty(Resolutions.FullScreenResolutionsList[_dropDownFullScreenResolutions.Index].Width, Resolutions.FullScreenResolutionsList[_dropDownFullScreenResolutions.Index].Height);
            UltimaGameSettings.UserInterface.PlayWindowGumpResolution = new ResolutionProperty(Resolutions.PlayWindowResolutionsList[_dropDownPlayWindowResolutions.Index].Width, Resolutions.PlayWindowResolutionsList[_dropDownPlayWindowResolutions.Index].Height);
            UltimaGameSettings.Engine.IsVSyncEnabled = _isVSyncEnabled.IsChecked;
            UltimaGameSettings.Debug.IsConsoleEnabled = _isConsoleEnabled.IsChecked;
            UltimaGameSettings.Debug.ShowFps = _showFps.IsChecked;

            UltimaGameSettings.UserInterface.SpeechColor = _speechColor.Index;
            UltimaGameSettings.UserInterface.EmoteColor = _emoteColor.Index;
            UltimaGameSettings.UserInterface.PartyMsgColor = _partyMsgColor.Index;
            UltimaGameSettings.UserInterface.GuildMsgColor = _guildMsgColor.Index;
            UltimaGameSettings.UserInterface.IgnoreGuildMsg = _ignoreGuildMsg.IsChecked;
            UltimaGameSettings.UserInterface.AllianceMsgColor = _allianceMsgColor.Index;
            UltimaGameSettings.UserInterface.IgnoreAllianceMsg = _ignoreAllianceMsg.IsChecked;

            UltimaGameSettings.UserInterface.CrimeQuery = _crimeQuery.IsChecked;

            SwitchTopMenuGump();
        }

        public void setDefaultDropdownList(bool isEditable)
        {
            _macroKeyPress.Key = WinKeys.None;
            _chkShift.IsChecked = false;
            _chkAlt.IsChecked = false;
            _chkCtrl.IsChecked = false;
            for (int i = 0; i < _actionTypeList.Count(); i++)
            {
                _actionTypeList[i].Index = 0;
                _actionDropDown[i].IsVisible = false;
                if (_actionDropDown[i].IsFirstvisible)
                    _actionDropDown[i].ScrollButton.IsVisible = false;

                _actionText[i].Text = "";
                _actionText[i].IsVisible = isEditable;
            }
        }

        public void SaveCurrentMacro()
        {
            var action = Macros.Player.All[_currentMacro];
            if (action == null)
                return;

            action.Keystroke = _macroKeyPress.Key;
            action.Shift = _chkShift.IsChecked;
            action.Alt = _chkAlt.IsChecked;
            action.Ctrl = _chkCtrl.IsChecked;

            action.Macros.Clear();

            for (var i = 0; i < _actionTypeList.Length; i++)
            {
                var macro = new Macro((MacroType)_actionTypeList[i].Index);
                switch (macro.Type)
                {
                    case MacroType.Say:
                    case MacroType.Whisper:
                    case MacroType.Yell:
                    case MacroType.Emote:
                    case MacroType.Delay:
                        macro.ValueString = _actionText[i].Text;
                        break;
                    case MacroType.UseSkill:
                    case MacroType.CastSpell:
                    case MacroType.OpenGump:
                    case MacroType.CloseGump:
                    case MacroType.Move:
                    case MacroType.ArmDisarm:
                        macro.ValueInteger = macro.ValueInteger = _actionDropDown[i].Index;
                        break;
                    default:
                        // no value by default
                        break;
                }
                action.Macros.Add(macro);
            }
        }

        public void SwitchTopMenuGump()
        {
            if (!UltimaGameSettings.UserInterface.MenuBarDisabled && _userInterface.GetControl<TopMenuGump>() == null)
                _userInterface.AddControl(new TopMenuGump(), 0, 0); // by default at the top of the screen.
            else if (UltimaGameSettings.UserInterface.MenuBarDisabled && _userInterface.GetControl<TopMenuGump>() != null)
                _userInterface.GetControl<TopMenuGump>().Dispose();
        }

        public override void OnButtonClick(int buttonID)
        {
            switch ((Buttons)buttonID)
            {
                case Buttons.Cancel:
                    {
                        Dispose();
                        break;
                    }
                case Buttons.Apply:
                    {
                        SaveSettings();
                        break;
                    }
                case Buttons.Default:
                    {
                        break;
                    }
                case Buttons.Okay:
                    {
                        SaveSettings();
                        Dispose();
                        break;
                    }
                case Buttons.MAdd:
                    SaveCurrentMacro();
                    Macros.Player.AddNewMacroAction(new Action(), _currentMacro + 1);
                    setDefaultDropdownList(true);
                    break;

                case Buttons.MDelete:
                    if (Macros.Player.All.Count == 0)
                        return;
                    Macros.Player.All.RemoveAt(_currentMacro);
                    _currentMacro--;
                    if (_currentMacro < 0)
                        _currentMacro = 0;
                    ChangeCurrentMacro(_currentMacro);
                    break;

                case Buttons.MPrevious:
                    _currentMacro--;
                    if (_currentMacro < 0)
                        _currentMacro = 0;

                    ChangeCurrentMacro(_currentMacro);
                    break;

                case Buttons.MNext:
                    _currentMacro++;
                    if (_currentMacro >= Macros.Player.All.Count)
                        _currentMacro = Macros.Player.All.Count - 1;

                    ChangeCurrentMacro(_currentMacro);
                    break;
            }
        }

        private enum Buttons
        {
            Sound,
            Help,
            Chat,
            Macros,
            MAdd,
            MDelete,
            MPrevious,
            MNext,
            Interface,
            Display,
            Reputation,
            Miscellaneous,
            Filters,
            Cancel,
            Apply,
            Default,
            Okay
        }

        protected override void CloseWithRightMouseButton()
        {
            // reset changes to macro list
            Macros.Player.Load();
            base.CloseWithRightMouseButton();
        }
    }
}