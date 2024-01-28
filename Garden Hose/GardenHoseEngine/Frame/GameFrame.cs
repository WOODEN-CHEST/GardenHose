using GardenHoseEngine.Collections;
using GardenHoseEngine.Engine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    public PropertyAnimManager AnimationManager { get; } = new();

    public float Brightness
    {
        get => _colorMask.Brightness;
        set => _colorMask.Brightness = value;
    }

    public float Opacity
    {
        get => _colorMask.Opacity;
        set => _colorMask.Opacity = value;
    }

    public Color Mask
    {
        get => _colorMask.Mask;
        set => _colorMask.Mask = value;
    }

    public Color CombinedMask => _colorMask.CombinedMask;


    // Private fields.
    private readonly List<ILayer> _layers = new();
    private readonly DiscreteTimeList<ITimeUpdatable> _updateableItems = new();
    private ColorMask _colorMask;


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
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        _updateableItems.Add(item);
    }

    public void RemoveItem(ITimeUpdatable item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }
        _updateableItems.Remove(item);
    }

    public void ClearItems()
    {
        _updateableItems.Clear();
    }


    /* Updating and drawing. */
    public virtual void Update(IProgramTime time)
    {
        _updateableItems.ApplyChanges();
        AnimationManager.Update(time);

        foreach (var Item in _updateableItems)
        {
            Item.Update(time);
        }
    }

    public virtual void Draw(IDrawInfo info, RenderTarget2D layerPixelBuffer, RenderTarget2D framePixelBuffer)
    {
        if (_layers.Count == 0) return;

        foreach (ILayer FrameLayer in _layers)
        {
            if (FrameLayer.DrawableItemCount == 0)
            {
                continue;
            }

            Display.GraphicsManager.GraphicsDevice.SetRenderTarget(layerPixelBuffer);
            Display.GraphicsManager.GraphicsDevice.Clear(Color.Transparent);
            FrameLayer.Draw(info);

            Display.GraphicsManager.GraphicsDevice.SetRenderTarget(framePixelBuffer);
            info.SpriteBatch.Begin(blendState: GameFrameManager.DefaultBlendState, effect: FrameLayer.Shader);
            info.SpriteBatch.Draw(layerPixelBuffer, Vector2.Zero, FrameLayer.CombinedMask);
            info.SpriteBatch.End();
        }
    }

    /* Status control. */
    public virtual void Load()
    {
        IsLoaded = true;
    }

    public virtual void OnStart() { }

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
        AssetManager.FreeAllUserAssets(this);
        IsLoaded = false;
    }
}
