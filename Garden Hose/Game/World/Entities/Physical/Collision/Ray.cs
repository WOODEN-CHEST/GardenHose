using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal struct Ray
{
    // Fields.
    internal float YOffset { get; private set; }

    internal float YStep { get; private set; }

    internal float OriginX { get; private set; } // Cheap band-aid fix for vertical ray cases where y=∞x+.


    // Constructors.
    internal Ray(float xOffset, float xStep) : this(xOffset, xStep, 0f) { }

    internal Ray(float xOffset, float xStep, float originX)
    {
        YOffset = xOffset;
        YStep = xStep;
        OriginX = originX;
    }

    internal Ray(Vector2 lineStart, Vector2 lineEnd)
    {
        OriginX = lineStart.X;

        float DeltaX = lineEnd.X - lineStart.X;
        float DeltaY = lineEnd.Y - lineStart.Y;

        YStep = DeltaY / DeltaX;

        YOffset = lineStart.Y + (-lineStart.X) * YStep;
    }

    internal Ray(Edge edge) : this(edge.StartVertex, edge.EndVertex) { }


    // Internal static methods.
    internal static Vector2? GetIntersection(Ray ray1, Ray ray2)
    {
        if (ray1 == ray2)
        {
            return new Vector2(0, ray1.YOffset);
        }
        if (ray1.YStep == ray2.YStep)
        {
            return null;
        }

        if (float.IsInfinity(ray1.YStep))
        {
            return new Vector2(ray1.OriginX, ray2.YOffset + ray2.YStep * ray1.OriginX);
        }
        else if (float.IsInfinity(ray2.YStep))
        {
            return new Vector2(ray2.OriginX, ray1.YOffset + ray1.YStep * ray2.OriginX);
        }


        float XMultiplier = ray1.YStep;
        XMultiplier -= ray2.YStep;
        float XValue = ray2.YOffset;
        XValue -= ray1.YOffset;
        XValue /= XMultiplier;

        float YValue = ray1.YStep * XValue + ray1.YOffset;

        return new Vector2(XValue, YValue);
    }


    // Internal methods.
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal float GetValueAtX(float x) => YStep * x + YOffset;


    // Inherited methods.
    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        if (obj is not Ray)
        {
            return false;
        }

        return this == (Ray)obj;
    }

    public override string ToString()
    {
        return $"{YStep}X {(YOffset >= 0 ? '+' : '-')} {Math.Abs(YOffset)}";
    }


    // Operators.
    public static bool operator ==(Ray ray1, Ray ray2)
    {
        return (ray1.YStep == ray2.YStep)  && (ray1.YOffset == ray2.YOffset);
    }

    public static bool operator !=(Ray ray1, Ray ray2)
    {
        return (ray1.YStep != ray2.YStep) || (ray1.YOffset != ray2.YOffset);
    }
}