using System;
using System.Collections.Concurrent;
using GardenHoseServer.World;
using System.Diagnostics;
using GardenHoseEngine.Frame;
using GardenHose.Frames;
using GardenHose.Frames.InGame;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Animation;

namespace GardenHose.Game;

internal class GHGame : FrameComponentManager<InGameFrame>
{
    // Fields
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

    internal ILayer BackgroundLayer { get; private init; } = new Layer("background");

    internal ILayer ItemLayer { get; private init; } = new Layer("items");

    internal ILayer UILayer { get; private init; } = new Layer("ui");

    internal GameWorld World;

    internal SpriteItem PlanetItem;


    // Private fields.
    /* Ticking. */
    private bool _isPaused = false;

    private float _simulationSpeed = 1f;
    private float _passedTimeSeconds = 0f;
    private const float MAXIMUM_PASSED_TIME_SECONDS = 0.05f;


    // Constructors.
    internal GHGame(InGameFrame frame) : base(frame)
    {
        World = new(this);

        ParentFrame.AddLayer(BackgroundLayer);
        ParentFrame.AddLayer(ItemLayer);
        ParentFrame.AddLayer(UILayer);
    }


    // Inherited methods.
    internal override void Load(AssetManager assetManager)
    {
        SpriteAnimation BallAnim = new(0f, ParentFrame, assetManager, Origin.Center, "test/ball");
        PlanetItem = new(GH.Engine.Display, BallAnim);
        ItemLayer.AddDrawableItem(PlanetItem);
    }

    internal override void OnStart()
    {

        if (IsRunning) return;

        IsRunning = true;
        World.OnStart(ItemLayer);
    }

    internal override void OnEnd()
    {
        if (!IsRunning)
        {
            return;
        }

        IsRunning = false;
    }

    internal override void Update(float passedTimeSeconds)
    {
        if (!IsRunning)
        {
            return;
        }

        _passedTimeSeconds = passedTimeSeconds;
        IsRunningSlowly = _passedTimeSeconds > MAXIMUM_PASSED_TIME_SECONDS;
        if (IsRunningSlowly)
        {
            _passedTimeSeconds = MAXIMUM_PASSED_TIME_SECONDS;
        }
        _passedTimeSeconds *= SimulationSpeed;


        World.Tick(_passedTimeSeconds);
    }
}