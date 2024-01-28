using GardenHoseEngine;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHose.Frames.MainMenu;

internal partial class MainMenuFrame : GameFrame
{
    // Private fields.
    private readonly MainFrameUIManager _userInterfaceManager; 
    private readonly MainFrameBackgroundManager _backgroundManager;


    // Constructors.
    public MainMenuFrame(string? name) : base(name)
    {
        ILayer BackgroundLayer = new Layer("background");
        ILayer UserInterfaceLayer = new Layer("user interface");

        AddLayer(BackgroundLayer);
        AddLayer(UserInterfaceLayer);

        _backgroundManager = new(this, BackgroundLayer);
        _userInterfaceManager = new(this, UserInterfaceLayer);
    }


    // Inherited methods.
    public override void Load()
    {
        _userInterfaceManager.Load();
        _backgroundManager.Load();
        base.Load();
    }

    public override void OnStart()
    {
        base.OnStart();
        
        _userInterfaceManager.OnStart();
        _backgroundManager.OnStart();

        GHEngine.Game.IsMouseVisible = true;
    }

    public override void Update(IProgramTime time)
    {
        base.Update(time);;
        _backgroundManager.Update(time);
        _userInterfaceManager.Update(time);
    }
}