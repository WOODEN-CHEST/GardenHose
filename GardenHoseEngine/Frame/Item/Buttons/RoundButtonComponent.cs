using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseEngine.Frame.Item.Buttons;

public struct RoundButtonComponent : IButtonComponent
{
    // Fields.

    public Vector2 Offset { get; set; } = Vector2.Zero;

    public float Radius
    {
        get => _radius;
        set
        {
            _radius = Math.Clamp(value, 0, float.MaxValue);
        }
    }

    public Vector2 Size { get; set; } = Vector2.One;


    // Private fields.
    private float _radius;


    // Constructors.
    public RoundButtonComponent(Vector2 offset, float radius)
    {
        Offset = offset;
        Radius = radius;
    }


    // Inherited methods.
    public bool IsLocationOverButton(Vector2 locationToTest, Vector2 origin)
    {
        return Vector2.Distance(origin + Offset, locationToTest) <= _radius * Size.X;
    }
}