using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Physical.Events;

internal class CollisionEventArgs : EventArgs
{
    // Internal fields.
    internal Vector2 CollisionLocation { get; init; }

    internal float ForceApplied { get; init; }


    // Constructors.
    public CollisionEventArgs(Vector2 collisionLocation, float forceApplied)
    {
        CollisionLocation = collisionLocation;
        ForceApplied = forceApplied;
    }
}