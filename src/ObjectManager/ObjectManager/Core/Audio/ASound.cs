using System;
using System.Collections.Generic;

namespace OA.Core.Audio
{
    public abstract class ASound : IDisposable
    {
        string _name;
        public string Name
        {
            get { return _name; }
            private set
            {
                if (!string.IsNullOrEmpty(value)) _name = value.Replace(".mp3", "");
                else _name = string.Empty;
            }
        }
        public DateTime LastPlayed = DateTime.MinValue;
        public static TimeSpan MinimumDelay = TimeSpan.FromSeconds(1d);

        abstract protected byte[] GetBuffer();
        abstract protected void OnBufferNeeded(object sender, EventArgs e);
        virtual protected void AfterStop() { }
        virtual protected void BeforePlay() { }

        //static readonly List<Tuple<DynamicSoundEffectInstance, double>> _effectInstances;
        //static readonly List<Tuple<DynamicSoundEffectInstance, double>> _musicInstances;
        //protected DynamicSoundEffectInstance _thisInstance;

        //protected int Frequency = 22050;
        //protected AudioChannels Channels = AudioChannels.Mono;

        //static ASound()
        //{
        //    _effectInstances = new List<Tuple<DynamicSoundEffectInstance, double>>();
        //    _musicInstances = new List<Tuple<DynamicSoundEffectInstance, double>>();
        //}

        public ASound(string name)
        {
            Name = name;
        }

        public void Dispose()
        {
            //if (_thisInstance != null)
            //{
            //    _thisInstance.BufferNeeded -= OnBufferNeeded;
            //    if (!_thisInstance.IsDisposed)
            //    {
            //        _thisInstance.Stop();
            //        _thisInstance.Dispose();
            //    }
            //    _thisInstance = null;
            //}
        }

        /// <summary>
        /// Plays the effect.
        /// </summary>
        /// <param name="asEffect">Set to false for music, true for sound effects.</param>
        public void Play(bool asEffect, AudioEffects effect = AudioEffects.None, float volume = 1.0f, bool spamCheck = false)
        {
            //double now = (float)Service.Get<UltimaGame>().TotalMS;
            //CullExpiredEffects(now);

            //if (spamCheck && (LastPlayed + MinimumDelay > DateTime.Now))
            //    return;

            //BeforePlay();
            //_thisInstance = GetNewInstance(asEffect);
            //if (_thisInstance == null)
            //{
            //    Dispose();
            //    return;
            //}

            //switch (effect)
            //{
            //    case AudioEffects.PitchVariation:
            //        float pitch = Utility.RandomValue(-5, 5) * .025f;
            //        _thisInstance.Pitch = pitch;
            //        break;
            //}
            
            //LastPlayed = DateTime.Now;

            //byte[] buffer = GetBuffer();
            //if (buffer != null && buffer.Length > 0)
            //{
            //    _thisInstance.BufferNeeded += OnBufferNeeded;
            //    _thisInstance.SubmitBuffer(buffer);
            //    _thisInstance.Volume = volume;
            //    _thisInstance.Play();
            //    List<Tuple<DynamicSoundEffectInstance, double>> list = (asEffect) ? _effectInstances : _musicInstances;
            //    double ms = _thisInstance.GetSampleDuration(buffer.Length).TotalMilliseconds;
            //    list.Add(new Tuple<DynamicSoundEffectInstance, double>(_thisInstance, now + ms));
            //}
        }

        public void Stop()
        {
            AfterStop();
        }

        private void CullExpiredEffects(double now)
        {
            //// Check to see if any existing instances have stopped playing. If they have, remove the
            //// reference to them so the garbage collector can collect them.
            //for (int i = 0; i < _effectInstances.Count; i++)
            //{
            //    if (_effectInstances[i].Item1.IsDisposed || _effectInstances[i].Item1.State == SoundState.Stopped || _effectInstances[i].Item2 <= now)
            //    {
            //        _effectInstances[i].Item1.Dispose();
            //        _effectInstances.RemoveAt(i);
            //        i--;
            //    }
            //}

            //for (int i = 0; i < _musicInstances.Count; i++)
            //{
            //    if (_musicInstances[i].Item1.IsDisposed || _musicInstances[i].Item1.State == SoundState.Stopped)
            //    {
            //        _musicInstances[i].Item1.Dispose();
            //        _musicInstances.RemoveAt(i);
            //        i--;
            //    }
            //}
        }

        //private DynamicSoundEffectInstance GetNewInstance(bool asEffect)
        //{
        //    List<Tuple<DynamicSoundEffectInstance, double>> list = (asEffect) ? _effectInstances : _musicInstances;
        //    int maxInstances = (asEffect) ? 32 : 2;
        //    if (list.Count >= maxInstances)
        //        return null;
        //    else
        //        return new DynamicSoundEffectInstance(Frequency, Channels); // shouldn't we be recycling these?
        //}
    }
}
