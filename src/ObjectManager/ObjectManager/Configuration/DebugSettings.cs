namespace OA.Configuration
{
    public sealed class DebugSettings : ASettingsSection
    {
        bool _isConsoleEnabled = true;
        bool _showFps = true;
        bool _logPackets = false;

        /// <summary>
        /// If true, all received packets will be logged to Tracer.Debug, and any active Tracer listeners (console, debug.txt file logger)
        /// </summary>
        public bool LogPackets
        {
            get { return _logPackets; }
            set { SetProperty(ref _logPackets, value); }
        }

        /// <summary>
        /// If true, FPS should display either in the window caption or in the game window. (not currently enabled).
        /// </summary>
        public bool ShowFps
        {
            get { return _showFps; }
            set { SetProperty(ref _showFps, value); }
        }

        /// <summary>
        /// If true, a console window which will display debug and error messages should appear at runtime. This may not work in Release configurations.
        /// </summary>
        public bool IsConsoleEnabled
        {
            get { return _isConsoleEnabled; }
            set { SetProperty(ref _isConsoleEnabled, value); }
        }
    }
}