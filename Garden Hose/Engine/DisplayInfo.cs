using GardenHose.Engine.Frame;
using GardenHose.Engine.IO;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using System;
using System.Runtime.CompilerServices;

namespace GardenHose.Engine;

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
    public static float ItemScale { get; private set; }
    public static float InverseItemScale { get; private set; }


    // Private static fields.
    private static int s_windowedWidth = MinWidth;
    private static int s_windowedHeight = MinHeight;


    // Static constructors.
    static DisplayInfo()
    {
        UserInput.AddKeyListener(null,KeyCondition.OnPress, Keys.F11, OnDisplayButtonPress);
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


    /* Adjusting item values. */
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RealToVirtualPosition(ref Vector2 position)
    {
        position.X += XOffset;
        position.Y += YOffset;

        position *= ItemScale;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void VirtualToRealPosition(ref Vector2 position)
    {
        position *= InverseItemScale;

        position.X -= XOffset;
        position.Y -= YOffset;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void RealToVirtualScale(ref Vector2 scale) => scale *= ItemScale;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void VirtualToRealScale(ref Vector2 scale) => scale *= InverseItemScale;


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

        // Final value calculation.
        InverseItemScale = 1 / ItemScale;
    }

    private static void SetUltraWideRatio()
    {
        RealAdjustedHeight = Height;
        RealAdjustedWidth = TargetRatio / Ratio * Width;

        XOffset = (Width - RealAdjustedWidth) / 2f;
        YOffset = 0f;

        ItemScale = Height / TargetHeight;
    }

    private static void SetNarrowRatio()
    {
        RealAdjustedHeight = RatioRatio * Height;
        RealAdjustedWidth = Width;

        XOffset = 0f;
        YOffset = (Height - RatioRatio * Height) / 2f;

        ItemScale = Width / TargetWidth;
    }

    private static void OnDisplayButtonPress(object sender, EventArgs args)
    {
        if (UserInput.KeyboardCur.IsKeyDown(Keys.LeftControl)) SnapSizeToRatio();
        else ToggleFullScreen();
    }
}