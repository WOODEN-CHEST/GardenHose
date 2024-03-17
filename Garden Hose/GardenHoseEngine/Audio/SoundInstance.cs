using NAudio.Dsp;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Tests.Audio;

namespace GardenHoseEngine.Audio;

public class SoundInstance
{
    // Fields.
    public const float PAN_LEFT = 0f;
    public const float PAN_MIDDLE = 1f;
    public const float PAN_RIGHT = 2f;
    public const float VOLUME_MIN = 0f;
    public const float VOLUME_MAX = 1f;
    public const double SPEED_MIN = -10;
    public const double SPEED_DEFAULT = 1d;
    public const double SPEED_MAX = 10d;

    public readonly Sound SourceSound;
    public SoundInstanceState State => _properties.State;

    public bool IsLooped
    {
        get => _properties.IsLooped;
        set
        {
            lock (this) { _properties.IsLooped = value; }
        }
    }

    public float Volume
    {
        get => _properties.Volume;
        set
        {
            lock (this)
            {
                _properties.Volume = Math.Clamp(value, VOLUME_MIN, VOLUME_MAX);
            }
        }
    }

    public float Pan
    {
        get => _properties.Pan;
        set
        {
            lock (this)
            {
                _properties.Pan = Math.Clamp(value, PAN_LEFT,  PAN_RIGHT);
            }
        }
    }

    public double Speed
    {
        get => _properties.Speed;
        set
        {
            lock (this)
            {
                _properties.Speed = Math.Clamp(value, SPEED_MIN, SPEED_MAX);
            }
        }
    }

    public int? LowPassCutoffFrequency
    {
        get => _properties.LowPassCutoffFrequency;
        set
        {
            lock (this)
            {
                _properties.LowPassCutoffFrequency = value.HasValue ?
                    Math.Clamp(value!.Value, 0, AudioEngine.ActiveEngine.WaveFormat.SampleRate / 2)
                    : null;
            }
        }
    }

    public int? HighPassCutoffFrequency
    {
        get => _properties.HighPassCutoffFrequency;
        set
        {
            lock (this)
            {
                _properties.HighPassCutoffFrequency = value.HasValue ?
                    Math.Clamp(value!.Value, 0, AudioEngine.ActiveEngine.WaveFormat.SampleRate / 2)
                    : null;
                _filterLeft.SetHighPassFilter(AudioEngine.ActiveEngine.WaveFormat.SampleRate, (float)value!, 2f);
                _filterRight.SetHighPassFilter(AudioEngine.ActiveEngine.WaveFormat.SampleRate, (float)value!, 2f);
            }
        }
    }

    public event EventHandler? SoundFinished;
    public event EventHandler? SoundLooped;


    // Private fields.
    private const float FILTER_ORDER = 3f;

    private int _sourceIndex = 0;
    private double _doubleSourceIndex = 0d;
    private SoundInstanceProperties _properties;
    private SoundInstanceProperties _appliedProperties;
    private int? _newSourceIndex = null;

    private readonly BiQuadFilter _filterLeft = BiQuadFilter.LowPassFilter(0, 0, 0);
    private readonly BiQuadFilter _filterRight = BiQuadFilter.LowPassFilter(0, 0, 0);


    // Constructors.
    internal SoundInstance(Sound sourceSound,
        TimeSpan? startTime,
        float volume,
        float pan,
        double speed,
        int? lowPassCutoffFrequency,
        int? highPassCutoffFrequency)
    {
        SourceSound = sourceSound ?? throw new ArgumentNullException(nameof(sourceSound));

        _properties = new();
        
        if (startTime.HasValue)
        {
            SetTime(startTime.Value);
        }
        _properties.Volume = volume;
        _properties.Pan = pan;
        _properties.Speed = speed;
        _properties.LowPassCutoffFrequency = lowPassCutoffFrequency;
        _properties.HighPassCutoffFrequency = highPassCutoffFrequency;

        _appliedProperties = _properties;
    }


