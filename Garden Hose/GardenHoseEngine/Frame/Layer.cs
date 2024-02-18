using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame;

public class Layer : ILayer
{
    // Fields.
    public string Name { get; init; }

    public bool IsVisible { get; set; }

    public Effect? Shader { get; set; }

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

    public int DrawableItemCount => _drawableItems.Count;



    // Private fields.
    private readonly List<LayerItem> _drawableItems = new();
    private ColorMask _colorMask = new();
    

    // Constructors.
    public Layer(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }


    // Inherited  methods.
    public void AddDrawableItem(IDrawableItem item)
    {
        AddDrawableItem(item, ILayer.DEFAULT_Z_INDEX);
    }

    public void AddDrawableItem(IDrawableItem item, float zIndex)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        LayerItem LItem = new LayerItem(item, zIndex);

        // Quick add.
        if (_drawableItems.Count > 0)
        {
            if (_drawableItems[0].ZIndex > zIndex)
            {
                _drawableItems.Insert(0, LItem);
                return;
            }
            if (_drawableItems[^1].ZIndex <= zIndex)
            {
                _drawableItems.Add(LItem);
                return;
            }
        }


        // Find index.
        for (int i = 0; i < _drawableItems.Count; i++)
        {
            if (_drawableItems[i].ZIndex > zIndex)
            {
                _drawableItems.Insert(i, LItem);
                return;
            }
        }

        _drawableItems.Add(LItem);
    }

    public void RemoveDrawableItem(IDrawableItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        for (int i = 0; i < _drawableItems.Count; i++)
        {
            if (_drawableItems[i].Item == item)
            {
                _drawableItems.RemoveAt(i);
                return;
            }
        }
    }

    public void ClearDrawableItems()
    {
        _drawableItems.Clear();
    }

    public void Draw(IDrawInfo info)
    {
        Effect? ActiveEffect = _drawableItems[0].Item.Shader;
        info.SpriteBatch.Begin(effect: ActiveEffect, blendState: GameFrameManager.DefaultBlendState);

        foreach (LayerItem LItem in _drawableItems)
        {
            if (LItem.Item.Shader != ActiveEffect)
            {
                info.SpriteBatch.End();
                ActiveEffect = LItem.Item.Shader;
                info.SpriteBatch.Begin(effect: ActiveEffect, blendState: GameFrameManager.DefaultBlendState);
            }

            LItem.Item.Draw(info);
        }

        info.SpriteBatch.End();
    }
}