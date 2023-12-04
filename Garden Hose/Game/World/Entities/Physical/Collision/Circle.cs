using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Physical.Collision;

internal struct Circle
{
    // Fields.
    internal float RSquared { get; init; }

    internal float XNumber { get; init; }

    internal float YNumber { get; init; }


    // Constructors.
    internal Circle(float radius, Vector2 position)
    {
        RSquared = radius * radius;

        XNumber = -position.X;
        YNumber = -position.Y;
    }


    // Internal static methods.
    internal static Vector2[] GetIntersections(Circle circle, EquationRay ray)
    {
        // Edge case if testing against vertical ray.
        if (float.IsInfinity(ray.YStep))
        {
            float CircleXMin = -circle.XNumber - MathF.Sqrt(circle.RSquared);
            float CircleXMax = -circle.XNumber + MathF.Sqrt(circle.RSquared);

            if (ray.OriginX < CircleXMin || ray.OriginX > CircleXMax)
            {
                return new Vector2[0];
            }
            else if ((ray.OriginX == CircleXMin) || (ray.OriginX == CircleXMax))
            {
                return new Vector2[] { new Vector2(ray.OriginX, -circle.YNumber)};
            }

            float CircleRadius = MathF.Sqrt(circle.RSquared);
            float Distance = (ray.OriginX + circle.XNumber);
            float YValue = MathF.Sqrt(circle.RSquared - (Distance * Distance));

            return new Vector2[]
            {
                new Vector2(ray.OriginX, -circle.YNumber + YValue),
                new Vector2(ray.OriginX, -circle.YNumber - YValue)
            };
        }

        // Prepare values.
        float A, B, C;
        A = 1f + (ray.YStep * ray.YStep);
        B = (2f * circle.XNumber) + (2f * ray.YStep * (ray.YOffset + circle.YNumber));
        C = (circle.XNumber * circle.XNumber) 
            + ((circle.YNumber + ray.YOffset) * (circle.YNumber + ray.YOffset)) - circle.RSquared;

        float TwoA = A * 2f;
        float Discriminant = (B * B) - (4f * A * C);

        // Get roots.
        if (Discriminant < 0)
        {
            return new Vector2[0];
        }
        else if (Discriminant is 0f or -0f)
        {
            float X = -B / TwoA;
            return new Vector2[] { new Vector2(X, ray.GetValueAtX(X)) };
        }

        Discriminant = MathF.Sqrt(Discriminant);

        float X1 = (-B - Discriminant) / TwoA;
        float X2 = (-B + Discriminant) / TwoA;

        return new Vector2[] { new Vector2(X1, ray.GetValueAtX(X1)), new Vector2(X2, ray.GetValueAtX(X2)) };
    
    
    }

    internal static Vector2[] GetIntersections(Circle circleA , Circle circleB)
    {
        // (x + a)^2 + (x + b)^2 - r1^2 - (x + c)^2 - (x + d)^2 + r2^2 = 0
        // Transforms into linear function y = ax + b

        float XMult, YMult, CValue;

        XMult = (2f * circleA.XNumber) - (2f * circleB.XNumber);
        YMult = ((2f * circleA.YNumber) - (2f * circleB.YNumber)) * -1f;
        CValue = -circleA.RSquared + circleB.RSquared
            + (circleA.XNumber * circleA.XNumber) 
            + (circleA.YNumber * circleA.YNumber)
            - (circleB.XNumber * circleB.XNumber)
            -(circleB.YNumber * circleB.YNumber);

        EquationRay IntersectionRay = new(CValue / YMult, XMult / YMult);

        return GetIntersections(circleA, IntersectionRay);
    }


    // Inherited methods.
    public override string ToString()
    {
        return $"(X{(XNumber > 0f ? " +" : null)} {XNumber})^2 + (Y{(YNumber > 0f ? " +" : null)} {YNumber})^2 = {RSquared}";
    }
}