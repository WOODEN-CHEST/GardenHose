using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace GardenHoseEngine.Extensions;

public static class ExtVector2
{
    public static void Rotate(this ref Vector2 vector, float angle)
    {
        vector.X = (vector.X * MathF.Cos(angle)) - (vector.Y * MathF.Sin(angle));
        vector.Y = (vector.X * MathF.Sin(angle)) + (vector.Y * MathF.Cos(angle));
    }
}