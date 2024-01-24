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
    public readonly float Time;
    public readonly InterpolationMethod InterpMethod;


    // Internal fields.
    internal float TimeToNext;


    // Constructors,
    public AnimVector2Keyframe(Vector2 location, float time, InterpolationMethod interpMethod)
    {
        if (float.IsNegative(time))
        {
            throw new ArgumentException($"Keyframe time cannot be negative: {time}");
        }
        if (!float.IsFinite(time))
        {
            throw new ArgumentException($"Invalid keyframe time: {time}");
        }

        Location = location;
        Time = time;
        InterpMethod = interpMethod;
    }
}