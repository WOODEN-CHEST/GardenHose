using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using System.Text;
using System.Threading.Tasks;

namespace GardenHoseServer.World.Entities;

internal abstract class PhysicalEntity : Entity
{
    // Fields.
    internal Vector2 Position { get; set; }

    internal Vector2 Motion { get; set; }

    internal float Rotation { get; set; }

    internal float AngularMotion { get; set; }


    // Constructors.
    internal PhysicalEntity(EntityType type, GameWorld world) : base(type, world) { }


    // Methods.
    internal override void Tick(float passedTimeSeconds)
    {
        World.PhysicsEngine.Simulate(this, passedTimeSeconds);
    }
}