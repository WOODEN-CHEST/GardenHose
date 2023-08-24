using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Audio;

public class SoundInstance : ISampleProvider
{
    // Fields.
    


    public WaveFormat WaveFormat => _source.Format;

    public float Volume
    {
        get => _volume;
        set
        {
            _volume = Math.Clamp(value, AudioEngine.VOLUME_MIN, AudioEngine.VOLUME_MAX);
            IsVolumePassNeeded = _volume != AudioEngine.VOLUME_MAX;
        }
    }
    public bool IsVolumePassNeeded { get; private set; } = false;

    public float Pan
    {
        get => _pan;
        set
        {
            _pan = Math.Clamp(value, AudioEngine.PAN_LEFT, AudioEngine.PAN_RIGHT);
            IsPanPassNeeded = _pan != AudioEngine.PAN_MIDDLE;
        }
    }
    public bool IsPanPassNeeded { get; private set; } = false;

    public float Pitch
    {
        get => _pitch;
        set
        {
            _pitch = Math.Clamp(value, AudioEngine.PITCH_MIN, AudioEngine.PITCH_MAX);
            IsPitchPassNeeded = _pitch != AudioEngine.PITCH_DEFAULT;
        }
    }
    public bool IsPitchPassNeeded { get; private set; } = false;

    public int? LowPassCutoffFrequency
    {
        get => _lowPassCutoffFrequency;
        set
        {
            IsLowPassNeeded = value.HasValue;

            _lowPassCutoffFrequency = IsLowPassNeeded ? 
                Math.Clamp(value!.Value, 0, _source.Format.SampleRate / 2)
                : null;
        }
    }
    public bool IsLowPassNeeded { get; private set; } = false;

    public int? HighPassCutoffFrequency
    {
        get => _highPassCutoffFrequency;
        set
        {
            IsHighPassNeeded = value.HasValue;

            _highPassCutoffFrequency = IsHighPassNeeded ?
                Math.Clamp(value!.Value, 0, _source.Format.SampleRate / 2)
                : null;
        }
    }
    public bool IsHighPassNeeded { get; private set; } = false;

    public float Reverb
    {
        get => _reverb;
        set
        {
            throw new NotImplementedException("Reverb not yet implemented");
        }
    }
    public bool IsReverbPassNeeded { get; private set; } = false;

    public event EventHandler? SoundFinished;


    // Private fields.
    private readonly Sound _source;
    private readonly WaveFormat _waveFormat;
    private int _sourceIndex = 0;

    private float _volume = 1f;
    private float _pan = 0f;
    private float _pitch = 1f;
    private int? _lowPassCutoffFrequency = null;
    private int? _highPassCutoffFrequency = null;
    private float _reverb = 0f;


    // Constructors.
    public SoundInstance(Sound sourceSound)
    {
        _source = sourceSound ?? throw new ArgumentNullException(nameof(sourceSound));
    }


    // Methods.
    public void Stop()
    {

    }

    public void Continue()
    {

    }

    public void SetTime(double timeSeconds)
    {

    }


    // Inehrited methods.
    public int Read(float[] buffer, int offset, int count)
    {
        int ReadSamples = Math.Min(count, _source.Samples.Length - _sourceIndex);

        for (int i = offset; i < (offset + ReadSamples); i++, _sourceIndex++)
        {
            buffer[i] = _source.Samples[_sourceIndex] * _volume;
        }

        _source.SourceAudioEngine.ProcessSound(this, buffer, offset, ReadSamples);
        return ReadSamples;
    }
}