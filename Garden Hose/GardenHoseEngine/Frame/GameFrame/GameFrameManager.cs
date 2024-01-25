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

    

    public static float PassedTimeSeconds { get; private set; } = 0f;

    public static float TotalTimeSeconds { get; private set; } = 0f;

    public static RenderTarget2D LayerPixelBuffer { get; private set; }

    public static RenderTarget2D FramePixelBuffer { get; private set; }


    // Private fields.
    private static ConcurrentQueue<Action> s_actions = new();
    private static SpriteBatch s_spriteBatch;


    // Static methods.
    public static void UpdateFrames(float passedTimeSeconds)
    {
        PassedTimeSeconds = passedTimeSeconds;
        TotalTimeSeconds += PassedTimeSeconds;

        while (s_actions.TryDequeue(out Action? ActionToExecute))
        {
            ActionToExecute!.Invoke();
        }


        ActiveFrame.Update();
    }

    public static void DrawFrames()
    {
        Display.GraphicsManager.GraphicsDevice.SetRenderTarget(FramePixelBuffer);
        Display.GraphicsManager.GraphicsDevice.Clear(Color.Black);

        ActiveFrame.Draw();

        Display.GraphicsManager.GraphicsDevice.SetRenderTarget(null);
        s_spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        s_spriteBatch.Draw(FramePixelBuffer, Vector2.Zero, Color.White);
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
        if (activeFrame== null)
        {
            throw new ArgumentNullException(nameof(activeFrame));
        }

        ActiveFrame = activeFrame ?? throw new ArgumentNullException(nameof(activeFrame));

        activeFrame.BeginLoad();
        activeFrame.Load();
        activeFrame.FinalizeLoad();

        EnqueueAction(() => { activeFrame.OnStart(); });

        Display.DisplayChanged += OnDisplayChangedEvent;
        s_spriteBatch = new(Display.GraphicsManager.GraphicsDevice);
        CreateLayerPixelBuffer();
    }


    // Private methods.
    private static void OnDisplayChangedEvent(object? sender, EventArgs args) => CreateLayerPixelBuffer();

    [MemberNotNull(nameof(LayerPixelBuffer))]
    private static void CreateLayerPixelBuffer()
    {
        LayerPixelBuffer?.Dispose();
        FramePixelBuffer?.Dispose();

        LayerPixelBuffer = new(Display.GraphicsManager.GraphicsDevice,
            (int)Display.WindowSize.X, (int)Display.WindowSize.Y, false, SurfaceFormat.Color, 
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        FramePixelBuffer = new(Display.GraphicsManager.GraphicsDevice,
            (int)Display.WindowSize.X, (int)Display.WindowSize.Y, false, SurfaceFormat.Color,
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }


    // Inherited methods.
    public static async void LoadNextFrame(IGameFrame nextFrame, Action onLoadComplete)
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

        await Task.Run(() =>
        {
            try
            {
                nextFrame.BeginLoad();
                nextFrame.Load();
                nextFrame.FinalizeLoad();
            }
            catch (Exception e)
            {
                s_actions.Enqueue(() => throw new Exception(
                    $"Exception during loading of frame \"{nextFrame.Name}\". {e}"));
            }
        });

        s_actions.Enqueue(onLoadComplete);
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
                AssetManager.FreeUnusedAssets();
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