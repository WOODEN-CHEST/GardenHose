using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Audio;

public class Sound
{
    // Public fields.
    public readonly TimeSpan Length;


    // Internal fields.
    internal WaveFormat Format => SourceAudioEngine.WaveFormat;
    internal readonly float[] Samples;
    internal readonly AudioEngine SourceAudioEngine;


    // Constructors.
    internal Sound(string filePath, AudioEngine audioEngine) 
    {
        SourceAudioEngine = audioEngine ?? throw new ArgumentNullException(nameof(audioEngine));

        AudioFileReader Reader = null!;
        ISampleProvider Sampler = null!;
        try
        {
            Reader = new(filePath);
            Sampler = Reader.ToSampleProvider();
        }
        catch (IOException e)
        {
            Reader?.Dispose();
            throw new AssetLoadException(filePath, $"Couldn't read audio file. {e}");
        }

        if (!AreWaveFormatsSame(SourceAudioEngine.WaveFormat, Sampler.WaveFormat))
        {
            throw new AssetLoadException(filePath,
                $"Wrong wave format: {Sampler.WaveFormat.Encoding} " +
                $"(SampleRate:{Sampler.WaveFormat.SampleRate} Channels:{Sampler.WaveFormat.Channels})");
        }

        try
        {
            Length = Reader.TotalTime;
            Samples = new float[(int)Reader.TotalTime.TotalSeconds
                * SourceAudioEngine.WaveFormat.SampleRate * SourceAudioEngine.WaveFormat.Channels];
            Sampler.Read(Samples, 0, Samples.Length);
        }
        catch (IOException e)
        {
            throw new AssetLoadException(filePath, $"Couldn't read audio file. {e}");
        }
    }


    // Methods.
    public SoundInstance CreateSoundInstance(TimeSpan? startTime = null,
        float volume = SoundInstance.VOLUME_MAX,
        float pan = SoundInstance.PAN_MIDDLE,
        double speed = SoundInstance.SPEED_DEFAULT,
        int? lowPassCutoffFrequency = null,
        int? highPassCutoffFrequency = null)
    {
        return new SoundInstance(this, startTime, volume, pan, speed, lowPassCutoffFrequency, highPassCutoffFrequency);
    }


    // Private methods.
    private bool AreWaveFormatsSame(WaveFormat format1, WaveFormat format2)
    {
        return (format1.SampleRate == format2.SampleRate)
            && (format1.Channels == format2.Channels)
            && (format1.Encoding == format2.Encoding);
    }
}