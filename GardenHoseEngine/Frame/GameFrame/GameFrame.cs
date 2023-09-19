using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace GardenHoseEngine.Frame;


public class GameFrame : IGameFrame
{
    // Fields.
    [MemberNotNull(nameof(_name))]
    public string Name { get; set; }

    public bool IsLoaded { get; private set; } = false;

    public ILayer? TopLayer
    {
        get => _layers.Count == 0 ? null : _layers[^1];
    }

    public ILayer? BottomLayer
    {
        get => (_layers.Count == 0 ? null : _layers[0]);
    }

    public int LayerCount => _layers.Count;


    // Private fields.
    private string _name;
    private readonly List<ILayer> _layers = new();

    private readonly HashSet<ITimeUpdateable> _updateableItems = new();


    // Constructors.
    public GameFrame(string? name)
    {
        Name = name ?? "Unnamed frame";
    }


    // Inherited methods.
    /* Layers. */
    public void AddLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer, nameof(layer));
        _layers.Add(layer);
    }

    public void RemoveLayer(ILayer layer)
    {
        ArgumentNullException.ThrowIfNull(layer, nameof(layer));
        _layers.Remove(layer);
    }

    public ILayer? GetLayer(int index)
    {
        if ((index < 0) || (index >  LayerCount - 1))
        {
            return null;
        }

        return _layers[index];
    }

    public ILayer? GetLayer(string name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        
        foreach (var Layer in _layers)
        {
            if (Layer.Name == name) return Layer;
        }
        return null;
    }


    /* Items. */
    public void AddUpdateable(ITimeUpdateable item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        _updateableItems.Add(item);
    }

    public void RemoveUpdateable(ITimeUpdateable item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        _updateableItems.Remove(item);
    }

    public void ClearUpdateables()
    {
        foreach (var Item in _updateableItems)
        {
            Item.ForceRemove();
        }
        _updateableItems.Clear();
    }


    /* Updating and drawing. */
    public virtual void Update(TimeSpan passedTime)
    {
        foreach (var Item in _updateableItems)
        {
            Item.Update(passedTime);
        }
    }

    public virtual void Draw(TimeSpan passedTime, GraphicsDevice graphicsDevice, SpriteBatch spriteBatch, RenderTarget2D layerPixelBuffer)
    {
        if (_layers.Count == 0) return;

        foreach (ILayer FrameLayer in _layers)
        {
            graphicsDevice.SetRenderTarget(layerPixelBuffer);
            graphicsDevice.Clear(Color.Transparent);
            FrameLayer.Draw(passedTime, spriteBatch);


            graphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, effect: FrameLayer.Shader);
            spriteBatch.Draw(layerPixelBuffer, Vector2.Zero, FrameLayer.CombinedMask);
            spriteBatch.End();
        }
    }

    /* Status control. */
    public virtual void Load(AssetManager assetManager)
    {
        assetManager.RegisterGameFrame(this);
        IsLoaded = true;
    }

    public virtual void OnStart()
    {

    }

    public virtual void Restart()
    {
        OnStart();
    }

    public virtual void OnEnd()
    {
        ClearUpdateables();
    }

    public virtual void Unload(AssetManager assetManager)
    {
        assetManager.UnregisterGameFrame(this);
        IsLoaded = false;
    }
}
