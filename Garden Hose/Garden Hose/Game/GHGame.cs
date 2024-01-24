using System;
using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using GardenHose.Frames.InGame;
using GardenHose.Game.AssetManager;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework.Input;
using GardenHoseEngine.Frame.Item.Text;
using GardenHose.Frames.Global;
using GardenHoseEngine.Screen;
using GardenHoseEngine;
using System.Diagnostics;

namespace GardenHose.Game;

internal class GHGame
{
    // Fields.
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

    internal float UpdateTime { get; private set; } = 0f;

    internal GameBackground Background { get; private init; }


    // Private fields.
    private bool _isDebugTextEnabled = false;
    private bool _areEntityOverlaysEnabled = false;
    private bool _toggledDebugOption = false;

    private float _timeSinceInfoUpdate = 0f;
    private float _totalUpdateTime = 0f;
    private int _infoUpdateCount = 0;
    private const float INFO_UPDATE_TIME = 0.5f; // 0.5 seconds.


    /* Ticking. */
    private bool _isPaused = false;

    private float _simulationSpeed = 1f;
    private readonly GHGameTime _gameTime = new();
    private const float MAXIMUM_PASSED_TIME_SECONDS = 1f / 20;
    private const float MINIMUM_PASSED_TIME_SECONDS = 1f / 400f;


    /* Input listening. */
    private IInputListener _debugToggleListener;
    private IInputListener _entityOverlayToggleListener;
    private SimpleTextBox _debugText = new(GlobalFrame.GeEichFont, "") { Origin = Origin.TopLeft, IsShadowEnabled = true };


    // Constructors.
    internal GHGame(InGameFrame parentFrame, GameWorldSettings worldSettings)
    {
        ParentFrame = parentFrame ?? throw new ArgumentNullException(nameof(parentFrame));
        ParentFrame.AddLayer(BottomItemLayer);
        ParentFrame.AddLayer(TopItemLayer);
        ParentFrame.AddLayer(UILayer);

        AssetManager = new(ParentFrame);

        Background = worldSettings.Background ?? throw new ArgumentNullException(nameof(worldSettings.Background));
        Background.Load(AssetManager);
        Background.CreateBackground();
        BottomItemLayer.AddDrawableItem(Background);

        World = new(this, BottomItemLayer, TopItemLayer, worldSettings);
    }


    // Private methods.
    private void OnDebugToggleEvent(object? sender, EventArgs args)
    {
        if (_toggledDebugOption)
        {
            _toggledDebugOption = false;
            return;
        }

        _isDebugTextEnabled = !_isDebugTextEnabled;
        if (_isDebugTextEnabled)
        {
            UILayer.AddDrawableItem(_debugText);
        }
        else
        {
            UILayer.RemoveDrawableItem(_debugText);
        }
    }

    private void OnOverlaysToggleEvent(object? sender, EventArgs args)
    {
        _toggledDebugOption = true;
        _areEntityOverlaysEnabled = !_areEntityOverlaysEnabled;

        World.IsDebugInfoEnabled = !World.IsDebugInfoEnabled;
    }

    private void UpdateGame()
    {
        /* Non-tick dependent things. */
        Background.Update();

        /* Tick dependent things. */
        ProgramTime Time = new();
        Time.PassedTimeSeconds = GameFrameManager.PassedTimeSeconds;

        if (!_gameTime.Update(Time))
        {
            return;
        }

        World.Tick(_gameTime);
    }

    // Inherited methods.
    internal void OnStart()
    {
        if (IsRunning) return;

        _debugToggleListener = KeyboardListenerCreator.SingleKey(this, KeyCondition.OnRelease, OnDebugToggleEvent, Keys.F3);
        UserInput.AddListener(_debugToggleListener);
        _entityOverlayToggleListener = KeyboardListenerCreator.Shortcut(this, KeyCondition.OnPress, OnOverlaysToggleEvent, Keys.F3, Keys.G);
        UserInput.AddListener(_entityOverlayToggleListener);

        IsRunning = true;
        World.Start();
    }

    internal void Update()
    {
        if (!IsRunning)
        {
            return;
        }

        Stopwatch UpdateTimeMeasurer = Stopwatch.StartNew();
        UpdateGame();
        UpdateTimeMeasurer.Stop();

        _timeSinceInfoUpdate += GameFrameManager.PassedTimeSeconds;
        _totalUpdateTime += (float)UpdateTimeMeasurer.Elapsed.TotalMilliseconds;
        _infoUpdateCount++;
        if (_timeSinceInfoUpdate > INFO_UPDATE_TIME)
        {
            UpdateTime = _totalUpdateTime / _infoUpdateCount;
            if (!float.IsFinite(UpdateTime))
            {
                UpdateTime = 0f;
            }
            _infoUpdateCount = 0;
            _totalUpdateTime = 0f;
            _timeSinceInfoUpdate = 0f;
        }

        if (_isDebugTextEnabled)
        {
            _debugText.Text = $"FPS: {Display.FPS}" +
                $"\nTick Time: {UpdateTime}ms" +
                $"\nEntity Count: {World.EntityCount}";
        }
    }

    internal void OnEnd()
    {
        if (!IsRunning)
        {
            return;
        }

        _debugToggleListener.StopListening();
        _entityOverlayToggleListener.StopListening();

        World.End();
        IsRunning = false;
    }
}