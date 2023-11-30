using GardenHose.Game.World.Material;

namespace GardenHose.Game.World.Entities;

internal class WorldPlanetPart : PhysicalEntityPart
{
    // Constructors.
    internal WorldPlanetPart(float radius,
        WorldMaterial material,
        PhysicalEntity entity) : base(new ICollisionBound[] { new BallCollisionBound(radius) }, material, entity) { }


    // Inherited methods.
    internal override void SequentialTick()
    {
        base.SequentialTick();
    }

    internal override void ParallelTick() { }

    protected override void OnPartDestroy() { }

    protected override void OnPartDamage() { }

    protected override void OnPartBreakOff() { }
}