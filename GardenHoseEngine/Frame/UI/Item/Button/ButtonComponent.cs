using GardenHoseEngine.Extensions;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using System;


namespace GardenHoseEngine.Frame.UI.Item;

public struct ButtonComponent
{
    // Fields.
    public readonly ButtonShape Shape;
    public Vector2 Position;

    public Vector2 Size;
    public float Radius
    {
        set => Size.X = value * value;
        get => MathF.Sqrt(Size.X);
    }

    public float Rotation
    {
        get => _rotation;
        set
        {
            if (float.IsNaN(value) || float.IsInfinity(value))
            {
                throw new ArgumentException($"Invalid button rotation: \"{value}\"");
            }

            _rotation = value;
            _isRotated = value is not -0f and not 0f;
        }
    }


    // Private fields.
    private float _rotation = 0f;
    private bool _isRotated = false;


    // Constructors.
    public ButtonComponent(Vector2 position, Vector2 size)
    {
        Shape = ButtonShape.Rectangle;
        Position = position;
        Size = size;
    }
    
    public ButtonComponent(Vector2 position, float radius)
    {
        Shape = ButtonShape.Circle;
        Position = position;
        Radius = radius;
    }


    // Methods.
    public bool IsMouseOver()
    {
        if (Shape == ButtonShape.Circle)
        {
            return Vector2.DistanceSquared(Position, UserInput.VirtualMousePosCur) <= Size.X;
        }

        Vector2 Mouse = UserInput.VirtualMousePosCur;

        if (_isRotated) Mouse.Rotate(_rotation);

        return ((Mouse.X >= Position.X) && (Mouse.X <= Position.X + Size.X))
            && ((Mouse.Y >= Position.Y) && (Mouse.Y <= Position.Y + Size.Y));
    }
}