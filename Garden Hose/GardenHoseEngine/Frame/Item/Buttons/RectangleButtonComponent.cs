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
    public Vector2 Dimensions
    {
        get => _dimensions;
        set
        {
            _dimensions = value;
            _offset = -(_dimensions / 2f);
        }
    }


    // Private fields.
    private Vector2 _dimensions;
    private Vector2 _offset = Vector2.Zero;


    // Constructors.
    public RectangleButtonComponent(Vector2 dimensions)
    {
        Dimensions = dimensions;
    }


    // Inherited methods.
    public bool IsLocationOverButton(Vector2 locationToTest, Vector2 origin, Vector2 scale)
    {
        origin += _offset * scale;
        Vector2 endPosition = origin + (Dimensions * scale);

        return (origin.X <= locationToTest.X) && (locationToTest.X <= endPosition.X)
            && (origin.Y <= locationToTest.Y) && (locationToTest.Y <= endPosition.Y);
    }
}