using GardenHose.Game.World.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal class PlanetPart : PhysicalEntityPart
{
    // Constructors.
    internal PlanetPart(float radius, 
        WorldMaterial material, 
        PhysicalEntity entity) : base(new ICollisionBound[] { new BallCollisionBound(radius) }, material, entity)

    {
        MaterialInstance.Temperature = 900f;
        IsInvulnerable = true;
    }


    // Inherited methods.
    internal override void SequentialTick() { }

    internal override void ParallelTick() { }

    protected override void OnPartDestroy() { }

    protected override void OnPartDamage() { }

    protected override void OnPartBreakOff() { }
}