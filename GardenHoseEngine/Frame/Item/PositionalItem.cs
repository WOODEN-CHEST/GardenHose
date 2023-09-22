using System;
using System.Diagnostics.CodeAnalysis;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame.Item;

public class PositionalItem : IDrawableItem
{
    // Fields.
    public virtual bool IsVisible { get; set; }

    public virtual Effect? Shader { get; set; }

    public virtual IDrawer? Drawer { get; set; }

    public IVirtualConverter Converter { get; private init; }

    public virtual AnimVector2 Position { get; private init; }

    public virtual AnimVector2 Scale { get; private init; }

    public float Rotation
    {
        get => _rotation;
        set
        {
            if (!float.IsFinite(value))
            {
                throw new ArgumentException($"Invalid rotation: {value}", nameof(value));
            }

            _rotation = value;
        }
    }


    // Private fields.
    private float _rotation = 0f;


    // Constructors.
    public PositionalItem(ITimeUpdater updater, IVirtualConverter converter, IDrawer? drawer)
    {
        ArgumentNullException.ThrowIfNull(updater, nameof(updater));
        Converter = converter ?? throw new ArgumentNullException(nameof(converter));

        Position = new(updater);
        Scale = new(updater);
        Scale.Vector = Vector2.One;
        Drawer = drawer;
        Drawer?.AddDrawableItem(this);
    }


    // Inherited methods.
    public virtual void Draw(TimeSpan passedTime, SpriteBatch spriteBatch) { }
}