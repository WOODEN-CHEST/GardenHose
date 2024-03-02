using System;
using GardenHose.Game.World;
using GardenHoseEngine.Frame;
using GardenHose.Frames.InGame;
using GardenHose.Game.GameAssetManager;

namespace GardenHose.Game;

internal class GHGame
{
    // Fields.
    public ILayer ItemLayer { get; private init; } = new Layer("itemss");
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
        ParentFrame.AddLayer(ItemLayer);
        ParentFrame.AddLayer(UILayer);

        AssetManager = new(ParentFrame);

        Background = worldSettings.Background ?? throw new ArgumentNullException(nameof(worldSettings.Background));
        Background.Load(AssetManager);
        Background.CreateBackground();
        ItemLayer.AddDrawableItem(Background, -1.0f);

        World = new(this, ItemLayer, worldSettings);

        _debugInfo = new(this, World, GameTime);
        UILayer.AddDrawableItem(_debugInfo);
    }


    // Inherited methods.
    internal void OnStart()
    {
        if (IsRunning) return;

        IsRunning = true;
        World.Start();
        GameTime.Speed = 1f;
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
        IsRunning = false;
    }
}