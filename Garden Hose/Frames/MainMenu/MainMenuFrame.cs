using GardenHoseEngine;
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
    public override void Load(AssetManager assetManager)
    {
        base.Load(assetManager);

        LayerManager.Load(assetManager);
        ButtonManager.Load(assetManager);
        BackgroundManager.Load(assetManager);
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

    public override void Update(float passedTimeSeconds)
    {
        base.Update(passedTimeSeconds);
        LayerManager.Update(passedTimeSeconds);
    }

    public override void Draw(float passedTimeSeconds,
        GraphicsDevice graphicsDevice, 
        SpriteBatch spriteBatch,
        RenderTarget2D layerPixelBuffer,
        RenderTarget2D framePixelBuffer)
    {
        base.Draw(passedTimeSeconds, graphicsDevice, spriteBatch, layerPixelBuffer, framePixelBuffer);  
    }
}