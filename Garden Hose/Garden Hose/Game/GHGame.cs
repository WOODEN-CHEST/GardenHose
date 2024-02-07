using System;
using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using GardenHose.Frames.InGame;
using GardenHose.Game.GameAssetManager;

namespace GardenHose.Game;

internal class GHGame
{
    // Fields.
    public ILayer BottomItemLayer { get; private init; } = new Layer("items bottom");
    public ILayer TopItemLayer { get; private init; } = new Layer("items top");
    public ILayer UILayer { get; private init; } = new Layer("ui");
    public GameWorld World { get; private set; }
    public InGameFrame ParentFrame { get; private init; }
    public GHGameAssetManager AssetManager { get; private init; }



    // Internal fields
    internal bool IsPaused { get; set; } = false;

    internal bool IsRunning { get; private set; }

    internal float UpdateTime { get; private set; } = 0f;

    internal GameBackground Background { get; private init; }

    internal GHGameTime GameTime { get; } = new();


    // Private fields.
    private readonly GHGameDebugInfo _debugInfo;


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

        _debugInfo = new(this, World, GameTime);
        UILayer.AddDrawableItem(_debugInfo);
    }


    // Inherited methods.
    internal void OnStart()
    {
        if (IsRunning) return;

        IsRunning = true;
        World.Start();
        _debugInfo.OnGameStart();
    }

    internal void Update(IProgramTime time)
    {
        if (!IsRunning)
        {
            return;
        }

        if (IsPaused || !GameTime.Update(time))
        {
            return;
        }

        _debugInfo.StartTickMeasure();
        World.Tick(GameTime);
        _debugInfo.StopTickMeasure(time);
    }

    internal void OnEnd()
    {
        if (!IsRunning)
        {
            return;
        }


        World.End();
        _debugInfo.OnGameEnd();
        IsRunning = false;
    }
}