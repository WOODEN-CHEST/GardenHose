using GardenHose.Engine.IO;
using Microsoft.Xna.Framework;


namespace GardenHose.Engine.Frame.UI.Item;

public struct ButtonComponent
{
    // Fields.
    public ButtonShape Shape;
    public Vector2 Position;
    public Vector2 Size;


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
        Size.X = radius;
    }


    // Methods.
    public bool IsMouseOver()
    {
        if (Shape == ButtonShape.Circle)
        {
            return Vector2.DistanceSquared(Position, UserInput.VirtualMouseCur) <= Size.X;
        }

        Vector2 Mouse = UserInput.VirtualMouseCur;
        return ((Mouse.X >= Position.X) && (Mouse.X <= Position.X + Size.X))
            && ((Mouse.Y >= Position.Y) && (Mouse.Y <= Position.Y + Size.Y));
    }
}