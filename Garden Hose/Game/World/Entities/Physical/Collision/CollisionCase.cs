using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities;


internal record class CollisionCase
{
    // Fields.
    internal PhysicalEntity EntityA { get; init; }

    internal PhysicalEntity EntityB { get; init; }

    internal PhysicalEntityPart PartA { get; init; }

    internal PhysicalEntityPart PartB { get; init; }

    internal ICollisionBound BoundA { get; init; }

    internal ICollisionBound BoundB { get; init; }

    internal Vector2 BoundAPosition => PartA.Position + BoundA.Offset;

    internal Vector2 BoundBPosition => PartB.Position + BoundB.Offset;

    internal Vector2[] CollisionPoints { get; init; }

    internal Vector2 AverageCollisionPoint
    {
        get
        {
            Vector2 CollisionPoint = Vector2.Zero;

            foreach (Vector2 Point in CollisionPoints)
            {
                CollisionPoint += Point;
            }

            return CollisionPoint / CollisionPoints.Length;
        }
    }

    internal Vector2 SurfaceNormal
    {
        get => _surfaceNormal;
        init
        {
            _surfaceNormal = value;
            if (!float.IsFinite(_surfaceNormal.LengthSquared()))
            {
                _surfaceNormal = Vector2.One;
            }
        }
    }


    // Private fields.
    private Vector2 _surfaceNormal;
}