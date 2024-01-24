using Microsoft.Xna.Framework;
using System;


namespace GardenHose.Game.World.Entities.Physical.Collision;


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

    internal Vector2 EntityAMotion { get; init; }

    internal Vector2 EntityBMotion { get; init; }

    internal Vector2 EntityARotationalMotionAtPoint { get; init; }

    internal Vector2 EntityBRotationalMotionAtPoint { get; init; }

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

    internal Vector2 SurfaceNormal { get; init; }


    // Constructors.
    internal CollisionCase(PhysicalEntity entityA,
        PhysicalEntity entityB,
        PhysicalEntityPart partA,
        PhysicalEntityPart partB,
        ICollisionBound boundA,
        ICollisionBound boundB,
        Vector2 surfaceNormal,
        Vector2[] collisionPoints)
    {
        EntityA = entityA ?? throw new ArgumentNullException(nameof(entityA));
        EntityB = entityB ?? throw new ArgumentNullException(nameof(entityB));
        PartA = partA ?? throw new ArgumentNullException(nameof(partA));
        PartB = partB ?? throw new ArgumentNullException(nameof(partB));
        BoundA = boundA;
        BoundB = boundB;

        SurfaceNormal = surfaceNormal;
        if (!float.IsFinite(SurfaceNormal.LengthSquared()))
        {
            SurfaceNormal = Vector2.One;
        }

        CollisionPoints = collisionPoints ?? throw new ArgumentNullException(nameof(collisionPoints));

        EntityAMotion = EntityA.Motion;
        EntityBMotion = EntityB.Motion;

        EntityARotationalMotionAtPoint = EntityA.GetAngularMotionAtPoint(AverageCollisionPoint);
        EntityBRotationalMotionAtPoint = EntityB.GetAngularMotionAtPoint(AverageCollisionPoint);
    }
}