using OA.Core;
using OA.Core.Audio;
using OA.Ultima.Resources;
using System.Collections.Generic;

namespace OA.Ultima.Audio
{
    public class AudioService
    {
        readonly Dictionary<int, ASound> _sounds = new Dictionary<int, ASound>();
        readonly Dictionary<int, ASound> _music = new Dictionary<int, ASound>();
        UOMusic _musicCurrentlyPlaying;

        public void Update()
        {
            if (_musicCurrentlyPlaying != null)
                _musicCurrentlyPlaying.Update();
        }

        public void PlaySound(int soundIndex, AudioEffects effect = AudioEffects.None, float volume = 1.0f, bool spamCheck = false)
        {
            if (volume < 0.01f)
                return;
            if (UltimaGameSettings.Audio.SoundOn)
            {
                ASound sound;
                if (_sounds.TryGetValue(soundIndex, out sound))
                    sound.Play(true, effect, volume, spamCheck);
                else
                {
                    string name;
                    byte[] data;
                    if (SoundData.TryGetSoundData(soundIndex, out data, out name))
                    {
                        sound = new UOSound(name, data);
                        _sounds.Add(soundIndex, sound);
                        sound.Play(true, effect, volume, spamCheck);
                    }
                }
            }
        }

        public void PlayMusic(int id)
        {
            if (UltimaGameSettings.Audio.MusicOn)
            {
                if (id < 0) // not a valid id, used to stop music.
                {
                    StopMusic();
                    return;
                }
                if (!_music.ContainsKey(id))
                {
                    string name;
                    bool loops;
                    if (MusicData.TryGetMusicData(id, out name, out loops))
                        _music.Add(id, new UOMusic(id, name, loops));
                    else
                    {
                        Utils.Error($"Received unknown music id {id}");
                        return;
                    }
                }
                var toPlay = _music[id];
                if (toPlay != _musicCurrentlyPlaying)
                {
                    // stop the current song
                    StopMusic();
                    // play the new song!
                    _musicCurrentlyPlaying = toPlay as UOMusic;
                    _musicCurrentlyPlaying.Play(false);
                }
            }
        }

        public void StopMusic()
        {
            if (_musicCurrentlyPlaying != null)
            {
                _musicCurrentlyPlaying.Stop();
                _musicCurrentlyPlaying.Dispose();
                _musicCurrentlyPlaying = null;
            }
        }
    }
}
