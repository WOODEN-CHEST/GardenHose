using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Physical.Events;

internal class PartDamageEventArgs : CollisionEventArgs
{
    // Internal fields.
    internal PhysicalEntityPart Part { get; init; }

    public PartDamageEventArgs(PhysicalEntityPart part, Vector2 collisionLocation, float forceApplied) 
        : base(collisionLocation, forceApplied)
    {
        Part = part ?? throw new ArgumentNullException(nameof(part));
    }
}
