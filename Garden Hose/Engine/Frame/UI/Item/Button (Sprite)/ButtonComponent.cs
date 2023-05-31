using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHose.Engine.Frame.UI.Item;

public struct ButtonComponent
{
    // Fields.
    public Vector2 Position;
    public ButtonShape Shape;
    public Vector2 Size;
    public float Radius;


    // Constructors.
    public ButtonComponent(Vector2 relativePosition, Vector2 size)
    {
        Shape = ButtonShape.Rectangle;
        Position = relativePosition;
        Position = relativePosition;
    }

    public ButtonComponent(Vector2 relativePosition, float raidus)
    {
        Shape = ButtonShape.Circle;
        Position = relativePosition;
        Radius = raidus;
    }
}