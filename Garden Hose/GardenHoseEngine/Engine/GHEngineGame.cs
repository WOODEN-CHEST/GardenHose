using GardenHoseEngine.Audio;
using GardenHoseEngine.Frame;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


namespace GardenHoseEngine.Engine;


public partial class GHEngineGame : Game
{
    // Private fields.


    // Constructors.
    public GHEngineGame()
    {
        Display.GraphicsManager = new(this);
    }


    // Inherited methods.
    protected override void Initialize()
    {
        base.Initialize();

        // Create engine components.
        /* Display. */
        Display.VirtualSize = GHEngine.StartupSettings!.VirtualSize;
        Display.WindowSize = GHEngine.StartupSettings.WindowSize ?? Display.ScreenSize / 1.5f;
        Display.FullScreenSize = Display.ScreenSize;
        Display.SinglePixel = new(Display.GraphicsManager.GraphicsDevice, 1, 1);
        Display.SinglePixel.SetData(new Color[] { Color.White });
        Display.SharedLine = new();
        Window.ClientSizeChanged += Display.OnWindowSizeChangeByUserEvent;
        UserInput.AddListener(KeyboardListenerCreator.SingleKey(null, KeyCondition.OnPress,
            Display.OnUserToggleFullscreenEvent, Keys.F11));

        /* User input. */
        Window.FileDrop += UserInput.OnFileDropEvent;
        Window.TextInput += UserInput.OnTextInputEvent;

        /*  Audio engine. */
        AudioEngine.Engine = new();

        /* Asset manager. */
        AssetManager.Initialize(GHEngine.StartupSettings.AssetBasePath, GHEngine.StartupSettings.AssetExtraPath);


        // Set fields.
        IsMouseVisible = GHEngine.StartupSettings.IsMouseVisible;
        Window.AllowAltF4 = GHEngine.StartupSettings.AllowAltF4;
        Window.AllowUserResizing = GHEngine.StartupSettings.AllowUserResizing;

        // Start frames.
        GameFrameManager.Initialize(GHEngine.StartupSettings.StartupFrame, GHEngine.StartupSettings.GlobalFrame);

        // Delete startup settings.
        GHEngine.StartupSettings = null;
    }

    protected override void Update(GameTime gameTime)
    {
        UserInput.ListenForInput(IsActive);
        GameFrameManager.UpdateFrames((float)gameTime.ElapsedGameTime.TotalSeconds);
        Display.Update();
    }

    protected override void Draw(GameTime gameTime)
    {
        GameFrameManager.DrawFrames();
    }
}