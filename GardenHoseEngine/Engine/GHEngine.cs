﻿using GardenHoseEngine;
using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using GardenHoseEngine.Logging;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;


namespace GardenHose;


public class GHEngine : Game
{
    // Public fields.
    public GHEngineStartupSettings StartupSettings { get; private init; }

    public readonly string GameName;

    public readonly string InternalName;

    public string DataRootPath { get; private set; }

    public Logger Logger { get; private set; }

    public GraphicsDeviceManager GraphicsManager { get; private set; }

    public Display Display { get; private set; }

    public UserInput UserInput { get; private set; }

    public AudioEngine AudioEngine { get; private set; }

    public AssetManager AssetManager { get; private set; }

    public GameFrameManager FrameManager { get; private set; }

    public Texture2D SinglePixel { get; private set; }


    // Constructors.
    public GHEngine(GHEngineStartupSettings startupSettings) 
    {
        StartupSettings = startupSettings ?? throw new ArgumentNullException(nameof(startupSettings));

        GraphicsManager = new(this);
        GameName = StartupSettings.GameName;
        InternalName = StartupSettings.InternalName;
    }


    // Methods.
    public void Execute()
    {
        DataRootPath = Path.Combine(StartupSettings.GameDataRootDirectory, InternalName);
        Logger = new(Path.Combine(DataRootPath, "logs"));

        try
        {
            Run();
            AudioEngine.Dispose();
            Logger.Dispose();
        }
        catch (Exception e)
        {
            OnCrash(e);
            AudioEngine.Dispose();
        }
    }


    // Private methods.
    private void OnCrash(Exception e)
    {
        Logger.Critical($"Game has crashed! " +
        $"Main thread ID: {Environment.CurrentManagedThreadId}. Info: {e}");
        Logger.Dispose();

        if (OperatingSystem.IsWindows())
        {
            Process.Start("notepad", Logger.LogPath);
        }
    }

    private void OnUserToggleFullscreenEvent(object? sender, EventArgs args)
    {
        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.LeftControl))
        {
            Display.CorrectWindowedSize();
        }
        else Display.IsFullScreen = !Display.IsFullScreen;
    }


    // Inherited methods.
    protected override void Initialize()
    {
        base.Initialize();

        // Create engine components.
        Display = new(GraphicsManager, Window, StartupSettings.VirtualSize, 
            StartupSettings.WindowSize, StartupSettings.IsFullScreen);
        SinglePixel = new(GraphicsManager.GraphicsDevice, 1, 1);
        SinglePixel.SetData(new Color[] { Color.White });


        UserInput = new(Display, Window);
        UserInput.AddListener(KeyboardListenerCreator.SingleKey(UserInput, this, null, KeyCondition.OnPress,
            OnUserToggleFullscreenEvent, Keys.F11));

        AudioEngine = new();
        AssetManager = new(StartupSettings.AssetBasePath, StartupSettings.AssetExtraPath,
            AudioEngine, Content, GraphicsManager, Logger);


        // Set fields.
        IsMouseVisible = StartupSettings.IsMouseVisible;
        Window.AllowAltF4 = StartupSettings.AllowAltF4;
        Window.AllowUserResizing = StartupSettings.AllowUserResizing;


        // Start frames.
        FrameManager = new(GraphicsManager, Display, AssetManager,
            StartupSettings.StartupFrame, StartupSettings.GlobalFrame);
    }

    protected override void Update(GameTime gameTime)
    {
        UserInput.ListenForInput(IsActive);
        FrameManager.UpdateFrames((float)gameTime.ElapsedGameTime.TotalSeconds);

    }

    protected override void Draw(GameTime gameTime)
    {
        FrameManager.DrawFrames((float)gameTime.ElapsedGameTime.TotalSeconds);
    }
}