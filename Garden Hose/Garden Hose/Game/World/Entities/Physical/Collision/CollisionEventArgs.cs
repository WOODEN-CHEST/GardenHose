using System;

namespace GardenHose.Game.World.Entities.Physical.Collision;

internal class CollisionEventArgs : EventArgs
{
    // Internal fields.
    internal CollisionCase Case { get; private init; }
    internal float ForceApplied { get; private init; }


    // Constructors.
    public CollisionEventArgs(CollisionCase collisionCase, float forceApplied)
    {
        Case = collisionCase ?? throw new ArgumentNullException(nameof(collisionCase));
        ForceApplied = forceApplied;
    }
}