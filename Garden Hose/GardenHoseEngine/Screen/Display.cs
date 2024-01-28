using GardenHoseEngine.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame;
using GardenHoseEngine.Frame.Item.Basic;

namespace GardenHoseEngine.Screen;

public static class Display
{
    // Fields.
    public static GraphicsDeviceManager GraphicsManager { get; set; }

    public static Texture2D SinglePixel { get; internal set; }

    public static Line SharedLine
    {
        get => s_sharedLine;
        set => s_sharedLine = value;
    }


    public static readonly Vector2 MinimumWindowSize = new(144f, 80f);

    public static bool IsFullScreen
    {
        get => GraphicsManager.IsFullScreen;
        set
        {
            if (value != IsFullScreen)
            {
                SetDisplay(value ? FullScreenSize : WindowedSize, value);
            }
        }
    }

    public static Vector2 FullScreenSize
    {
        get => s_fullScreenSize;
        set
        {
            if (IsFullScreen)
            {
                SetDisplay(value, true);
            }
            else
            {
                s_fullScreenSize = ValidateScreenSize(value);
            }
        }
    }

    public static Vector2 WindowedSize
    {
        get => s_windowedSize;
        set
        {
            if (!IsFullScreen)
            {
                SetDisplay(value, false);
            }
            else
            {
                s_windowedSize = ValidateScreenSize(value);
            }
        }
    }

    public static Vector2 WindowSize
    {
        get => new(GHEngine.Game.Window.ClientBounds.Width, GHEngine.Game.Window.ClientBounds.Height);
        set => SetDisplay(value, IsFullScreen);
    }

    public static int? FrameRateLimit { get; set; } = 144;

    public static Vector2 VirtualSize { get; internal set; }

    public static Vector2 ViewportSize { get; private set; }

    public static Vector2 ScreenSize
    {
        get
        {
            return new Vector2(GraphicsManager.GraphicsDevice.Adapter.CurrentDisplayMode.Width,
                GraphicsManager.GraphicsDevice.Adapter.CurrentDisplayMode.Height);
        }
    }
    public static Vector2 ItemOffset { get; private set; }

    public static float ItemScale { get; private set; }

    public static float InverseItemScale { get; private set; }

    public static float FPS { get; private set; }

    public static event EventHandler? DisplayChanged;


    // Private static fields.
    private static Vector2 s_windowedSize;
    private static Vector2 s_fullScreenSize;

    [ThreadStatic]
    private static Line s_sharedLine;;


    // Static methods.
    public static void CorrectWindowedSize()
    {
        if (!IsFullScreen)
        {
            SetDisplay(ViewportSize, false);
        }
    }


    // Internal static methods.
    internal static void OnWindowSizeChangeByUserEvent(object? sender, EventArgs args)
    {
        if ((GHEngine.Game.Window.ClientBounds.Width < MinimumWindowSize.X)
            || (GHEngine.Game.Window.ClientBounds.Height < MinimumWindowSize.Y))
        {
            SetDisplay(MinimumWindowSize, IsFullScreen);
        }
        else
        {
            UpdateDisplayInfo();
        }
    }

    internal static void OnUserToggleFullscreenEvent(object? sender, EventArgs args)
    {
        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.LeftControl))
        {
            CorrectWindowedSize();
        }
        else IsFullScreen = !IsFullScreen;
    }

    internal static void Update(IProgramTime time)
    {
        FPS = 1f / time.PassedTimeSeconds;
    }


    // Private static methods.
    private static void SetDisplay(Vector2 newSize, bool isFullScreen)
    {
        newSize = ValidateScreenSize(newSize);

        GraphicsManager.IsFullScreen = isFullScreen;
        GraphicsManager.PreferredBackBufferWidth = (int)newSize.X;
        GraphicsManager.PreferredBackBufferHeight = (int)newSize.Y;
        GraphicsManager.ApplyChanges();

        UpdateDisplayInfo();
    }

    private static void UpdateDisplayInfo()
    {
        // Update size info.
        if (IsFullScreen)
        {
            s_fullScreenSize = WindowSize;
        }
        else
        {
            s_windowedSize = WindowSize;
        }

        // Calculate properties.
        ItemScale = Math.Min(WindowSize.X / VirtualSize.X, WindowSize.Y / VirtualSize.Y);
        InverseItemScale = 1 / ItemScale;
        ViewportSize = VirtualSize * ItemScale;
        ItemOffset = new((WindowSize.X - ViewportSize.X) / 2f, (WindowSize.Y - ViewportSize.Y) / 2f);

        DisplayChanged?.Invoke(null, EventArgs.Empty);
    }

    private static Vector2 ValidateScreenSize(Vector2 size)
    {
        if (!(float.IsFinite(size.X) && (size.X > 0)
            && float.IsFinite(size.Y) && (size.Y > 0)))
        {
            throw new ArgumentException($"Invalid virtual display size: {size}", nameof(size));
        }

        size.X = Math.Max(size.X, MinimumWindowSize.X);
        size.Y = Math.Max(size.Y, MinimumWindowSize.Y);
        return size;
    }

    public static Vector2 ToRealPosition(Vector2 virtualVector) => (virtualVector * ItemScale) + ItemOffset;

    public static Vector2 ToVirtualPosition(Vector2 realVector) => (realVector - ItemOffset) * InverseItemScale;

    public static Vector2 ToRealScale(Vector2 virtualScale) => virtualScale * ItemScale;

    public static Vector2 ToVirtualScale(Vector2 virtualScale) => virtualScale * InverseItemScale;
}