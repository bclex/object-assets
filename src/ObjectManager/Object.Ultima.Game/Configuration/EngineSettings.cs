using OA.Configuration;

namespace OA.Ultima.Configuration
{
    public sealed class EngineSettings : ASettingsSection
    {
        bool _isVSyncEnabled;
        bool _isFixedTimeStep;

        public EngineSettings()
        {
            IsFixedTimeStep = true;
            IsVSyncEnabled = false;
        }

        public bool IsFixedTimeStep
        {
            get { return _isFixedTimeStep; }
            set { SetProperty(ref _isFixedTimeStep, value); }
        }

        public bool IsVSyncEnabled
        {
            get { return _isVSyncEnabled; }
            set { SetProperty(ref _isVSyncEnabled, value); }
        }
    }
}