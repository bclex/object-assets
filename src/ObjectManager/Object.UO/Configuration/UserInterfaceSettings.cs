using OA.Ultima.Configuration.Properties;
using OA.Ultima.Core;
using OA.Ultima.Core.Configuration;
using OA.Ultima.Resources;

namespace OA.Ultima.Configuration
{
    public class UserInterfaceSettings : ASettingsSection
    {
        ResolutionProperty _fullScreenResolution;
        ResolutionProperty _windowResolution;
        ResolutionProperty _worldGumpResolution;
        bool _playWindowPixelDoubling;
        bool _isFullScreen;
        MouseProperty _mouse;
        bool _alwaysRun;
        bool _menuBarDisabled;

        int _speechColor = 4 + Utility.RandomValue(0, 99) * 5;
        int _emoteColor = 646;
        int _partyMsgPrivateColor = 58;
        int _partyMsgColor = 68;
        int _guildMsgColor = 70;
        bool _ignoreGuildMsg;
        int _allianceMsgColor = 487;
        bool _ignoreAllianceMsg;
        bool _crimeQuery;

        public UserInterfaceSettings()
        {
            FullScreenResolution = new ResolutionProperty();
            WindowResolution = new ResolutionProperty();
            PlayWindowGumpResolution = new ResolutionProperty();
            _playWindowPixelDoubling = false;
            IsMaximized = false;
            Mouse = new MouseProperty();
            AlwaysRun = false;
            MenuBarDisabled = false;
            CrimeQuery = true;
        }

        public bool IsMaximized
        {
            get { return _isFullScreen; }
            set { SetProperty(ref _isFullScreen, value); }
        }

        public MouseProperty Mouse
        {
            get { return _mouse; }
            set { SetProperty(ref _mouse, value); }
        }

        public ResolutionProperty FullScreenResolution
        {
            get { return _fullScreenResolution; }
            set
            {
                if (!Resolutions.IsValidFullScreenResolution(value))
                    return;
                SetProperty(ref _fullScreenResolution, value);
            }
        }

        public ResolutionProperty WindowResolution
        {
            get { return _windowResolution; }
            set { SetProperty(ref _windowResolution, value); }
        }

        public ResolutionProperty PlayWindowGumpResolution
        {
            get { return _worldGumpResolution; }
            set
            {
                if (!Resolutions.IsValidPlayWindowResolution(value))
                    SetProperty(ref _worldGumpResolution, new ResolutionProperty());
                SetProperty(ref _worldGumpResolution, value);
            }
        }

        public bool PlayWindowPixelDoubling
        {
            get { return _playWindowPixelDoubling; }
            set { SetProperty(ref _playWindowPixelDoubling, value); }
        }

        public bool AlwaysRun
        {
            get { return _alwaysRun; }
            set { SetProperty(ref _alwaysRun, value); }
        }

        public bool MenuBarDisabled
        {
            get { return _menuBarDisabled; }
            set { SetProperty(ref _menuBarDisabled, value); }
        }

        public int SpeechColor
        {
            get { return _speechColor; }
            set { SetProperty(ref _speechColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int EmoteColor
        {
            get { return _emoteColor; }
            set { SetProperty(ref _emoteColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int PartyPrivateMsgColor
        {
            get { return _partyMsgPrivateColor; }
            set { SetProperty(ref _partyMsgPrivateColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int PartyMsgColor
        {
            get { return _partyMsgColor; }
            set { SetProperty(ref _partyMsgColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public int GuildMsgColor
        {
            get { return _guildMsgColor; }
            set { SetProperty(ref _guildMsgColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public bool IgnoreGuildMsg
        {
            get { return _ignoreGuildMsg; }
            set { SetProperty(ref _ignoreGuildMsg, value); }
        }

        public int AllianceMsgColor
        {
            get { return _allianceMsgColor; }
            set { SetProperty(ref _allianceMsgColor, Clamp(value, 0, HueData.HueCount - 1)); }
        }

        public bool IgnoreAllianceMsg
        {
            get { return _ignoreAllianceMsg; }
            set { SetProperty(ref _ignoreAllianceMsg, value); }
        }

        public bool CrimeQuery
        {
            get { return _crimeQuery; }
            set { SetProperty(ref _crimeQuery, value); }
        }
    }
}
