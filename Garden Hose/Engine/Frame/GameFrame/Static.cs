using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Content;

namespace GardenHose.Engine.Frame;


public partial class GameFrame
{
    // Static fields.
    public static readonly SpriteBatch DrawBatch;
    public static readonly GraphicsDevice Graphics;
    public static readonly ContentManager Content;
    public static GameTime Time { get; private set; }

    public static GameFrame ActiveFrame
    {
        get => _activeFrame;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            SwitchFrame(ActiveFrame, value);
            _activeFrame = value;
        }
    }

    public static GameFrame GlobalFrame
    {
        get => _globalFrame;
        set
        {
            SwitchFrame(ActiveFrame, value);
            _globalFrame = value;
        }
    }

    public static Color BackgroundColor = Color.White;
    public static bool DrawBackground = true;

    public static uint DrawCalls = 0;


    // Private static fields.
    private static GameFrame _activeFrame;
    private static GameFrame _globalFrame;


    // Static constructor.
    static GameFrame()
    {
        Graphics = MainGame.Instance.GraphicsDevice;
        DrawBatch = new SpriteBatch(Graphics);
        Content = MainGame.Instance.Content;
    }


    // Static methods.
    public static void UpdateAll(GameTime time)
    {
        Time = time;

        GlobalFrame?.Update();
        ActiveFrame.Update();
    }

    public static void DrawAll(GameTime time)
    {
        Time = time;
        DrawCalls = 0;
        if (DrawBackground) Graphics.Clear(BackgroundColor);

        GlobalFrame?.Draw();
        ActiveFrame.Draw();
    }


    // Private static methods.
    private static void SwitchFrame(GameFrame oldFrame, GameFrame newFrame)
    {
        if (oldFrame == newFrame) return;

        if (oldFrame != null)
        {
            oldFrame.OnEnd();
            oldFrame.Unload();
        }
        newFrame?.Load();

        AssetManager.FreeMemory();
        GC.Collect();

        newFrame?.OnStart();
    }
}
