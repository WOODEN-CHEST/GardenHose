using Microsoft.Xna.Framework;
using System;

using GardenHose.Engine.IO;
using GardenHose.Engine.Logging;
using GardenHose.Engine.Frame;
using GardenHose.Engine;
using GardenHose.Frames;
using Microsoft.Xna.Framework.Input;
using System.Threading;
using System.Reflection;
using System.IO;

namespace GardenHose;

public class MainGame : Game
{
    // Static fields.
    public static MainGame Instance { get; private set; }
    public static GraphicsDeviceManager GraphicsManager { get; private set; }


    // Constructors.
    public MainGame()
    {
        GraphicsManager = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        Instance = this;

        IsMouseVisible = true;
        GraphicsManager.PreferredBackBufferWidth = 1280;
        GraphicsManager.PreferredBackBufferHeight = 720;
    }


    // Inherited methods.
    protected override void Initialize()
    {
        base.Initialize();

        Window.AllowUserResizing = true;
        IsFixedTimeStep = false;

        AssetManager.BasePath = Path.Combine( 
            Path.GetDirectoryName(Assembly.GetAssembly(this.GetType()).Location), 
            Content.RootDirectory);
        AssetManager.ExtraPath = null;
        AssetManager.CreateAssetEntries();

        DisplayInfo.SetTargetSize(1920f, 1080f);

        GameFrame.BackgroundColor = Color.Black;
        GameFrame.ActiveFrame = new FrameMain();
    }

    protected override void Update(GameTime gameTime)
    {
        UserInput.ListenForInput();
        DisplayInfo.Update(Window.ClientBounds.Width, Window.ClientBounds.Height);
        GameFrame.UpdateAll(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GameFrame.DrawAll(gameTime);
    }
}