using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame;

public class GameFrameManager : IGameFrameManager
{
    // Fields.
    public IGameFrame GlobalFrame { get; init; }
    public IGameFrame ActiveFrame { get; private set; }
    public IGameFrame? NextFrame { get; private set; }


    // Private fields.
    private GraphicsDeviceManager _graphicsDeviceManager;
    private Display _display;
    private AssetManager _assetManager;

    private ConcurrentQueue<Action> _actions = new();
    private SpriteBatch _spriteBatch;
    private RenderTarget2D _layerPixelBuffer;
    private RenderTarget2D _framePixelBuffer;


    // Constructors.
    internal GameFrameManager(GraphicsDeviceManager graphicsManager,
        Display display,
        AssetManager assetManager,
        IGameFrame activeFrame,
        IGameFrame globalFrame)
    {
        _graphicsDeviceManager = graphicsManager ?? throw new ArgumentNullException(nameof(graphicsManager));
        _display = display ?? throw new ArgumentNullException(nameof(display));
        _assetManager = assetManager ?? throw new ArgumentNullException(nameof(assetManager));

        ActiveFrame = activeFrame ?? throw new ArgumentNullException(nameof(activeFrame));
        GlobalFrame = globalFrame ?? throw new ArgumentNullException(nameof(globalFrame));

        EnqueueAction( () =>
        {
            GlobalFrame.BeginLoad(_assetManager);
            GlobalFrame.Load(_assetManager);
            GlobalFrame.FinalizeLoad(_assetManager);
            GlobalFrame.OnStart();

            ActiveFrame.BeginLoad(_assetManager);
            ActiveFrame.Load(_assetManager);
            ActiveFrame.FinalizeLoad(_assetManager);
            ActiveFrame.OnStart();
        });
        

        _display.DisplayChanged += OnDisplayChangedEvent;
        CreateLayerPixelBuffer();

        _spriteBatch = new(_graphicsDeviceManager.GraphicsDevice);
    }


    // Methods.
    public void UpdateFrames(float passedTimeSeconds)
    {
        while (_actions.TryDequeue(out Action? ActionToExecute))
        {
            ActionToExecute!.Invoke();
        }

        ActiveFrame.Update(passedTimeSeconds);
        GlobalFrame.Update(passedTimeSeconds);
    }

    public void DrawFrames(float passedTimeSeconds)
    {
        _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(_framePixelBuffer);
        _graphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

        ActiveFrame.Draw(passedTimeSeconds, _graphicsDeviceManager.GraphicsDevice, _spriteBatch, _layerPixelBuffer, _framePixelBuffer);

        _graphicsDeviceManager.GraphicsDevice.SetRenderTarget(null);
        _spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        _spriteBatch.Draw(_framePixelBuffer, Vector2.Zero, Color.White);
        _spriteBatch.End();
    }

    public void EnqueueAction(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        _actions.Enqueue(action);
    }


    // Private methods.
    private void OnDisplayChangedEvent(object? sender, EventArgs args) => CreateLayerPixelBuffer();

    [MemberNotNull(nameof(_layerPixelBuffer))]
    private void CreateLayerPixelBuffer()
    {
        _layerPixelBuffer?.Dispose();
        _framePixelBuffer?.Dispose();

        _layerPixelBuffer = new(_graphicsDeviceManager.GraphicsDevice,
            (int)_display.WindowSize.X, (int)_display.WindowSize.Y, false, SurfaceFormat.Color, 
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
        _framePixelBuffer = new(_graphicsDeviceManager.GraphicsDevice,
            (int)_display.WindowSize.X, (int)_display.WindowSize.Y, false, SurfaceFormat.Color,
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }


    // Inherited methods.
    public async void LoadNextFrame(IGameFrame nextFrame, Action onLoadComplete)
    {
        if  (NextFrame  != null)
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
                nextFrame.BeginLoad(_assetManager);
                nextFrame.Load(_assetManager);
                nextFrame.FinalizeLoad(_assetManager);
            }
            catch (Exception e)
            {
                _actions.Enqueue(() => throw new Exception(
                    $"Exception during loading of frame \"{nextFrame.Name}\". {e}"));
            }
        });

        _actions.Enqueue(onLoadComplete);
    }

    public void JumpToNextFrame() => EnqueueAction(() =>
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
                OldFrame.Unload(_assetManager);
                _assetManager.FreeUnusedAssets();
                GC.Collect();
            }
            catch (Exception e)
            {
                _actions.Enqueue(() => throw new Exception(
                    $"Exception unloading old frame \"{OldFrame.Name}\". {e}"));
            }
        });
    });
}