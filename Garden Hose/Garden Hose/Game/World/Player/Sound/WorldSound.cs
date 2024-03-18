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

    internal float Volume { get; set; }
    internal float Pan { get; set; }
    internal double Speed { get; set; }
    internal int? LowPassCutoffFrequency { get; set; }
    internal int? HighPassCutoffFrequency { get; set; }

    internal EventHandler? SoundFinished;

    internal const float POSITION_PAN_DIVIDER = 300f;
    internal const float VOLUME_DISTANCE_MULTIPLIER = 0.003f;
    internal const float VOLUME_SPEED_MULTIPLIER = 0.01f;


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

        _instance.Volume = 1f / Math.Min((Source.SoundSourcePosition - player.Camera.CameraCenter).Length() * VOLUME_DISTANCE_MULTIPLIER, 1);
        _instance.Pan = (Source.SoundSourcePosition.X - player.Camera.CameraCenter.X) / POSITION_PAN_DIVIDER;
        _instance.Speed = 1f + (Vector2.Dot(GHMath.NormalizeOrDefault(player.SpaceShip.Motion), GHMath.NormalizeOrDefault(Source.SoundSourceMotion))
            * player.SpaceShip.Motion.Length() - Source.SoundSourceMotion.Length() * VOLUME_SPEED_MULTIPLIER);
    }


    // Private methods.
    private void OnSoundFinishEvent(object? sender, EventArgs args)
    {
        SoundFinished?.Invoke(this, args);
    }
}