﻿using GardenHoseEngine.Screen;
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
        ActiveFrame.Load(_assetManager);
        ActiveFrame.OnStart();

        GlobalFrame = globalFrame ?? throw new ArgumentNullException(nameof(globalFrame));
        GlobalFrame.Load(_assetManager);
        GlobalFrame.OnStart();

        _display.DisplayChanged += OnDisplayChangedEvent;
        CreateLayerPixelBuffer();

        _spriteBatch = new(_graphicsDeviceManager.GraphicsDevice);
    }


    // Methods.
    public void UpdateFrames(TimeSpan passedTime)
    {
        while (_actions.TryDequeue(out Action? ActionToExecute))
        {
            ActionToExecute!.Invoke();
        }

        ActiveFrame.Update(passedTime);
        GlobalFrame.Update(passedTime);
    }

    public void DrawFrames(TimeSpan passedTime)
    {
        _graphicsDeviceManager.GraphicsDevice.Clear(Color.Transparent);
        ActiveFrame.Draw(passedTime, _graphicsDeviceManager.GraphicsDevice, _spriteBatch, _layerPixelBuffer);
        GlobalFrame.Draw(passedTime, _graphicsDeviceManager.GraphicsDevice, _spriteBatch, _layerPixelBuffer);
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
        _layerPixelBuffer = new(_graphicsDeviceManager.GraphicsDevice,
            (int)_display.WindowSize.X, (int)_display.WindowSize.Y, false, SurfaceFormat.Color, 
            DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
    }


    // Inherited methods.
    public void LoadNextFrame(IGameFrame nextFrame, Action onLoadComplete)
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

        Task.Run(() =>
        {
            try
            {
                nextFrame.Load(_assetManager);
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