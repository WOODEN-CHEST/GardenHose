using GardenHoseEngine.Collections;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;

namespace GardenHoseEngine.Frame;

public class GameFrame : IGameFrame
{
    // Fields.
    public string Name { get; set; }

    public bool IsLoaded { get; internal set; } = false;

    public ILayer? TopLayer
    {
        get => _layers.Count == 0 ? null : _layers[^1];
    }

    public ILayer? BottomLayer
    {
        get => (_layers.Count == 0 ? null : _layers[0]);
    }

    public int LayerCount => _layers.Count;

    public PropertyAnimManager AnimManager { get; } = new();


    // Private fields.
    private readonly List<ILayer> _layers = new();
    private readonly DiscreteTimeList<ITimeUpdatable> _updateableItems = new();


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
    public void AddItem(ITimeUpdatable item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        _updateableItems.Add(item);
    }

    public void RemoveItem(ITimeUpdatable item)
    {
        ArgumentNullException.ThrowIfNull(item, nameof(item));
        _updateableItems.Remove(item);
    }

    public void ClearItems()
    {
        _updateableItems.Clear();
    }


    /* Updating and drawing. */
    public virtual void Update()
    {
        _updateableItems.ApplyChanges();

        foreach (var Item in _updateableItems)
        {
            Item.Update();
        }
    }

    public virtual void Draw()
    {
        if (_layers.Count == 0) return;

        foreach (ILayer FrameLayer in _layers)
        {
            Display.GraphicsManager.GraphicsDevice.SetRenderTarget(GameFrameManager.LayerPixelBuffer);
            Display.GraphicsManager.GraphicsDevice.Clear(Color.Transparent);
            FrameLayer.Draw();

            Display.GraphicsManager.GraphicsDevice.SetRenderTarget(GameFrameManager.FramePixelBuffer);
            GameFrameManager.s_spriteBatch.Begin(blendState: BlendState.NonPremultiplied, effect: FrameLayer.Shader);
            GameFrameManager.s_spriteBatch.Draw(GameFrameManager.LayerPixelBuffer, Vector2.Zero, FrameLayer.CombinedMask);
            GameFrameManager.s_spriteBatch.End();
        }
    }

    /* Status control. */
    public void BeginLoad()
    {
        AssetManager.RegisterGameFrame(this);
    }

    public virtual void Load()
    {
        
    }

    public void FinalizeLoad()
    {
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
        ClearItems();
    }

    public virtual void Unload()
    {
        AssetManager.UnregisterGameFrame(this);
        IsLoaded = false;
    }
}