    // Methods.
    public void Play()
    {
        AudioEngine.ActiveEngine.AddSound(this);
        lock (this)
        {
            _properties.State = SoundInstanceState.Playing;
        }
    }

    public void Stop()
    {
        AudioEngine.ActiveEngine.RemoveSound(this);
        lock (this)
        {
            _properties.State = SoundInstanceState.Stopped;
        }
    }

    public void SetTime(double factor)
    {
        if (_properties.State == SoundInstanceState.Stopped)
        {
            throw new InvalidOperationException("Sound is stopped and cannot have its time modified.");
        }

        factor = Math.Clamp(factor, 0f, 1f);
        lock (this)
        {
            _newSourceIndex = (int)((SourceSound.Samples.Length - 1) * factor);
        }
    }

    public void SetTime(TimeSpan time)
    {
        SetTime(time / SourceSound.Length);
    }


    // Private methods.
    private void EndSound()
    {
        lock (this)
        {
            _properties.State = SoundInstanceState.Stopped;
        }
        AudioEngine.ActiveEngine.RemoveSound(this);
        SoundFinished?.Invoke(this, EventArgs.Empty);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FillBufferWithSilence(float[] buffer, int offset, int count)
    {
        for (int i = offset; i < offset + count; i++)
        {
            buffer[i] = 0f;
        }
    }

    /* Effects. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PanPass(float[] buffer, int count, float pan)
    {
        const float MIN_MAIN_VOLUME = 0.5f;
        const float MAX_SECONDARY_VOLUME = 0.5f;
        const float MAX_VOLUME_BOOST = 0.4f;

        bool PannedToLeft = pan < PAN_MIDDLE;
        pan = PannedToLeft ? pan : (PAN_RIGHT - pan);
        float VolumeBoost = 1f + MAX_VOLUME_BOOST * (1f - pan);

        if (PannedToLeft)
        {
            float LeftVolumeInLeft = (MIN_MAIN_VOLUME + (MAX_SECONDARY_VOLUME * pan)) * VolumeBoost;
            float RightVolumeInLeft = VOLUME_MAX - LeftVolumeInLeft;
            float RightVolume = pan;

            for (int i = 0; i < count - 1; i += 2)
            {
                buffer[i] = (buffer[i] * LeftVolumeInLeft) + (buffer[i + 1] * RightVolumeInLeft);
                buffer[i + 1] *= RightVolume;
            }
        }
        else
        {
            float RightVolumeInRight = (MIN_MAIN_VOLUME + (MAX_SECONDARY_VOLUME * pan)) * VolumeBoost;
            float LeftVolumeInRight = VOLUME_MAX - RightVolumeInRight;
            float LeftVolume = pan;

            for (int i = 0; i < count - 1; i += 2)
            {
                buffer[i] *= LeftVolume;
                buffer[i + 1] = (buffer[i + 1] * RightVolumeInRight) + (buffer[i] * LeftVolumeInRight);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void BiQuadFilterPass(float[] buffer, int count)
    {
        for (int i = 0; i < (count - 1); i += 2)
        {
            buffer[i] = _filterLeft.Transform(buffer[i]);
            buffer[i + 1] = _filterRight.Transform(buffer[i + 1]);
        }
    }


    /* Reading. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void NormalRead(float[] buffer, int offset, int count)
    {
        int SamplesRead = Math.Min(count, (SourceSound.Samples.Length - 1) - _sourceIndex);

        for (int i = offset; i < (offset + SamplesRead); i++, _sourceIndex++)
        {
            buffer[i] = SourceSound.Samples[_sourceIndex] * _appliedProperties.Volume;
        }

        _doubleSourceIndex = _sourceIndex / 2d;

        if (SamplesRead < count)
        {
            if (_appliedProperties.IsLooped)
            {
                SoundLooped?.Invoke(this, EventArgs.Empty);
                _sourceIndex = 0;
                NormalRead(buffer, offset + SamplesRead, count - SamplesRead);
            }
            else
            {
                EndSound();
                FillBufferWithSilence(buffer, offset + SamplesRead, count - SamplesRead);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void SpeedResampleRead(float[] buffer, int count)
    {
        // Variables.
        int BufferIndex = 0;
        float Factor1, Factor2;

        while (BufferIndex < count)
        {
            // Check for end of sound buffer.
            _sourceIndex = (int)_doubleSourceIndex * 2;
            if (((SourceSound.Samples.Length - _sourceIndex) < 4) || _sourceIndex < 0)
            {
                if (_appliedProperties.IsLooped)
                {
                    SoundLooped?.Invoke(this, EventArgs.Empty);
                    _doubleSourceIndex = _sourceIndex > 0 ? 0d : ((SourceSound.Samples.Length -4) / 2);
                    continue;
                }
                else
                {
                    EndSound();
                    FillBufferWithSilence(buffer, BufferIndex + 1, count - BufferIndex);
                    break;
                }
            }

            // Set values.
            Factor2 = (float)(_doubleSourceIndex % 1d);
            Factor1 = 1f - Factor2;

            // Left Channel.
            buffer[BufferIndex] = (SourceSound.Samples[_sourceIndex] * Factor1
                + SourceSound.Samples[_sourceIndex + 2] * Factor2) * _appliedProperties.Volume;

            // Right Channel
            buffer[BufferIndex + 1] = (SourceSound.Samples[_sourceIndex + 1] * Factor1
                + SourceSound.Samples[_sourceIndex + 3] * Factor2) * _appliedProperties.Volume;

            // Increment values.
            BufferIndex += 2;
            _doubleSourceIndex += _appliedProperties.Speed;
        }
    }

    internal void GetSamples(float[] buffer, int sampleCount)
    {
        // Copy properties so they do not change during
        // processing if other threads change them in that time.
        int? NewSourceIndex;
        lock (this) 
        {
            NewSourceIndex = _newSourceIndex;
            _newSourceIndex = null;
            _appliedProperties = _properties; 
        }

        // Check index change.
        if (NewSourceIndex.HasValue)
        {
            _sourceIndex = NewSourceIndex.Value;
            _doubleSourceIndex = _sourceIndex / 2d;
        }

        // Read samples until desired count is reached.
        if (Speed == SPEED_DEFAULT)
        {
            NormalRead(buffer, 0, sampleCount);
        }
        else
        {
            SpeedResampleRead(buffer, sampleCount);
        }

        // Apply any existing effects.
        if (_appliedProperties.Pan != PAN_MIDDLE)
        {
            PanPass(buffer, sampleCount, _appliedProperties.Pan);
        }
        if (_appliedProperties.LowPassCutoffFrequency != null)
        {
            _filterLeft.SetLowPassFilter(AudioEngine.ActiveEngine.WaveFormat.SampleRate, 
                _appliedProperties.LowPassCutoffFrequency.Value, FILTER_ORDER);
            _filterRight.SetLowPassFilter(AudioEngine.ActiveEngine.WaveFormat.SampleRate,
                _appliedProperties.LowPassCutoffFrequency.Value, FILTER_ORDER);

            BiQuadFilterPass(buffer, sampleCount);
        }
        if (_appliedProperties.HighPassCutoffFrequency != null)
        {
            _filterLeft.SetHighPassFilter(AudioEngine.ActiveEngine.WaveFormat.SampleRate, 
                _appliedProperties.HighPassCutoffFrequency.Value, FILTER_ORDER);
            _filterRight.SetHighPassFilter(AudioEngine.ActiveEngine.WaveFormat.SampleRate,
                _appliedProperties.HighPassCutoffFrequency.Value, FILTER_ORDER);

            BiQuadFilterPass(buffer, sampleCount);
        }
    }
}