using GardenHose.Engine.IO;
using GardenHose.Engine.Logging;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace GardenHose.Engine.Frame;

public static class DisplayInfo
{
    // Constants.
    public const int MinWidth = 144;
    public const int MinHeight = 80;


    // Static fields.
    public static float TargetWidth { get; private set; }
    public static float TargetHeight { get; private set; }
    public static float TargetRatio { get; private set; }

    public static float Width { get; private set; }
    public static float Height { get; private set; }
    public static float Ratio { get; private set; }

    public static float RatioRatio { get; private set; }

    public static float RealAdjustedWidth { get; private set; }
    public static float RealAdjustedHeight { get; private set; }

    public static float XOffset { get; private set; }
    public static float YOffset { get; private set; }
    public static float XScale { get; private set; }
    public static float YScale { get; private set; }
    public static float ObjectScale { get; private set; }


    // Private static fields.
    private static int s_windowedWidth = MinWidth;
    private static int s_windowedHeight = MinHeight;


    // Static constructors.
    static DisplayInfo()
    {
        KeyboardEventListener.AddListener(OnDisplayButtonPress, KeyEventTrigger.OnPress, Keys.F11);
    }


    // Static methods.
    public static void Update(int curWidth, int curHeight)
    {
        if ((int)Width != curHeight || (int)Height != curHeight)
        {
            SetDisplayData(curWidth, curHeight);
        }
    }

    public static void SetTargetSize(float targetWidth, float targetHeight)
    {
        TargetWidth = targetWidth;
        TargetHeight = targetHeight;
        TargetRatio = targetWidth / targetHeight;

        SetDisplayData(targetWidth, targetHeight);
    }

    public static void ToggleFullScreen()
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

    public static void SnapSizeToRatio()
    {
        if (MainGame.GraphicsManager.IsFullScreen) return;

        Height = RealAdjustedHeight;
        Width = RealAdjustedWidth;

        MainGame.GraphicsManager.PreferredBackBufferWidth = Math.Max((int)Math.Ceiling(Width), MinWidth);
        MainGame.GraphicsManager.PreferredBackBufferHeight = Math.Max((int)Math.Ceiling(Height), MinHeight);
        MainGame.GraphicsManager.ApplyChanges();

        SetDisplayData(Width, Height);
    }


    // Private static methods.
    private static void SetDisplayData(float width, float height)
    {
        // Calculate values.
        Width = width;
        Height = height;
        Ratio = width / height;

        RatioRatio = Ratio / TargetRatio;

        if (Ratio > TargetRatio) SetUltraWideRatio();
        else SetNarrowRatio();

        XScale = RealAdjustedWidth / TargetWidth;
        YScale = RealAdjustedHeight / TargetHeight;

        if (!MainGame.GraphicsManager.IsFullScreen)
        {
            s_windowedWidth = Math.Max(100, (int)Width);
            s_windowedHeight = Math.Max(100, (int)Height);
        }

        // Update drawables to match new size.
        if (GameFrame.ActiveFrame != null)
        {
            foreach (Layer L in GameFrame.ActiveFrame.Layers) L.OnDisplayChange();
        }

        if (GameFrame.GlobalFrame != null)
        {
            foreach (Layer L in GameFrame.GlobalFrame?.Layers) L.OnDisplayChange();
        }
    }

    private static void SetUltraWideRatio()
    {
        RealAdjustedHeight = Height;
        RealAdjustedWidth = (TargetRatio / Ratio) * Width;

        XOffset = (Width - RealAdjustedWidth) / 2f;
        YOffset = 0f;

        ObjectScale = Height / TargetHeight;
    }

    private static void SetNarrowRatio()
    {
        RealAdjustedHeight = RatioRatio * Height;
        RealAdjustedWidth = Width;

        XOffset = 0f;
        YOffset = (Height - (RatioRatio * Height)) / 2f;

        ObjectScale = Width / TargetWidth;
    }

    private static void OnDisplayButtonPress()
    {
        if (KeyboardEventListener.StateCur.IsKeyDown(Keys.LeftControl)) SnapSizeToRatio();
        else ToggleFullScreen();
    }
}