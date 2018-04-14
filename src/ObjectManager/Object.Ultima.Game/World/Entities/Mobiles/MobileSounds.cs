using OA.Core;
using OA.Core.Audio;
using OA.Ultima.Audio;
using OA.Ultima.Core;
using System.Collections.Generic;

namespace OA.Ultima.World.Entities.Mobiles
{
    public static class MobileSounds
    {
        static AudioService _audio = Service.Get<AudioService>();

        static Dictionary<Serial, MobileSoundData> _data = new Dictionary<Serial,MobileSoundData>();

        static int[] _stepSFX = { 0x12B, 0x12C };
        static int[] _stepMountedSFX = { 0x129, 0x12A };

        public static void ResetFootstepSounds(Mobile mobile)
        {
            if (_data.ContainsKey(mobile.Serial))
                _data[mobile.Serial].LastFrame = 1.0f;
        }

        public static void DoFootstepSounds(Mobile mobile, double frame)
        {
            if (!mobile.Body.IsHumanoid || mobile.Flags.IsHidden)
                return;

            MobileSoundData data;
            if (!_data.TryGetValue(mobile.Serial, out data))
            {
                data = new MobileSoundData(mobile);
                _data.Add(mobile.Serial, data);
            }

            bool play = (data.LastFrame < 0.5d && frame >= 0.5d) || (data.LastFrame > 0.5d && frame < 0.5d);
            if (mobile.IsMounted && !mobile.IsRunning && frame > 0.5d)
                play = false;

            if (play)
            {
                float volume = 1f;
                int distanceFromPlayer = Utility.DistanceBetweenTwoPoints(mobile.DestinationPosition.Tile, WorldModel.Entities.GetPlayerEntity().DestinationPosition.Tile);
                if (distanceFromPlayer > 4)
                    volume = 1f - (distanceFromPlayer - 4) * 0.05f;


                if (mobile.IsMounted && mobile.IsRunning)
                {
                    int sfx = Utility.RandomValue(0, _stepMountedSFX.Length - 1);
                    _audio.PlaySound(_stepMountedSFX[sfx], AudioEffects.PitchVariation, volume);
                }
                else
                {
                    int sfx = Utility.RandomValue(0, _stepSFX.Length - 1);
                    _audio.PlaySound(_stepSFX[sfx], AudioEffects.PitchVariation, volume);
                }
            }
            data.LastFrame = frame;
        }

        private class MobileSoundData
        {
            public Mobile Mobile
            {
                get;
                private set;
            }

            public double LastFrame = 1d;

            public MobileSoundData(Mobile mobile)
            {
                Mobile = mobile;
            }
        }
    }
}
