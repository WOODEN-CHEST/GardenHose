using GardenHoseEngine;
using GardenHoseEngine.Audio;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Player.Sound;

internal class WorldSound
{
    // Internal fields.
    internal ISoundSource? Source { get; }

    internal bool IsLooped
    {
        get => _instance.IsLooped;
        set => _instance.IsLooped = value;
    }

    internal float Volume { get; set; } = SoundInstance.VOLUME_MAX;
    internal float Pan { get; set; } = SoundInstance.PAN_MIDDLE;
    internal double Speed { get; set; } = SoundInstance.SPEED_DEFAULT;
    internal int? LowPassCutoffFrequency { get; set; } = null;
    internal int? HighPassCutoffFrequency { get; set; } = null;

    internal EventHandler? SoundFinished;

    internal const float POSITION_PAN_DIVIDER = 1000f;
    internal const float VOLUME_DISTANCE_MULTIPLIER = 0.0015f;
    internal const float VOLUME_SPEED_MULTIPLIER = 0.01f;
    internal const float LOW_PASS_DISTANCE_MULITPLIER = 0.01f;
    internal const float LOW_PASS_MIN_DISTANCE = 400f;
    internal const float LOW_PASS_START_FREQUENCY = 20000;
    internal const float LOW_PASS_MIN_FREQUENCY = 400;


    // Private fields.
    private readonly SoundInstance _instance;


    // Constructors.
    internal WorldSound(ISoundSource? source, SoundInstance instance)
    {
        Source = source;
        _instance = instance ?? throw new ArgumentNullException(nameof(instance));
        _instance.SoundFinished += OnSoundFinishEvent;
    }


    // Internal methods.
    internal void Play() => _instance.Play();

    internal void Stop() => _instance.Stop();

    internal void Tick(GHGameTime time, WorldPlayer player)
    {
        if (Source == null)
        {
            _instance.Volume = Volume;
            _instance.Pan = Pan;
            _instance.Speed = Speed;
            _instance.HighPassCutoffFrequency = HighPassCutoffFrequency;
            _instance.LowPassCutoffFrequency = LowPassCutoffFrequency;
            return;
        }

        _instance.Volume = Volume / Math.Max((Source.SoundSourcePosition - player.Camera.CameraCenter).Length() * VOLUME_DISTANCE_MULTIPLIER, 1f);
        _instance.Pan = Pan + (Source.SoundSourcePosition.X - player.Camera.CameraCenter.X) / POSITION_PAN_DIVIDER;

        float MotionDot = Vector2.Dot(GHMath.NormalizeOrDefault(player.SpaceShip.Motion), GHMath.NormalizeOrDefault(Source.SoundSourceMotion));
        float RelativeMotionSpeed = player.SpaceShip.Motion.Length() - Source.SoundSourceMotion.Length();

        _instance.Speed = Speed + (MotionDot * RelativeMotionSpeed * VOLUME_SPEED_MULTIPLIER);

        float DistanceBeteenSourceAndCamera = (Source.SoundSourcePosition - player.Camera.CameraCenter).Length();
        _instance.LowPassCutoffFrequency = DistanceBeteenSourceAndCamera < LOW_PASS_MIN_DISTANCE ? null :
            (int)Math.Max(LOW_PASS_START_FREQUENCY / Math.Max(1f, (DistanceBeteenSourceAndCamera - LOW_PASS_MIN_DISTANCE)
            * LOW_PASS_DISTANCE_MULITPLIER), LOW_PASS_MIN_FREQUENCY);
    }


    // Private methods.
    private void OnSoundFinishEvent(object? sender, EventArgs args)
    {
        SoundFinished?.Invoke(this, args);
    }
}