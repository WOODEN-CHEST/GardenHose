using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Physical;

internal record class CollisionCase
{
    internal PhysicalEntity EntityA { get; set; }

    internal PhysicalEntity EntityB { get; set; }

    internal PhysicalEntityPart PartA { get; set; }

    internal PhysicalEntityPart PartB { get; set; }

    internal ICollisionBound BoundA { get; set; }

    internal ICollisionBound BoundB { get; set; }

    internal Vector2[] CollisionPoints { get; set; }

    internal Vector2 SurfaceNormal { get; set; }
}