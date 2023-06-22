using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;


namespace GardenHose.Engine.Frame;


public partial class GameFrame
{
    // Static fields.
    public static readonly GraphicsDevice Graphics;
    public static readonly ContentManager Content;
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



    // Private static fields.
    private static GameFrame s_activeFrame;
    private static GameFrame s_globalFrame;

    private static readonly Texture2D s_SinglePixel;
    private static GameLoopInfo s_loopInfo = new();


    private static readonly SpriteBatch s_drawBatch;
    private static bool s_drawBatchBegun = false;
    private static Effect s_drawBatchShader = null;


    // Static constructor.
    static GameFrame()
    {
        Graphics = MainGame.Instance.GraphicsDevice;
        s_drawBatch = new SpriteBatch(Graphics);
        Content = MainGame.Instance.Content;

        s_SinglePixel = new(Graphics, 1, 1);
        s_SinglePixel.SetData(new Color[] { new(0xFFFFFFFF) });
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


        if (DrawBackground) Graphics.Clear(BackgroundColor);
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
        Vector2 scale,
        Effect shader)
    {
        s_loopInfo.TextureDraws++;
        PrepareDrawBatch(shader);

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

    public static void DrawString()
    {
        s_loopInfo.StringDraws++;
        PrepareDrawBatch(shader);
    }

    public static void DrawLine(Vector2 origin, Vector2 destination, float thickness, Color color)
    {
        DrawLine(
            origin,
            MathF.Atan((destination.Y - origin.Y) / (destination.X - origin.X)),
            Vector2.Distance(origin, destination),
            thickness,
            color);
    }

    public static void DrawLine(Vector2 origin, float rotation, float length, float thickness, Color color)
    {
        s_loopInfo.BasicDraws++;

        length *= DisplayInfo.ItemScale;
        DisplayInfo.VirtualToRealPosition(ref origin);

        s_drawBatch.Draw(
            s_SinglePixel,
            origin,
            null,
            color,
            rotation,
            new Vector2(0f, thickness / 2f),
            new Vector2(length, thickness),
            SpriteEffects.None,
            0.5f);
    }

    public static void DrawRectangle(Rectangle rectangle, Color color)
    {
        s_loopInfo.BasicDraws++;

        s_drawBatch.Draw(
            s_SinglePixel,
            rectangle.Location.ToVector2(),
            null,
            color,
            0f,
            Vector2.Zero,
            rectangle.Size.ToVector2(),
            SpriteEffects.None,
            0.5f);
    }


    // Private static methods.
    private static async void SwitchActiveFrameAsync(GameFrame newFrame)
    {
        if (s_activeFrame == newFrame) return;

        await Task.Run(() => newFrame?.Load());

        Actions.Enqueue(() =>
        {
            GameFrame OldFrame = s_activeFrame;
            OldFrame.OnEnd();
            s_activeFrame = newFrame;
            newFrame.OnStart();

            Task.Run(() =>
            {
                OldFrame?.Unload();
                AssetManager.FreeMemory();
                GC.Collect();
            });
        });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void PrepareDrawBatch(Effect shader)
    {
        if (s_drawBatchBegun && (shader == s_drawBatchShader)) return;

        if (s_drawBatchBegun) s_drawBatch.End();

        s_loopInfo.DrawBatchCount++;
        s_drawBatchShader = shader;
        s_drawBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, effect: shader);
    }
}