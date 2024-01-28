using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame;

public static class GameFrameManager
{
    // Static fields.
    public static IGameFrame ActiveFrame { get; private set; }
    public static IGameFrame? NextFrame { get; private set; }
    public static BlendState DefaultBlendState => BlendState.NonPremultiplied;


    // Private static fields.
    private static ConcurrentQueue<Action> s_actions = new();
    private static SpriteBatch s_spriteBatch;
    private static RenderTarget2D s_layerPixelBuffer;
    private static RenderTarget2D s_framePixelBuffer;
    
    private static GenericDrawInfo s_drawInfo = new();


    // Static methods.
    public static void UpdateFrames(IProgramTime time)
    {
        while (s_actions.TryDequeue(out Action? ActionToExecute))
        {
            ActionToExecute!.Invoke();
        }


        ActiveFrame.Update(time);
    }

    public static void DrawFrames()
    {
        s_drawInfo.Time = s_time;

        Display.GraphicsManager.GraphicsDevice.SetRenderTarget(s_framePixelBuffer);
        Display.GraphicsManager.GraphicsDevice.Clear(Color.Black);
        ActiveFrame.Draw(s_drawInfo, s_layerPixelBuffer, s_framePixelBuffer);

        Display.GraphicsManager.GraphicsDevice.SetRenderTarget(null);
        s_spriteBatch.Begin(blendState: DefaultBlendState);
        s_spriteBatch.Draw(s_framePixelBuffer, Vector2.Zero, ActiveFrame.CombinedMask);
        s_spriteBatch.End();
    }

    public static void EnqueueAction(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        s_actions.Enqueue(action);
    }


    // Internal static methods.
    internal static void Initialize(IGameFrame activeFrame)
    {
        ActiveFrame = activeFrame ?? throw new ArgumentNullException(nameof(activeFrame));

        activeFrame.Load();

        EnqueueAction(() => { activeFrame.OnStart(); });

        Display.DisplayChanged += OnDisplayChangedEvent;
        s_spriteBatch = new(Display.GraphicsManager.GraphicsDevice);
        CreateLayerPixelBuffer();
    }


    // Private methods.
    private static void OnDisplayChangedEvent(object? sender, EventArgs args) => CreateLayerPixelBuffer();

    [MemberNotNull(nameof(s_layerPixelBuffer))]
    private static void CreateLayerPixelBuffer()
    {
        s_layerPixelBuffer?.Dispose();
        s_framePixelBuffer?.Dispose();

        s_layerPixelBuffer = new(Display.GraphicsManager.GraphicsDevice,
            (int)Display.WindowSize.X, (int)Display.WindowSize.Y, false, SurfaceFormat.Color, 
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        s_framePixelBuffer = new(Display.GraphicsManager.GraphicsDevice,
            (int)Display.WindowSize.X, (int)Display.WindowSize.Y, false, SurfaceFormat.Color,
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }


    // Inherited methods.
    public static void LoadNextFrame(IGameFrame nextFrame, Action onLoadComplete)
    {
        if  (NextFrame != null)
        {
            throw new InvalidOperationException("Next frame is already set.");
        }
        if (nextFrame == null)
        {
            throw new ArgumentNullException(nameof(nextFrame));
        }
        if (onLoadComplete == null)
        {
            throw new ArgumentNullException(nameof(onLoadComplete));
        }

        NextFrame = nextFrame;

        Task.Run(() =>
        {
            try
            {
                nextFrame.Load();
                s_actions.Enqueue(onLoadComplete);
            }
            catch (Exception e)
            {
                s_actions.Enqueue(() => throw new Exception(
                    $"Exception during loading of frame \"{nextFrame.Name}\". {e}"));
            }
        });
    }

    public static void JumpToNextFrame() => EnqueueAction(() =>
    {
        if (NextFrame == null)
        {
            throw new InvalidOperationException("Next frame is null.");
        }

        lock (NextFrame)
        {
            if (!NextFrame.IsLoaded)
            {
                throw new InvalidOperationException("Next frame isn't loaded");
            }
        }

        ActiveFrame.OnEnd();
        IGameFrame OldFrame = ActiveFrame;
        ActiveFrame = NextFrame;
        NextFrame = null;

        ActiveFrame.OnStart();
        Task.Run(() =>
        {
            try
            {
                OldFrame.Unload();
                GC.Collect();
            }
            catch (Exception e)
            {
                s_actions.Enqueue(() => throw new Exception(
                    $"Exception unloading old frame \"{OldFrame.Name}\". {e}"));
            }
        });
    });
}