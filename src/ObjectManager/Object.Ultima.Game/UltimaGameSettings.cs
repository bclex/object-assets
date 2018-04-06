using OA.Configuration;
using OA.Ultima.Configuration;

namespace OA.Ultima
{
    public class UltimaGameSettings : BaseSettings
    {
        readonly EngineSettings _engine;
        readonly GumpSettings _gumps;
        readonly UserInterfaceSettings _ui;
        readonly LoginSettings _login;
        readonly UltimaOnlineSettings _ultimaOnline;
        readonly AudioSettings _audio;

        UltimaGameSettings()
        {
            _login = CreateOrOpenSection<LoginSettings>();
            _ultimaOnline = CreateOrOpenSection<UltimaOnlineSettings>();
            _engine = CreateOrOpenSection<EngineSettings>();
            _ui = CreateOrOpenSection<UserInterfaceSettings>();
            _gumps = CreateOrOpenSection<GumpSettings>();
            _audio = CreateOrOpenSection<AudioSettings>();
        }

        public static LoginSettings Login => _instance._login;
        public static UltimaOnlineSettings UltimaOnline => _instance._ultimaOnline;
        public static EngineSettings Engine => _instance._engine;
        public static GumpSettings Gumps => _instance._gumps;
        public static UserInterfaceSettings UserInterface => _instance._ui;
        public static AudioSettings Audio => _instance._audio;
    }
}
