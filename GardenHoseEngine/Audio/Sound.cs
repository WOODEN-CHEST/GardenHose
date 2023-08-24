using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Audio;

public class Sound
{
    // Internal fields.
    internal WaveFormat Format => SourceAudioEngine.Format;
    internal readonly float[] Samples;
    internal readonly AudioEngine SourceAudioEngine;


    // Constructors.
    internal Sound(string filePath, AudioEngine ghEngine) 
    {
        SourceAudioEngine = ghEngine ?? throw new ArgumentNullException(nameof(ghEngine));

        try
        {
            using AudioFileReader Reader = new(filePath);
            ISampleProvider Sampler = Reader.ToSampleProvider();
            if (!AreWaveFormatsSame(SourceAudioEngine.Format, Sampler.WaveFormat))
            {
                throw new AssetLoadException(filePath, 
                    $"Wrong wave format: {Sampler.WaveFormat.Encoding} " +
                    $"(SampleRate:{Sampler.WaveFormat.SampleRate} Channels:{Sampler.WaveFormat.Channels})");
            }

            int SampleCount = (int)((Reader.TotalTime.TotalSeconds + 0.05d) 
                * SourceAudioEngine.Format.SampleRate * SourceAudioEngine.Format.Channels);
            Samples = new float[SampleCount];

            Sampler.Read(Samples, 0, Samples.Length);
        }
        catch (Exception e)
        {
            throw new AssetLoadException(filePath, e.ToString());
        }
    }


    // Private methods.
    private bool AreWaveFormatsSame(WaveFormat format1, WaveFormat format2)
    {
        return (format1.SampleRate == format2.SampleRate)
            && (format1.Channels == format2.Channels)
            && (format1.Encoding == format2.Encoding);
    }
}