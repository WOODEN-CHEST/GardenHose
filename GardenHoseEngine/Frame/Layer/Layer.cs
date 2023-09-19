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

    public IDrawer Drawer => this;

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
    private readonly List<IDrawableItem> _drawableItems = new();
    private readonly Dictionary<IDrawableItem, Effect> _drawableItemsWithShader = new();

    private ColorMask _colorMask = new();
    

    // Constructors.
    public Layer(GraphicsDeviceManager graphicsManager, string name)
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
            _drawableItemsWithShader.Add(item, item.Shader);
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

        _drawableItemsWithShader.Remove(item);
    }

    public void OnShaderChange(IDrawableItem item)
    {
        RemoveDrawableItem(item);
        AddDrawableItem(item);
    }

    public void ClearDrawableItems()
    {
        _drawableItems.Clear();
        _drawableItemsWithShader.Clear();
    }

    public void Draw(TimeSpan passedTime, SpriteBatch spriteBatch)
    {
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

        foreach (var ItemAndShader in _drawableItemsWithShader)
        {
            spriteBatch.Begin(blendState: BlendState.NonPremultiplied, effect: ItemAndShader.Value);
            ItemAndShader.Key.Draw(passedTime, spriteBatch);
            spriteBatch.End();
        }
    }
}