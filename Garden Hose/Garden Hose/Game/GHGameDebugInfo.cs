using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Text;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace GardenHose.Game;

internal class GHGameDebugInfo
{
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
    private bool _isDebugKeysEnabled;
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

        _debugTextToggleListener = KeyboardListenerCreator.SingleKey(KeyCondition.OnRelease, OnDebugTextKeyPressEvent, Keys.F3);
        _debugOverlaysToggleListener = KeyboardListenerCreator.SingleKey(KeyCondition.OnRelease, OnDebugTextKeyPressEvent, Keys.F4);
        _infoText = new(string.Empty, GH.GeeichFontLarge);
    }



    // Internal methods.
    internal void OnGameStart()
    {
        _debugOverlaysToggleListener.StartListening();
        _debugTextToggleListener.StartListening();
    }

    internal void OnGameEnd()
    {
        _debugTextToggleListener?.StopListening();
        _debugOverlaysToggleListener?.StopListening();
    }

    internal void StartTickMeasure()
    {
        _tickTimeMeasurer.Reset();
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

    internal void Draw(IDrawInfo info)
    {
        if (!IsDebugTextEnabled) return;

        _infoText.Text = $"GH Version {GH.GameVersion}\n" +
            $"FPS: {FPS.ToString("0.00")}\n" +
            $"Entity Count: {_world.EntityCount}\n" +
            $"Tick Time: {TickTime}sec ({(TickTime / _gameTime.MaxPassedWorldTime).ToString("0.00")}% of allowed)\n" +
            $"IsFallingBegind: {_game.GameTime.IsRunningSlowly}";
    }

    // Private methods.
    private void OnDebugTextKeyPressEvent(object? sender, EventArgs args)
    {
        IsDebugTextEnabled = !IsDebugTextEnabled;
    }

    private void OnDebugOverlaysKeyPressEvent(object? sender, EventArgs args)
    {
        IsDebugOverlaysEnabled = !IsDebugOverlaysEnabled;
        _game.World.IsDebugInfoDrawn = IsDebugOverlaysEnabled;
    }
}