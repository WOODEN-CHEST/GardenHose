using System;
using System.Collections.Concurrent;
using GardenHose.Game.World;
using System.Diagnostics;
using GardenHoseEngine.Frame;
using GardenHose.Frames;
using GardenHose.Frames.InGame;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Animation;
using GardenHose.Game.World.Planet;
using GardenHose.Game.World;

namespace GardenHose.Game;

internal class GHGame
{
    // Fields.
    public ILayer BackgroundLayer { get; private init; } = new Layer("background");

    public ILayer BottomItemLayer { get; private init; } = new Layer("items bottom");

    public ILayer TopItemLayer { get; private init; } = new Layer("items top");

    public ILayer UILayer { get; private init; } = new Layer("ui");

    public GameWorld World { get; private set; }

    public InGameFrame ParentFrame;

    public GHGameAssetManager AssetManager { get; private init; }



    // Internal fields
    internal const double SecondsPerTick = 0.05d;

    internal float SimulationSpeed
    {
        get => _simulationSpeed;
        set
        {
            lock (this)
            {
                _simulationSpeed = Math.Clamp(value, 0f, 10f);
            }
        }
    }

    internal bool IsPaused
    {
        get => _isPaused;
        set
        {
            lock (this)
            {
                _isPaused = value;
            }
        }
    }

    internal bool IsRunning { get; private set; }

    internal bool IsRunningSlowly { get; private set; } = false;


    // Private fields.
    /* Ticking. */
    private bool _isPaused = false;

    private float _simulationSpeed = 1f;
    private float _passedTimeSeconds = 0f;
    private const float MAXIMUM_PASSED_TIME_SECONDS = 0.05f;
    private const float MINIMUM_PASSED_TIME_SECONDS = 1f / 20f;


    // Constructors.
    internal GHGame(InGameFrame parentFrame, GameWorldSettings worldSettings)
    {
        ParentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
        ParentFrame.AddLayer(BackgroundLayer);
        ParentFrame.AddLayer(BottomItemLayer);
        ParentFrame.AddLayer(TopItemLayer);
        ParentFrame.AddLayer(UILayer);

        AssetManager = new(ParentFrame);

        World = new(this, worldSettings);
    }


    // Inherited methods.
    internal void OnStart()
    {
        if (IsRunning) return;

        IsRunning = true;
        World.Start();
    }

    internal void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        _passedTimeSeconds += GameFrameManager.PassedTimeSeconds;

        if (_passedTimeSeconds < MINIMUM_PASSED_TIME_SECONDS)
        {
            return;
        }

        IsRunningSlowly = _passedTimeSeconds > MAXIMUM_PASSED_TIME_SECONDS;
        if (IsRunningSlowly)
        {
            _passedTimeSeconds = MAXIMUM_PASSED_TIME_SECONDS;
        }
        _passedTimeSeconds *= SimulationSpeed;

        World.PassedTimeSeconds = _passedTimeSeconds;
        World.Tick();
        _passedTimeSeconds = 0f;
    }

    internal void OnEnd()
    {
        if (!IsRunning)
        {
            return;
        }

        World.End();
        IsRunning = false;
    }
}