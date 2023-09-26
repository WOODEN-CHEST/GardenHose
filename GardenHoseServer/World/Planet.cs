using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World;

internal class Planet
{
    // Fields.
    internal float Radius { get; private init; }

    internal float Attraction { get; private init; }


    // Constructors.
    internal Planet(float radius, float attraction)
    {
        if (radius <= 0 || !float.IsFinite(radius))
        {
            throw new ArgumentException($"Invalid planet radius: {radius}", nameof(radius));
        }
        if (!float.IsFinite(attraction))
        {
            throw new ArgumentException($"Invalid planet attraction: {attraction}", nameof(attraction));
        }

        Radius = radius;
        Attraction = attraction;
    }
}