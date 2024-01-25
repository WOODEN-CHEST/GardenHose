using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Frame.Item;

public class PropertyKeyframe
{
    // Fields.
    public Vector2 Position { get; init; }
    public Vector2 Size { get; init; }
    public float Rotation { get; init; }
    public float Time { get; init; }
    public InterpolationMethod Interpolation { get; init; }


    // Constructors,
    public PropertyKeyframe(Vector2 location, Vector2 size, float rotation, float time, InterpolationMethod interpolation)
    {
        if (float.IsNegative(time))
        {
            throw new ArgumentException($"Keyframe time cannot be negative: {time}");
        }

        Position = location;
        Time = time;
        Interpolation = interpolation;
    }
}