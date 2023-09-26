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
            if (!float.IsFinite(value))
            {
                throw new ArgumentException($"Invalid line thickness: \"{value}\"");
            }

            _thickness = value;
            UpdateShouldDraw();
        }
    }

    public float Length
    {
        get => _length;
        set
        {
            if (!float.IsFinite(value))
            {
                throw new ArgumentException($"Invalid line length: \"{value}\"");
            }

            _length = value;
            UpdateShouldDraw();
        }
    }


    // Private fields.
    private readonly Texture2D _singlePixel;
    private float _thickness;
    private float _length;
    private float _lineRotation;


    // Constructors.
    public Line(IVirtualConverter converter, Texture2D singlePixel)
        : base(converter)
    {
        _singlePixel = singlePixel ?? throw new ArgumentNullException(nameof(singlePixel));
    }


    // Methods.
    public void Set(Vector2 startPosition, Vector2 endPosition)
    {
        Set(startPosition, Vector2.Distance(startPosition, endPosition),
            MathF.Atan2(endPosition.Y - startPosition.Y, endPosition.X - startPosition.X));
    }

    public void Set(Vector2 endPosition)
    {
        Set(Position, Vector2.Distance(Position, endPosition),
            MathF.Atan2(endPosition.Y - Position.Vector.Y, endPosition.X - Position.Vector.X));
    }

    public void Set(Vector2 position, float length, float rotation)
    {
        Position.Vector = position;
        Length = length;
        Rotation = rotation;
    }


    // Inherited methods.
    protected override void UpdateShouldDraw()
    {
        _ShouldDraw = (_thickness != 0f) && (_length != 0f) && (IsVisible) && (Opacity != 0f);
    }

    public override void Draw(float passedTimeSeconds, SpriteBatch spriteBatch)
    {
        if (!_ShouldDraw) return;

        spriteBatch.Draw(_singlePixel,
            Converter.ToRealPosition(Position),
            null,
            CombinedMask,
            _lineRotation + Rotation,
            s_origin,
            Converter.ToRealScale(Scale) * new Vector2(_length, _thickness),
            SpriteEffects.None,
            IDrawableItem.DEFAULT_LAYER_DEPTH);
    }
}