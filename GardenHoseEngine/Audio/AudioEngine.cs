using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System.Runtime.CompilerServices;

namespace GardenHoseEngine.Audio;

internal class AudioEngine : IDisposable
{
    // Fields.
    public const int AUDIO_LATENCY_MS = 15;

    public const float PAN_LEFT = 0f;
    public const float PAN_MIDDLE = 1f;
    public const float PAN_RIGHT = 2f;

    public const float VOLUME_MIN = 0f;
    public const float VOLUME_MAX = 1f;

    public const float PITCH_MIN = 0f;
    public const float PITCH_DEFAULT = 1f;
    public const float PITCH_MAX = 2f;

    public readonly WaveFormat Format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);


    // Private fields.
    private readonly DirectSoundOut _outputDevice;
    private readonly MixingSampleProvider _waveMixer;
    private readonly BiQuadFilter _biQuadFilter;


    // Constructors.
    internal AudioEngine()
    {

        try
        {
            _waveMixer = new(Format);
            _waveMixer.ReadFully = true;

            _biQuadFilter = BiQuadFilter.AllPassFilter(Format.SampleRate, Format.SampleRate, 1f);

            _outputDevice = new(AUDIO_LATENCY_MS);
            _outputDevice.Init(_waveMixer);
            _outputDevice.PlaybackStopped += OnAudioPlaybackStop;
            _outputDevice.Play();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to initialize audio engine! {e}");
        }
    }


    // Internal methods.
    internal void PlaySound(SoundInstance sound)
    {
        _waveMixer.AddMixerInput(sound);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void ProcessSound(SoundInstance sound, float[] buffer, int offset, int count)
    {

        // Apply effects.
        if (sound.Pan != PAN_MIDDLE)
        {
            PanPass(buffer, offset, count, sound.Pan);
        }
        if (sound.Pitch != PITCH_DEFAULT)
        {
            PitchPass(buffer, offset, count, sound.Pitch);
        }
        if (sound.LowPassCutoffFrequency != null)
        {
            LowPass(buffer, offset, count, sound.LowPassCutoffFrequency.Value);
        }
        if (sound.HighPassCutoffFrequency != null)
        {
            HighPass(buffer, offset, count, sound.HighPassCutoffFrequency.Value);
        }
        if (sound.Reverb != 0f)
        {
            ReverbPass(buffer, offset, count, sound.Reverb);
        }
    }

    internal void OnAudioPlaybackStop(object? sender, EventArgs args)
    {

    }


    // Private methods.
    /* Individual effect passes */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PanPass(float[] buffer, int offset, int count, float pan)
    {
        //float LeftVolumeInleft;
        //float RightVolumeInleft;

        //float LeftVolumeInRight;
        //float RightVolumeInRight;

        //if (pan < 1f)
        //{
        //    LeftVolumeInRight = 0f;
        //    RightVolumeInRight = pan;

        //    LeftVolumeInleft = 0.5f + (0.5f * pan);
        //    RightVolumeInleft = 1 - LeftVolumeInleft;
        //}
        //else
        //{
        //    pan -= 1f;

        //    LeftVolumeInRight = 0f;
        //    RightVolumeInRight = pan;

        //    LeftVolumeInleft = 0.5f + (0.5f * pan);
        //    RightVolumeInleft = 1 - LeftVolumeInleft;
        //}

        const float MIN_MAIN_VOLUME = 0.5f;
        const float MAX_SECONDARY_VOLUME = 0.5f;

        if (pan < PAN_MIDDLE)
        {
            float LeftVolumeInLeft = MIN_MAIN_VOLUME + (MAX_SECONDARY_VOLUME * pan);
            float RightVolumeInLeft = 1f - LeftVolumeInLeft;
            float RightVolume = pan;

            for (int i = offset; i < (offset + count) - 1; i++)
            {
                buffer[i] = (buffer[i] * LeftVolumeInLeft) + (buffer[i + 1] * RightVolumeInLeft);
                buffer[++i] *= RightVolume;
            }
        }
        else
        {
            pan = 1f - (pan - 1f);

            float RightVolumeInRight = MIN_MAIN_VOLUME + (MAX_SECONDARY_VOLUME * pan);
            float LeftVolumeInRight = 1f - RightVolumeInRight;
            float LeftVolume = pan;

            for (int i = offset; i < (offset + count) - 1; i++)
            {
                buffer[i] = (buffer[i] * LeftVolumeInLeft) + (buffer[i + 1] * RightVolumeInLeft);
                buffer[++i] *= RightVolume;
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void PitchPass(float[] buffer, int offset, int count, float pitch)
    {
        throw new NotFiniteNumberException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void LowPass(float[] buffer, int offset, int count, int cutoffFrequency)
    {
        _biQuadFilter.SetLowPassFilter(Format.SampleRate, cutoffFrequency, 2f);
        
        for (int i = offset; i < (offset + count); i++)
        {
            buffer[i] = _biQuadFilter.Transform(buffer[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HighPass(float[] buffer, int offset, int count, int cutoffFrequency)
    {
        _biQuadFilter.SetHighPassFilter(Format.SampleRate, cutoffFrequency, 2f);

        for (int i = offset; i < (offset + count); i++)
        {
            buffer[i] = _biQuadFilter.Transform(buffer[i]);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ReverbPass(float[] buffer, int offset, int count, float amount)
    {
        throw new NotImplementedException("Reverb not supported.");
    }


    // Inherited methods.
    public void Dispose()
    {
        _outputDevice?.Dispose();
    }
}