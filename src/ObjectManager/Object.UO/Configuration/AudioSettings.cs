using OA.Ultima.Core.Configuration;

namespace OA.Ultima.Configuration
{
    public class AudioSettings : ASettingsSection
    {
        int _musicVolume;
        int _soundVolume;
        bool _musicOn;
        bool _soundOn;
        bool _footStepSoundOn;

        public AudioSettings()
        {
            MusicVolume = 100;
            SoundVolume = 100;
            MusicOn = true;
            SoundOn = true;
            FootStepSoundOn = true;
        }

        /// <summary>
        /// Volume of music. Value is percent of max volume, clamped to 0 - 100.
        /// </summary>
        public int MusicVolume
        {
            get { return _musicVolume; }
            set{ SetProperty(ref _musicVolume, Clamp(value, 0, 100)); }
        }

        /// <summary>
        /// Volume of sound effects. Value is percent of max volume, clamped to 0 - 100.
        /// </summary>
        public int SoundVolume
        {
            get { return _soundVolume; }
            set { SetProperty(ref _soundVolume, Clamp(value, 0, 100)); }
        }

        /// <summary>
        /// False = requests to play music are ignored.
        /// </summary>
        public bool MusicOn
        {
            get { return _musicOn; }
            set { SetProperty(ref _musicOn, value); }
        }

        /// <summary>
        /// False = requests to play sound effects are ignored.
        /// </summary>
        public bool SoundOn
        {
            get { return _soundOn; }
            set { SetProperty(ref _soundOn, value); }
        }

        /// <summary>
        /// False = no foot step sound effects ever play.
        /// </summary>
        public bool FootStepSoundOn
        {
            get { return _footStepSoundOn; }
            set { SetProperty(ref _footStepSoundOn, value); }
        }
    }
}
