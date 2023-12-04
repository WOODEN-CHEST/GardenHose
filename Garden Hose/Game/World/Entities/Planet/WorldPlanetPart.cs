using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Planet;

internal class WorldPlanetPart : PhysicalEntityPart
{
    // Constructors.
    internal WorldPlanetPart(float radius,
        WorldMaterial material,
        PhysicalEntity entity) : base(new ICollisionBound[] { new BallCollisionBound(radius) }, material, entity) { }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void SequentialTick()
    {
        base.SequentialTick();
    }

    [TickedFunction(false)]
    internal override void ParallelTick()
    {

    }

    protected override void OnPartDestroy(Vector2 collisionLocation, float forceAmount) { }

    protected override void OnPartDamage(Vector2 collisionLocation, float forceAmount) { }

    protected override void OnPartBreakOff(Vector2 collisionLocation, float forceAmount) { }

    internal override void Load(GHGameAssetManager assetManager)
    {
        
    }

    internal override void Draw()
    {

    }
}