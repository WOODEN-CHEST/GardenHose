using GardenHose.Game.World;
using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Text;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace GardenHose.Game;

internal class GHGameDebugInfo : IDrawableItem
{
    // Fields.
    public bool IsVisible { get; set; }
    public Effect? Shader { get; set; }


    // Internal fields.
    internal bool IsDebugTextEnabled { get; set; } = false;
    internal bool IsDebugOverlaysEnabled { get; set; } = false;
    internal float UpdateTimeSeconds { get; set; } = 0.5f;
    internal float TickTime { get; private set; } = 0f;
    internal float FPS { get; private set; } = 0f;
    

    // Private fields.
    private readonly GHGame _game;
    private readonly GameWorld _world;
    private readonly GHGameTime _gameTime;

    private float _timeSinceUpdate = 0f;
    private int _tickCountSinceUpdate = 0;
    private FittedText _infoText;

    /* User input. */
    private readonly IInputListener _debugTextToggleListener;
    private readonly IInputListener _debugOverlaysToggleListener;

    /* Tick time. */
    private readonly Stopwatch _tickTimeMeasurer = new();
    private float _accumilatedTickTime = 0f;


    // Constructors.
    internal GHGameDebugInfo(GHGame game, GameWorld world, GHGameTime time)
    {
        _game = game ?? throw new ArgumentNullException(nameof(game));
        _world = world ?? throw new ArgumentNullException(nameof(world));
        _gameTime = time ?? throw new ArgumentNullException(nameof(time));

        _debugTextToggleListener = IInputListener.CreateSingleKey(KeyCondition.OnRelease, Keys.F3);
        _debugOverlaysToggleListener = IInputListener.CreateSingleKey(KeyCondition.OnRelease, Keys.F4);
        _infoText = new(string.Empty, GH.GeeichFont)
        {
            FittingSizePixels = Display.VirtualSize,
            TextOrigin = Origin.TopLeft
        };
    }



    // Internal methods.
    internal void StartTickMeasure()
    {
        _tickTimeMeasurer.Reset();

        if (_debugOverlaysToggleListener.Listen())
        {
            IsDebugOverlaysEnabled = !IsDebugOverlaysEnabled;
            _game.World.IsDebugInfoDrawn = IsDebugOverlaysEnabled;
        }
        if (_debugTextToggleListener.Listen())
        {
            IsDebugTextEnabled = !IsDebugTextEnabled;
        }

        _tickTimeMeasurer.Start();
    }

    internal void StopTickMeasure(IProgramTime time)
    {
        _tickTimeMeasurer.Stop();
        _accumilatedTickTime += (float)_tickTimeMeasurer.Elapsed.TotalSeconds;

        _tickCountSinceUpdate++;
        _timeSinceUpdate += time.PassedTimeSeconds; 
        if (_timeSinceUpdate < UpdateTimeSeconds)
        {
            return;
        }

        TickTime = _accumilatedTickTime / _tickCountSinceUpdate;

        FPS = Display.FPS;

        _timeSinceUpdate = 0f;
        _tickCountSinceUpdate = 0;
        _accumilatedTickTime = 0f;
    }

    internal string GetDebugInfo()
    {
        throw new NotImplementedException();
    }

    // Private methods.
    public void Draw(IDrawInfo info)
    {
        if (!IsDebugTextEnabled) return;

        _infoText.Text = $"GH Version {GH.GameVersion}\n" +
            $"FPS: {FPS.ToString("0.00")}\n" +
            $"Entity Count: {_world.EntityCount}\n" +
            $"Tick Time: {(TickTime * 1000f).ToString("0.0000")}" +
            $" ms ({(TickTime / _gameTime.MaxPassedWorldTime * 100f).ToString("0.000")}% of allowed)\n";
        _infoText.Draw(info);
    }
}