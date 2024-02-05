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
    private GenericProgramTime _time = new();


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
        UserInput.AddListener(KeyboardListenerCreator.SingleKey(KeyCondition.OnPress,
            Display.OnUserToggleFullscreenEvent, Keys.F11));

        /* User input. */
        Window.FileDrop += UserInput.OnFileDropEvent;
        Window.TextInput += UserInput.OnTextInputEvent;

        /*  Audio engine. */
        AudioEngine.DefaultEngine = new();

        /* Asset manager. */
        AssetManager.Initialize(GHEngine.StartupSettings.AssetBasePath);


        // Set fields.
        IsMouseVisible = GHEngine.StartupSettings.IsMouseVisible;
        Window.AllowAltF4 = GHEngine.StartupSettings.AllowAltF4;
        Window.AllowUserResizing = GHEngine.StartupSettings.AllowUserResizing;

        // Start frames.
        GameFrameManager.Initialize(GHEngine.StartupSettings.StartupFrame);
    }

    protected override void Update(GameTime gameTime)
    {
        _time.TotalTimeSeconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
        _time.PassedTimeSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;

        UserInput.ListenForInput(IsActive);
        GameFrameManager.UpdateFrames(_time);
        Display.Update(_time);
    }

    protected override void Draw(GameTime gameTime)
    {
        GameFrameManager.DrawFrames(_time);
    }
}