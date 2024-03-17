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
    public TimeSpan Length { get; private init; }


    // Internal fields.
    internal float[] Samples { get; private init; }


    // Constructors.
    internal Sound(string filePath, AudioEngine audioEngine) 
    {
        AudioFileReader Reader = null!;
        ISampleProvider Sampler;
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

        if (!AudioEngine.ActiveEngine.WaveFormat.Equals(Sampler.WaveFormat))
        {
            throw new AssetLoadException(filePath,
                $"Wrong wave format: {Sampler.WaveFormat.Encoding} " +
                $"(SampleRate:{Sampler.WaveFormat.SampleRate} Channels:{Sampler.WaveFormat.Channels})");
        }

        try
        {
            Length = Reader.TotalTime;
            Samples = new float[(int)Math.Ceiling(Reader.TotalTime.TotalSeconds
                * AudioEngine.ActiveEngine.WaveFormat.SampleRate * AudioEngine.ActiveEngine.WaveFormat.Channels)];
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
}