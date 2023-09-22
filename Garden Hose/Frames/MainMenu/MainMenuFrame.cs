using GardenHose.Frames.Global;
using GardenHose.UI;
using GardenHose.UI.Buttons.Connector;
using GardenHoseEngine;
using GardenHoseEngine.Animatable;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Frame.Item.Text;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        ButtonManager = new(this);
        BackgroundManager = new(this);
    }


    // Inherited methods.
    public override void Load(AssetManager assetManager)
    {
        base.Load(assetManager);

        LayerManager.Load(assetManager);
        ButtonManager.Load(assetManager);
        BackgroundManager.Load(assetManager);
            

        FinalizeLoad();
    }

    public override void OnStart()
    {
        base.OnStart();
        
        LayerManager.OnStart();
        ButtonManager.OnStart();
        BackgroundManager.OnStart();

        GH.Engine.UserInput.AddListener(MouseListenerCreator.SingleButton(GH.Engine.UserInput, this, this, true,
            MouseCondition.OnClick, (sender, args) => ButtonManager.BringMainButtonsIn(), MouseButton.Right));
        GH.Engine.UserInput.AddListener(MouseListenerCreator.SingleButton(GH.Engine.UserInput, this, this, true,
            MouseCondition.OnClick, (sender, args) => ButtonManager.BringMainButtonsOut(), MouseButton.Middle));

        GH.Engine.IsMouseVisible = true;
    }

    public override void Update(TimeSpan passedTime)
    {
        LayerManager.Update(passedTime);

        base.Update(passedTime);
    }
}