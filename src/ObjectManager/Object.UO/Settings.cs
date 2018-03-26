using OA.Ultima.Configuration;
using OA.Ultima.Core.Configuration;
using System;

namespace OA.Ultima
{
    public class Settings
    {
        // === Instance ===============================================================================================
        readonly DebugSettings _debug;
        readonly EngineSettings _engine;
        readonly GumpSettings _gumps;
        readonly UserInterfaceSettings _ui;
        readonly LoginSettings _login;
        readonly UltimaOnlineSettings _ultimaOnline;
        readonly AudioSettings _audio;

        Settings()
        {
            _debug = CreateOrOpenSection<DebugSettings>();
            _login = CreateOrOpenSection<LoginSettings>();
            _ultimaOnline = CreateOrOpenSection<UltimaOnlineSettings>();
            _engine = CreateOrOpenSection<EngineSettings>();
            _ui = CreateOrOpenSection<UserInterfaceSettings>();
            _gumps = CreateOrOpenSection<GumpSettings>();
            _audio = CreateOrOpenSection<AudioSettings>();
        }

        // === Static Settings properties =============================================================================
        public static DebugSettings Debug => _instance._debug;
        public static LoginSettings Login => _instance._login;
        public static UltimaOnlineSettings UltimaOnline => _instance._ultimaOnline;
        public static EngineSettings Engine => _instance._engine;
        public static GumpSettings Gumps => _instance._gumps;
        public static UserInterfaceSettings UserInterface => _instance._ui;
        public static AudioSettings Audio => _instance._audio;

        static readonly Settings _instance;
        static readonly SettingsFile _file;

        static Settings()
        {
            _file = new SettingsFile("settings.cfg");
            _instance = new Settings();
            _file.Load();
        }

        public static void Save()
        {
            _file.Save();
        }

        public static T CreateOrOpenSection<T>()
            where T : ASettingsSection, new()
        {
            var sectionName = typeof(T).Name;
            var section = _file.CreateOrOpenSection<T>(sectionName);
            // Resubscribe incase this is called for a section 2 times.
            section.Invalidated -= OnSectionInvalidated;
            section.Invalidated += OnSectionInvalidated;
            section.PropertyChanged -= OnSectionPropertyChanged;
            section.PropertyChanged += OnSectionPropertyChanged;
            return section;
        }

        static void OnSectionPropertyChanged(object sender, EventArgs e)
        {
            _file.InvalidateDirty();
        }

        static void OnSectionInvalidated(object sender, EventArgs e)
        {
            _file.InvalidateDirty();
        }
    }
}
