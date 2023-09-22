using GardenHoseEngine;
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
    private SpriteAnimation _logoAnim;
    private SpriteItem _logo;


    // Constructors.
    public MainFrameBackgroundManager(MainMenuFrame parentFrame) : base(parentFrame) { }


    // Inherited methods.
    internal override void Load(AssetManager assetManager)
    {
        _logoAnim = new(0d, ParentFrame, assetManager, Origin.TopLeft, "ui/logo_tiny");
        
    }

    internal override void OnStart()
    {
        _logo = new(ParentFrame, GH.Engine.Display, ParentFrame.LayerManager.UILayer, _logoAnim);
        _logo.Position.Vector = new(50f, 50f);
    }
}