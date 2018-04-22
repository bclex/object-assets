using System;

namespace OA.Configuration
{
    public class BaseSettings
    {
        readonly DebugSettings _debug;
        readonly GameSettings _game;
        readonly XRSettings _xr;

        protected BaseSettings()
        {
            _debug = CreateOrOpenSection<DebugSettings>();
            _game = CreateOrOpenSection<GameSettings>();
            _xr = CreateOrOpenSection<XRSettings>();
        }

        public static DebugSettings Debug => _instance._debug;
        public static GameSettings Game => _instance._game;
        public static XRSettings XR => _instance._xr;

        static readonly BaseSettings _instance;
        static readonly SettingsFile _file;

        static BaseSettings()
        {
            _file = new SettingsFile("settings.cfg");
            _instance = new BaseSettings();
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
