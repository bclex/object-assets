using UnityEngine;

namespace OA.Core
{
    public class OneShotAudioStreamComponent : MonoBehaviour
    {
        public MP3StreamReader audioStream;

#if false
    AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    private void Update()
    {
        if (audioStream != null && audioStream.isDoneStreaming)
        {
            Debug.Log(audioStream.streamedSampleFrameCount * audioStream.channelCount);
            audioStream = null;
            audioSource.loop = false;
        }
        if (!audioSource.isPlaying)
        {
            Destroy(gameObject, 0.5f);
            enabled = false;
        }
    }
#endif

        PCMAudioBuffer streamBuffer;
        int UnitySampleRate = -1;

        private void Start()
        {
            streamBuffer = new PCMAudioBuffer(audioStream.channelCount, audioStream.bitDepth, audioStream.samplingRate, 8192);
            UnitySampleRate = AudioSettings.outputSampleRate;
        }

        private void Update()
        {
            if (audioStream.isDoneStreaming)
            {
                Destroy(gameObject);
                enabled = false;
            }
        }

        private void OnAudioFilterRead(float[] samples, int channelCount)
        {
            if (UnitySampleRate > 0)
            {
                var lowSRSampleCount = (int)((44100.0f / UnitySampleRate) * samples.Length);
                var lowSRSamples = new float[lowSRSampleCount];
                var samplesReturned = AudioUtils.FillUnityStreamBuffer(lowSRSamples, streamBuffer, audioStream);
                AudioUtils.ResampleHack(lowSRSamples, samples);
                //AudioUtils.LowPassHack(samples);
                if (audioStream.isOpen && audioStream.isDoneStreaming)
                    audioStream.Close();
            }
        }
    }
}