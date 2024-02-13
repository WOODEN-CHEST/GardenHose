using Microsoft.Xna.Framework;
using System;
using System.Linq;

namespace GardenHose.Game.World.Entities.Physical.Collision;


internal record class CollisionCase
{
    // Fields.
    internal PhysicalEntity SelfEntity { get; init; }

    internal PhysicalEntity TargetEntity { get; init; }

    internal PhysicalEntityPart SelfPart { get; init; }

    internal PhysicalEntityPart TargetPart { get; init; }

    internal ICollisionBound SelfBound { get; init; }

    internal ICollisionBound TargetBound { get; init; }

    internal Vector2 SelfBoundPosition => SelfPart.Position + SelfBound.Offset;

    internal Vector2 TargetBoundPosition => TargetPart.Position + TargetBound.Offset;

    internal Vector2 SelfMotion { get; init; }

    internal Vector2 TargetMotion { get; init; }

    internal Vector2 SelfRotationalMotionAtPoint { get; init; }

    internal Vector2 TargetRotationalMotionAtPoint { get; init; }

    internal Vector2[] CollisionPoints { get; init; }

    internal Vector2 AverageCollisionPoint { get; private init; }

    internal Vector2 SurfaceNormal { get; init; }
    internal Vector2 InverseSurfaceNormal { get; init; }


    // Constructors.
    internal CollisionCase(PhysicalEntity selfEntity,
        PhysicalEntity targetEntity,
        PhysicalEntityPart selfPart,
        PhysicalEntityPart targetPart,
        ICollisionBound selfBound,
        ICollisionBound targetBound,
        Vector2 surfaceNormal,
        Vector2 inverseSurfaceNormal,
        Vector2[] collisionPoints)
    {
        SelfEntity = selfEntity ?? throw new ArgumentNullException(nameof(selfEntity));
        TargetEntity = targetEntity ?? throw new ArgumentNullException(nameof(targetEntity));
        SelfPart = selfPart ?? throw new ArgumentNullException(nameof(selfPart));
        TargetPart = targetPart ?? throw new ArgumentNullException(nameof(targetPart));
        SelfBound = selfBound;
        TargetBound = targetBound;

        SurfaceNormal = surfaceNormal;
        InverseSurfaceNormal = inverseSurfaceNormal;

        CollisionPoints = collisionPoints ?? throw new ArgumentNullException(nameof(collisionPoints));
        Vector2 CollisionPoint = Vector2.Zero;
        foreach (Vector2 Point in CollisionPoints)
        {
            CollisionPoint += Point;
        }
        AverageCollisionPoint = CollisionPoint / CollisionPoints.Length;

        SelfMotion = SelfEntity.Motion;
        TargetMotion = TargetEntity.Motion;

        SelfRotationalMotionAtPoint = SelfEntity.GetAngularMotionAtPoint(AverageCollisionPoint);
        TargetRotationalMotionAtPoint = TargetEntity.GetAngularMotionAtPoint(AverageCollisionPoint);
    }


    // Internal methods.
    internal CollisionCase GetInvertedCase()
    {
        return new CollisionCase(
            TargetEntity,
            SelfEntity,
            TargetPart,
            SelfPart,
            TargetBound,
            SelfBound,
            InverseSurfaceNormal,
            SurfaceNormal,
            CollisionPoints.ToArray());
    }
}