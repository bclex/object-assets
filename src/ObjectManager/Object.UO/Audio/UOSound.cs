using OA.Ultima.Core.Audio;
using System;

namespace OA.Ultima.Audio
{
    class UOSound : ASound
    {
        readonly byte[] _waveBuffer;

        public UOSound(string name, byte[] buffer)
            : base(name)
        {
            _waveBuffer = buffer;
        }

        protected override void OnBufferNeeded(object sender, EventArgs e)
        {
            // not needed.
        }

        protected override byte[] GetBuffer()
        {
            return _waveBuffer;
        }
    };
}
