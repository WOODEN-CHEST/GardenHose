using System;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHoseEngine.Frame.Item.Basic;

public class Line : ColoredItem
{
    // Private static fields.
    private static readonly Vector2 s_origin = new(0f, 0.5f);


    // Public fields.
    public float Thickness
    {
        get => _thickness;
        set
        {
            _thickness = value;
            UpdateIsDrawingNeeded();
        }
    }

    public float Length
    {
        get => _length;
        set
        {
            _length = value;
            UpdateIsDrawingNeeded();
        }
    }


    // Private fields.
    private float _thickness;
    private float _length;
    private float _lineRotation;


    // Constructors.
    public Line() : base() { }


    // Methods.
    public void Set(Vector2 startPosition, Vector2 endPosition)
    {
        Set(startPosition, Vector2.Distance(startPosition, endPosition),
            MathF.Atan2(endPosition.Y - startPosition.Y, endPosition.X - startPosition.X));
    }

    public void Set(Vector2 endPosition)
    {
        Set(Position, Vector2.Distance(Position, endPosition),
            MathF.Atan2(endPosition.Y - Position.Y, endPosition.X - Position.X));
    }

    public void Set(Vector2 position, float length, float rotation)
    {
        Position = position;
        Length = length;
        Rotation = rotation;
    }


    // Inherited methods.
    protected override void UpdateIsDrawingNeeded()
    {
        IsDrawingNeeded = (_thickness != 0f) && (_length != 0f) && (IsVisible) && (Opacity != 0f);
    }

    public override void Draw(IDrawInfo info)
    {
        if (!IsDrawingNeeded) return;

        info.SpriteBatch.Draw(Display.SinglePixel,
            Display.ToRealPosition(Position),
            null,
            CombinedMask,
            _lineRotation + Rotation,
            s_origin,
            Display.ToRealScale(new Vector2(_length, _thickness)),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}