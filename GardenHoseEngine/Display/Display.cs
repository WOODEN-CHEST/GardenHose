using GardenHoseEngine.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Runtime.CompilerServices;

namespace GardenHoseEngine;

public class Display
{
    // Fields.
    public const int MinWidth = 144;
    public const int MinHeight = 80;

    public Vector2 VirtualSize { get; private set; }
    public float VirtualRatio { get; private set; }

    public Vector2 WindowSize { get; private set; }
    public float WindowRatio { get; private set; }

    public Vector2 GameAreaSize { get; private set; }

    public Vector2 ItemOffset { get; private set; }
    public float ItemScale { get; private set; }
    public float InverseItemScale { get; private set; }

    public event EventHandler? DisplayChanged;


    // Private fields.
    private GHEngine _ghEngine;
    private Vector2 _windowedSize;


    // Constructors.
    internal Display()
    {
        UserInput.AddKeyListener(null, null, KeyCondition.OnPress, Keys.F11, OnDisplayButtonPress);
    }


    // Methods.
    public void SetMode(DisplayInfo displayInfo)
    {
        DisplayMode DisplaySettings = _ghEngine.GraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode;

        _ghEngine.GraphicsDeviceManager.ApplyChanges();
    }


    // Internal methods.
    internal void Update(int curWidth, int curHeight)
    {
        if ((int)Width != curHeight || (int)Height != curHeight)
        {
            UpdateDisplayData(curWidth, curHeight);
        }
    }

    public void SetTargetSize(float targetWidth, float targetHeight)
    {
        VirtualWidth = targetWidth;
        GameHeight = targetHeight;
        VirtualRatio = targetWidth / targetHeight;

        UpdateDisplayData(targetWidth, targetHeight);
    }

    public void ToggleFullScreen()
    {
        DisplayMode Display = MainGame.GraphicsManager.GraphicsDevice.Adapter.CurrentDisplayMode;

        if (MainGame.GraphicsManager.IsFullScreen)
        {
            MainGame.GraphicsManager.PreferredBackBufferWidth = s_windowedWidth;
            MainGame.GraphicsManager.PreferredBackBufferHeight = s_windowedHeight;
            MainGame.GraphicsManager.IsFullScreen = false;
            MainGame.GraphicsManager.ApplyChanges();
        }
        else
        {
            MainGame.GraphicsManager.PreferredBackBufferWidth = Display.Width;
            MainGame.GraphicsManager.PreferredBackBufferHeight = Display.Height;
            MainGame.GraphicsManager.IsFullScreen = true;
            MainGame.GraphicsManager.ApplyChanges();
        }
    }

    public void SnapSizeToRatio()
    {
        if (MainGame.GraphicsManager.IsFullScreen) return;

        Height = RealAdjustedHeight;
        Width = RealAdjustedWidth;

        MainGame.GraphicsManager.PreferredBackBufferWidth = Math.Max((int)Math.Ceiling(Width), MinWidth);
        MainGame.GraphicsManager.PreferredBackBufferHeight = Math.Max((int)Math.Ceiling(Height), MinHeight);
        MainGame.GraphicsManager.ApplyChanges();

        UpdateDisplayData(Width, Height);
    }


    /* Adjusting item values. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ToVirtualPosition(Vector2 position)
    {
        position.X -= XOffset;
        position.Y -= YOffset;

        position *= InverseItemScale;

        return position;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Vector2 ToRealPosition(Vector2 position)
    {
        position *= ItemScale;

        position.X += XOffset;
        position.Y += YOffset;

        return position;
    }


    // Private static methods.
    private void UpdateDisplayData(Vector2 newSize)
    {
        if (newSize == VirtualSize) return;


        DisplayMode ScreenMode = _ghEngine.GraphicsDeviceManager.GraphicsDevice.Adapter.CurrentDisplayMode;
        WindowSize = new(ScreenMode.Width, ScreenMode.Height);
        WindowRatio = ScreenMode.AspectRatio;

        VirtualSize = newSize;
        VirtualRatio = VirtualSize.X / VirtualSize.Y;

        ItemScale = Math.Min(WindowSize.X / VirtualSize.X, WindowSize.Y / VirtualSize.Y);
        InverseItemScale = 1 / ItemScale;

        float ScreenToWindowRatio = WindowRatio / VirtualRatio;
        float WindowToScreenRatio = VirtualRatio / WindowRatio;
        GameAreaSize = VirtualSize * ItemScale;
        ItemOffset = new(
            ((WindowSize.X * InverseItemScale) - VirtualSize.X) / 2f,
            ((WindowSize.Y * InverseItemScale) - VirtualSize.Y) / 2f
        );


        DisplayChanged?.Invoke(this, EventArgs.Empty);
    }

    private void OnDisplayButtonPress(object? sender, EventArgs args)
    {
        if (UserInput.KeyboardCur.IsKeyDown(Keys.LeftControl))
        {
            SnapSizeToRatio();
        }
        else ToggleFullScreen();
    }
}