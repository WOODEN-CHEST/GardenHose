using GardenHoseEngine;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHose.Frames.MainMenu;

internal partial class MainMenuFrame : GameFrame
{
    // Internal fields.
    internal MainFrameLayerManager LayerManager { get; private init; }

    internal MainFrameButtonManager ButtonManager { get; private init; }

    internal MainFrameBackgroundManager BackgroundManager { get; private init; }


    // Constructors.
    public MainMenuFrame(string? name) : base(name)
    {
        LayerManager = new(this);
        BackgroundManager = new(this, LayerManager.BackgroundLayer);
        ButtonManager = new(this, LayerManager.UILayer);
    }


    // Inherited methods.
    public override void Load()
    {
        base.Load();

        LayerManager.Load();
        ButtonManager.Load();
        BackgroundManager.Load();
    }

    public override void OnStart()
    {
        base.OnStart();
        
        LayerManager.OnStart();
        ButtonManager.OnStart();
        BackgroundManager.OnStart();

        GHEngine.Game.IsMouseVisible = true;
    }

    public override void Update()
    {
        base.Update();
        LayerManager.Update();
        BackgroundManager.Update();
        ButtonManager.Update();
    }
}