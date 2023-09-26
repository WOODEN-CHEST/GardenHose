using System;
using System.Diagnostics.CodeAnalysis;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame.Item;

public class PositionalItem : IDrawableItem, ITimeUpdatable
{
    // Fields.
    public virtual bool IsVisible { get; set; }

    public virtual Effect? Shader { get; set; }

    public IVirtualConverter Converter { get; private init; }

    public virtual AnimVector2 Position { get; private init; }

    public virtual AnimVector2 Scale { get; private init; }

    public  virtual float Rotation
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
    public PositionalItem(IVirtualConverter converter)
    {
        Converter = converter ?? throw new ArgumentNullException(nameof(converter));

        Position = new();
        Scale = new();
        Scale.Vector = Vector2.One;
    }


    // Inherited methods.
    public virtual void Draw(float passedTimeSeconds, SpriteBatch spriteBatch) { }

    public virtual void Update(float passedTimeSeconds)
    {
        Scale.Update(passedTimeSeconds);
        Position.Update(passedTimeSeconds);
    }
}