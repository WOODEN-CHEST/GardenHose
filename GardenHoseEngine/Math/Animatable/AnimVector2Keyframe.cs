using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine;

public struct AnimVector2Keyframe
{
    // Fields.
    public readonly Vector2 Location;
    public readonly double Time;
    public readonly InterpolationMethod InterpMethod;


    // Internal fields.
    internal double TimeToNext;


    // Constructors,
    public AnimVector2Keyframe(Vector2 location, double time, InterpolationMethod interpMethod)
    {
        if (double.IsNegative(time))
        {
            throw new ArgumentException($"Keyframe time cannot be negative: {time}");
        }
        if (!double.IsFinite(time))
        {
            throw new ArgumentException($"Invalid keyframe time: {time}");
        }

        Location = location;
        Time = time;
        InterpMethod = interpMethod;
    }
}