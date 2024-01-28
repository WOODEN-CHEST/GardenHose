using GardenHoseEngine;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHose.Frames.MainMenu;

internal partial class MainMenuFrame : GameFrame
{
    // Private fields.
    private readonly MainFrameLayerManager _layerManager;
    private readonly MainFrameButtonManager _buttonManager; 
    private readonly MainFrameBackgroundManager _backgroundManager;


    // Constructors.
    public MainMenuFrame(string? name) : base(name)
    {
        _layerManager = new(this);
        _backgroundManager = new(this, _layerManager.BackgroundLayer);
        _buttonManager = new(this, _layerManager.UILayer);
    }


    // Inherited methods.
    public override void Load()
    {
        _layerManager.Load();
        _buttonManager.Load();
        _backgroundManager.Load();
        base.Load();
    }

    public override void OnStart()
    {
        base.OnStart();
        
        _layerManager.OnStart();
        _buttonManager.OnStart();
        _backgroundManager.OnStart();

        GHEngine.Game.IsMouseVisible = true;
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);
        _layerManager.Update(time);
        _backgroundManager.Update(time);
        _buttonManager.Update(time);
    }
}