using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace MusicCreator.ReaderCode;

class AudioPlaybackEngine : IDisposable
{
    private readonly IWavePlayer outputDevice;
    private readonly MixingSampleProvider mixer;

    public static readonly AudioPlaybackEngine Instance = new AudioPlaybackEngine(44100, 2);

    private AudioPlaybackEngine(int sampleRate = 44100, int channelCount = 2)
    {
        outputDevice = new WaveOutEvent();
        mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channelCount));
        mixer.ReadFully = true;
        outputDevice.Init(mixer);
        outputDevice.Play();
    }

    internal void PlaySound(string fileName)
    {
        var input = new AudioFileReader(fileName);
        AddMixerInput(new AutoDisposeFileReader(input));
    }
    
    internal void PlaySound(CachedSound sound)
    {
        AddMixerInput(new CachedSoundSampleProvider(sound));
    }

    private ISampleProvider ConvertToRightChannelCount(ISampleProvider input)
    {
        if (input.WaveFormat.Channels == mixer.WaveFormat.Channels)
        {
            return input;
        }

        if (input.WaveFormat.Channels == 1 && mixer.WaveFormat.Channels == 2)
        {
            return new MonoToStereoSampleProvider(input);
        }

        throw new NotImplementedException("Not yet implemented this channel count conversion");
    }

    private void AddMixerInput(ISampleProvider input)
    {
        mixer.AddMixerInput(ConvertToRightChannelCount(input));
    }

    public void Dispose()
    {
        outputDevice.Dispose();
    }

    
    internal class CachedSound
    {
        internal float[] AudioData { get; private set; }
        internal WaveFormat WaveFormat { get; private set; }

        internal CachedSound(string audioFileName)
        {
            using var audioFileReader = new AudioFileReader(audioFileName);

            // could add resampling in here if required
            WaveFormat = audioFileReader.WaveFormat;
            List<float> wholeFile = new List<float>((int)(audioFileReader.Length / 4));
            float[] readBuffer = new float[audioFileReader.WaveFormat.SampleRate * audioFileReader.WaveFormat.Channels];
            int samplesRead;
            while ((samplesRead = audioFileReader.Read(readBuffer, 0, readBuffer.Length)) > 0)
            {
                wholeFile.AddRange(readBuffer.Take(samplesRead));
            }

            AudioData = wholeFile.ToArray();
        }
    }

    private class CachedSoundSampleProvider(CachedSound cachedSound) : ISampleProvider
    {
        private long position;

        public int Read(float[] buffer, int offset, int count)
        {
            var availableSamples = cachedSound.AudioData.Length - position;
            var samplesToCopy = Math.Min(availableSamples, count);
            Array.Copy(cachedSound.AudioData, position, buffer, offset, samplesToCopy);
            position += samplesToCopy;
            return (int)samplesToCopy;
        }

        public WaveFormat WaveFormat
        {
            get { return cachedSound.WaveFormat; }
        }
    }

    private class AutoDisposeFileReader(AudioFileReader reader) : ISampleProvider
    {
        private bool isDisposed;

        public WaveFormat WaveFormat { get; private set; } = reader.WaveFormat;

        public int Read(float[] buffer, int offset, int count)
        {
            if (isDisposed)
                return 0;
            int read = reader.Read(buffer, offset, count);
            if (read == 0)
            {
                reader.Dispose();
                isDisposed = true;
            }
            
            return read;
        }
    }
}