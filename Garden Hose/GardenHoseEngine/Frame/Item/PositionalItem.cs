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
    public PositionalItem()
    {
        Position = new();
        Scale = new();
        Scale.Vector = Vector2.One;
    }


    // Inherited methods.
    public virtual void Draw() { }

    public virtual void Update()
    {
        Scale.Update();
        Position.Update();
    }
}