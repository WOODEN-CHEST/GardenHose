using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GardenHose.Engine.Frame.UI.Item;

public class Line : ColoredItem
{
    // Public fields.
    public float Thickness
    {
        get => _thickness;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || (value < 0f))
            {
                throw new ArgumentException($"Invalid line thickness: \"{value}\"");
            }

            _thickness = value;
            _origin.Y = _thickness / 2f;
            ShouldDraw = IsDrawingNeeded();
        }
    }

    public float Length
    {
        get => _length;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value) || (value < 0f))
            {
                throw new ArgumentException($"Invalid line length: \"{value}\"");
            }

            _length = value;
            ShouldDraw = IsDrawingNeeded();
        }
    }


    // Private fields.
    private float _thickness;
    private float _length;
    private Vector2 _origin = Vector2.Zero;


    // Constructors.
    public Line() { }

    public Line(Vector2 startPosition, Vector2 endPosition, float thickness)
    {
        Set(startPosition, endPosition);
        Thickness = thickness;
    }

    public Line(Vector2 position, float length, float thickness, float rotation)
    {
        Set(position, length, rotation);
        Thickness = thickness;
    }


    // Methods.
    public void Set(Vector2 startPosition, Vector2 endPosition)
    {
        Set(startPosition, Vector2.Distance(startPosition, endPosition), 
            MathF.Atan2(endPosition.Y - startPosition.Y, endPosition.X - startPosition.X));
    }

    public void Set(Vector2 position, float length, float rotation)
    {
        Position = position;
        Length = length;
        Rotation = rotation;
    }


    // Inherited methods.
    protected override bool IsDrawingNeeded() => base.IsDrawingNeeded() && (_thickness != 0f) && (_length != 0f);

    public override void Draw()
    {
        base.Draw();

        if (!ShouldDraw) return;

        GameFrame.DrawTexture(GameFrame.SinglePixel,
            RealPosition,
            null,
            RealColorMask,
            Rotation,
            _origin,
            new Vector2(_length, _thickness));
    }
}