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

        _drawableItems.Add(item);
    }

    public void AddDrawableItem(IDrawableItem item, int index)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        index = Math.Clamp(index, 0, DrawableItemCount - 1);
        _drawableItems.Insert(index, item);
    }

    public void RemoveDrawableItem(IDrawableItem item)
    {
        if (item == null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        _drawableItems.Remove(item);
    }

    public void ClearDrawableItems()
    {
        _drawableItems.Clear();
    }

    public void Draw()
    {
        List<IDrawableItem> ShaderedDrawables = new();

        GameFrameManager.s_spriteBatch.Begin(blendState: BlendState.NonPremultiplied);
        foreach (IDrawableItem Item in _drawableItems)
        {
            if (Item.Shader!= null)
            {
                ShaderedDrawables.Add(Item);
                continue;
            }

            Item.Draw();
        }
        GameFrameManager.s_spriteBatch.End();

        if (ShaderedDrawables.Count == 0)
        {
            return;
        }


        Effect AppliedShader = ShaderedDrawables[0].Shader!;
        GameFrameManager.s_spriteBatch.Begin(blendState: BlendState.NonPremultiplied, effect: AppliedShader);

        foreach (IDrawableItem Item in ShaderedDrawables)
        {
            if (AppliedShader != Item.Shader)
            {
                GameFrameManager.s_spriteBatch.End();

                AppliedShader = Item.Shader!;
                GameFrameManager.s_spriteBatch.Begin(blendState: BlendState.NonPremultiplied, effect: AppliedShader);
            }

            Item.Draw();
        }

        GameFrameManager.s_spriteBatch.End();
    }
}