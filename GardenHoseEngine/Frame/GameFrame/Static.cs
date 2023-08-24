using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;


namespace GardenHoseEngine.Frame;


public partial class GameFrame
{
    // Static fields.
    public static readonly GraphicsDevice GraphicsCard;
    public static GameTime Time { get; private set; }

    public static GameFrame ActiveFrame
    {
        get => s_activeFrame;
        set
        {
            if (value == null) throw new ArgumentNullException(nameof(value));
            SwitchActiveFrameAsync(value);
        }
    }

    public static GameFrame GlobalFrame
    {
        get => s_globalFrame;
        set
        {
            if (s_globalFrame != null) throw new
                    Exception("The global frame cannot be modified after already being set");

            s_globalFrame = value;
            s_globalFrame.Load();
            s_globalFrame.OnStart();
        }
    }

    public static Color BackgroundColor { get; set; } = Color.White;
    public static bool DrawBackground { get; set; } = true;
    public static readonly ConcurrentQueue<Action> Actions = new();
    public static GameLoopInfo LoopInfo { get => s_loopInfo; }

    public static readonly Texture2D SinglePixel; 


    // Private static fields.
    private static GameFrame s_activeFrame;
    private static GameFrame s_globalFrame;

    private static GameLoopInfo s_loopInfo = new();


    private static readonly SpriteBatch s_drawBatch;
    private static bool s_drawBatchBegun = false;
    private static Effect s_drawBatchShader = null;


    // Static constructor.
    static GameFrame()
    {
        GraphicsCard = MainGame.Instance.GraphicsDevice;
        s_drawBatch = new SpriteBatch(GraphicsCard);

        SinglePixel = new(GameFrame.GraphicsCard, 1, 1);
        SinglePixel.SetData(new Color[] { new(0xFFFFFFFF) });
    }


    // Static methods.
    public static void UpdateAll(GameTime time)
    {
        Time = time;
        Stopwatch UpdateTime = Stopwatch.StartNew();


        while (Actions.TryDequeue(out Action ExecutableAction)) ExecutableAction.Invoke();
        GlobalFrame?.Update();
        ActiveFrame.Update();


        UpdateTime.Stop();
        s_loopInfo.UpdateTime = UpdateTime.Elapsed;
    }

    public static void DrawAll(GameTime time)
    {
        Time = time;
        s_loopInfo.ResetDrawCount();
        Stopwatch DrawTime = Stopwatch.StartNew();


        if (DrawBackground) GraphicsCard.Clear(BackgroundColor);
        GlobalFrame?.Draw();
        ActiveFrame.Draw();


        s_drawBatch.End();
        s_drawBatchBegun = false;
        DrawTime.Stop();
        s_loopInfo.DrawTime = DrawTime.Elapsed;
    }

    public static void DrawTexture(Texture2D texture, 
        Vector2 position, 
        Rectangle? sourceRect, 
        Color color, 
        float rotation,
        Vector2 origin, 
        Vector2 scale)
    {
        s_loopInfo.TextureDraws++;

        s_drawBatch.Draw(texture,
        position,
        sourceRect,
        color,
        rotation,
        origin,
        scale,
        SpriteEffects.None,
        1f);
    }

    public static void DrawString(string text,
        SpriteFont font,
        Vector2 position,
        Color color,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        Effect shader = null)
    {
        s_loopInfo.StringDraws++;

        s_drawBatch.DrawString(font,
            text,
            position,
            color,
            rotation,
            origin,
            scale,
            SpriteEffects.None,
            1f);
    }


    // Private static methods.
    private static async void SwitchActiveFrameAsync(GameFrame newFrame)
    {
        if (s_activeFrame == newFrame) return;

        // Load new frame.
        await Task.Run(() => newFrame?.Load());

        // Switch frames.
        Actions.Enqueue(() =>
        {
            GameFrame OldFrame = s_activeFrame;
            OldFrame.OnEnd();
            s_activeFrame = newFrame;
            newFrame.OnStart();

            // Unload old frame.
            Task.Run(() =>
            {
                OldFrame?.Unload();
                AssetManager.F
                GC.Collect();
            });
        });
    }
}