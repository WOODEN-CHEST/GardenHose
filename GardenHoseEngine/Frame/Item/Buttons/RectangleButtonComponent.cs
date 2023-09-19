using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Buttons;

public struct RectangleButtonComponent : IButtonComponent
{
    // Fields.
    public Vector2 Dimensions { get; set; } = Vector2.Zero;

    public Vector2 Offset { get; set; } = Vector2.Zero; 

    public Vector2 Size { get; set; } = Vector2.One;


    // Constructors.
    public RectangleButtonComponent(Vector2 offset, Vector2 dimensions)
    {
        Offset = offset;
        Dimensions = dimensions;
    }


    // Inherited methods.
    public bool IsLocationOverButton(Vector2 locationToTest, Vector2 origin)
    {
        origin += Offset;
        Vector2 endPosition = origin + Dimensions;

        return (origin.X <= locationToTest.X) && (locationToTest.X <= endPosition.X * Size.X)
            && (origin.Y <= locationToTest.Y) && (locationToTest.Y <= endPosition.Y * Size.Y);
    }
}