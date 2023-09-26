using GardenHoseEngine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Animation;
using GardenHoseEngine.Frame.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Frames.MainMenu;

internal class MainFrameBackgroundManager : FrameComponentManager<MainMenuFrame>
{
    // Private fields.
    private readonly ILayer _bgLayer;

    private SpriteAnimation _logoAnim;
    private SpriteItem _logo;


    // Constructors.
    public MainFrameBackgroundManager(MainMenuFrame parentFrame, ILayer bgLayer) : base(parentFrame)
    {
        _bgLayer = bgLayer ?? throw new ArgumentNullException(nameof(bgLayer));
    }


    // Inherited methods.
    internal override void Load(AssetManager assetManager)
    {
        _logoAnim = new(0f, ParentFrame, assetManager, Origin.TopLeft, "ui/logo_tiny");
        
    }

    internal override void OnStart()
    {
        _logo = new(GH.Engine.Display, _logoAnim);
        _logo.Position.Vector = new(50f, 50f);
        ParentFrame.LayerManager.BackgroundLayer.AddDrawableItem(_logo);
    }
}