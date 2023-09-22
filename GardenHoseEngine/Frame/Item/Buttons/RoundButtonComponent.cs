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
    public float Radius
    {
        get => _radius;
        set
        {
            _radius = Math.Clamp(value, 0, float.MaxValue);
        }
    }


    // Private fields.
    private float _radius;


    // Constructors.
    public RoundButtonComponent(float radius)
    {
        Radius = radius;
    }


    // Inherited methods.
    public bool IsLocationOverButton(Vector2 locationToTest, Vector2 origin, Vector2 scale)
    {
        return Vector2.Distance(origin, locationToTest) <= _radius * scale.X;
    }
}