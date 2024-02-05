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

    public void Draw(IDrawInfo info)
    {
        List<IDrawableItem> ShaderedDrawables = null!;

        info.SpriteBatch.Begin(blendState: GameFrameManager.DefaultBlendState);
        foreach (IDrawableItem Item in _drawableItems)
        {
            if (Item.Shader != null)
            {
                ShaderedDrawables ??= new();
                ShaderedDrawables.Add(Item);
                continue;
            }

            Item.Draw(info);
        }
        info.SpriteBatch.End();

        if (ShaderedDrawables == null)
        {
            return;
        }

        Effect AppliedShader = ShaderedDrawables[0].Shader;
        info.SpriteBatch.Begin(blendState: GameFrameManager.DefaultBlendState, effect: AppliedShader);

        foreach (IDrawableItem Item in ShaderedDrawables)
        {
            if (AppliedShader != Item.Shader)
            {
                info.SpriteBatch.End();

                AppliedShader = Item.Shader!;
                info.SpriteBatch.Begin(blendState: GameFrameManager.DefaultBlendState, effect: AppliedShader);
            }

            Item.Draw(info);
        }

        info.SpriteBatch.End();
    }
}