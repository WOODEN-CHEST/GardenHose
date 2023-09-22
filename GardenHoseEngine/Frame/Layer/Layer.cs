using GardenHoseEngine.Collections;
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

    public IDrawer? Drawer
    {
        get => this;
        set { }
    }

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
    private readonly DiscreteTimeList<IDrawableItem> _drawableItems = new();
    private readonly DiscreteTimeList<(IDrawableItem Item, Effect Shader)> _drawableItemsWithShader = new();

    private ColorMask _colorMask = new();
    

    // Constructors.
    public Layer(string name)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }


    // Inherited  methods.
    public void AddDrawableItem(IDrawableItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (item.Shader == null)
        {
            _drawableItems.Add(item);
        }
        else
        {
            _drawableItemsWithShader.Add((item, item.Shader));
        }

    }

    public void RemoveDrawableItem(IDrawableItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (item.Shader == null)
        {
            _drawableItems.Remove(item);
            return;
        }

        _drawableItemsWithShader.Remove((item, item.Shader));
    }

    public void OnShaderChange(IDrawableItem item)
    {
        RemoveDrawableItem(item);
        AddDrawableItem(item);
    }

    public void ClearDrawableItems()
    {
        _drawableItems.ForceClear();
        _drawableItemsWithShader.ForceClear();
    }

    public void Draw(TimeSpan passedTime, SpriteBatch spriteBatch)
    {
        _drawableItems.ApplyChanges();
        DrawShaderlessItems(passedTime, spriteBatch);
        DrawShaderedItems(passedTime, spriteBatch);
    }


    // Private methods.
    private void DrawShaderlessItems(TimeSpan passedTime, SpriteBatch spriteBatch)
    {
        if (_drawableItems.Count == 0) return;

        spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        foreach (IDrawableItem Item in _drawableItems)
        {
            Item.Draw(passedTime, spriteBatch);
        }
        spriteBatch.End();
    }

    private void DrawShaderedItems(TimeSpan passedTime, SpriteBatch spriteBatch)
    {
        if (_drawableItemsWithShader.Count == 0) return;

        foreach (var Drawable in _drawableItemsWithShader)
        {
            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, effect: Drawable.Shader);
            Drawable.Item.Draw(passedTime, spriteBatch);
            spriteBatch.End();
        }
    }
}