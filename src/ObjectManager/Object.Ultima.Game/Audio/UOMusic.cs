using MP3Sharp;
using OA.Core;
using OA.Core.Audio;
using OA.Ultima.IO;
using System;

namespace OA.Ultima.Audio
{
    class UOMusic : ASound
    {
        MP3Stream _stream;
        const int NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK = 0x8000; // 32768 bytes, about 0.9 seconds
        readonly byte[] _waveBuffer = new byte[NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK];
        bool _repeat;
        bool _playing;

        string Path => FileManager.GetPath(string.Format("Music/Digital/{0}.mp3", Name));

        public UOMusic(int index, string name, bool loop)
            : base(name)
        {
            _repeat = loop;
            _playing = false;
            //Channels = AudioChannels.Stereo;
        }

        public void Update()
        {
            // sanity - if the buffer empties, we will lose our sound effect. Thus we must continually check if it is dead.
            OnBufferNeeded(null, null);
        }

        protected override byte[] GetBuffer()
        {
            if (_playing)
            {
                var bytesReturned = _stream.Read(_waveBuffer, 0, _waveBuffer.Length);
                if (bytesReturned != NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK)
                {
                    if (_repeat)
                    {
                        _stream.Position = 0;
                        _stream.Read(_waveBuffer, bytesReturned, _waveBuffer.Length - bytesReturned);
                    }
                    else
                    {
                        if (bytesReturned == 0)
                            Stop();
                    }
                }
                return _waveBuffer;
            }
            Stop();
            return null;
        }

        protected override void OnBufferNeeded(object sender, EventArgs e)
        {
            //if (_playing)
            //    while (_thisInstance.PendingBufferCount < 3)
            //    {
            //        var buffer = GetBuffer();
            //        if (_thisInstance.IsDisposed)
            //            return;
            //        _thisInstance.SubmitBuffer(buffer);
            //    }
        }

        protected override void BeforePlay()
        {
            //if (_playing)
            //    Stop();
            //try
            //{
            //    _stream = new MP3Stream(Path, NUMBER_OF_PCM_BYTES_TO_READ_PER_CHUNK);
            //    Frequency = _stream.Frequency;
            //    _playing = true;
            //}
            //catch (Exception e)
            //{
            //    // file in use or access denied.
            //    Utils.Error(e);
            //    _playing = false;
            //}
        }

        protected override void AfterStop()
        {
            if (_playing)
            {
                _playing = false;
                _stream.Close();
                _stream = null;
            }
        }
    }
}