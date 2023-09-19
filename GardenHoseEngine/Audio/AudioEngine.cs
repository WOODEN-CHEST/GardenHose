﻿using NAudio.CoreAudioApi;
using NAudio.Dsp;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace GardenHoseEngine.Audio;

public class AudioEngine : IDisposable, ISampleProvider
{
    // Static fields.
    public const int MAX_SOUNDS = 128;
    public const int AUDIO_LATENCY_MS = 30;
    public TimeSpan ExecutionTime
    {
        get { lock (this) { return _executionTime; } }
    }


    // Fields.
    public WaveFormat WaveFormat => _format;

    // Private fields.
    private readonly WaveFormat _format = WaveFormat.CreateIeeeFloatWaveFormat(44100, 2);
    private readonly WasapiOut _outputDevice;
    private readonly List<SoundInstance> _sounds = new(MAX_SOUNDS);
    private readonly ConcurrentQueue<SoundInstance> _soundsToAdd = new();
    private readonly ConcurrentQueue<SoundInstance> _soundsToRemove = new();
    private float[] _soundBuffer;
    private TimeSpan _executionTime;


    // Constructors.
    internal AudioEngine()
    {
        try
        {
            _outputDevice = new(AudioClientShareMode.Shared, true, AUDIO_LATENCY_MS);

            _outputDevice.Init(this);
            _outputDevice.Play();
        }
        catch (Exception e)
        {
            throw new Exception($"Failed to initialize audio engine! {e}");
        }
    }


    // Internal methods.
    internal void AddSound(SoundInstance sound) => _soundsToAdd.Enqueue(sound);

    internal void RemoveSound(SoundInstance sound) => _soundsToRemove.Enqueue(sound);


    // Private methods.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void EnsureBuffer(int requestedSize)
    {
        if ((_soundBuffer == null) || (requestedSize > _soundBuffer.Length))
        {
            _soundBuffer = new float[requestedSize];
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int FillBufferWithSilence(float[] buffer, int offset, int count)
    {
        for (int i = offset; i < (offset + count); i++)
        {
            buffer[i] = 0f;
        }
        return count;
    }

    private void AddQueuedSounds()
    {
        while (_soundsToAdd.TryDequeue(out var Sound))
        {
            _sounds.Add(Sound);
        }
    }

    private void RemoveQueuedSounds()
    {
        while (_soundsToRemove.TryDequeue(out var Sound))
        {
            _sounds.Remove(Sound);
        }
    }


    // Inherited methods.
    public void Dispose()
    {
        _outputDevice?.Dispose();
    }

    public int Read(float[] buffer, int offset, int count)
    {
        try
        {
            // Ensure buffer capacity, add queued sounds, start time measure.
            DateTime StartTime = DateTime.UtcNow;
            EnsureBuffer(count);
            AddQueuedSounds();

            // Early exit (fill with silence).
            if (_sounds.Count == 0)
            {
                return FillBufferWithSilence(buffer, offset, count);
            }

            // Overwrite buffer data with the first sound.
            _sounds[0].GetSamples(_soundBuffer, count);
            for (int i = offset, Source = 0; i < (offset + count); i++, Source++)
            {
                buffer[i] = _soundBuffer[Source];
            }


            // Add remaining sounds.
            for (int SoundIndex = 1; SoundIndex < _sounds.Count; SoundIndex++)
            {
                _sounds[SoundIndex].GetSamples(_soundBuffer, count);
                for (int Target = offset, Source = 0; Target < (offset + count); Target++, Source++)
                {
                    buffer[Target] += _soundBuffer[Source];
                }
            }

            // Remove queued sounds, stop time measure.
            RemoveQueuedSounds();
            _executionTime = DateTime.UtcNow - StartTime;
            return count;
        }
        catch (Exception e)
        {
            _outputDevice.Dispose();
            Console.WriteLine("aaa");
            return 0;
        }
    }
}