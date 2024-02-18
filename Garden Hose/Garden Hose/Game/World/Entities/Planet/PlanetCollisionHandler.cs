using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;

namespace GardenHose.Game.World.Entities.Planet;

internal class PlanetCollisionHandler : EntityCollisionHandler
{
    // Constructors.
    internal PlanetCollisionHandler(PhysicalEntity entity) : base(entity) { }


    // Inherited methods.
    internal override void OnCollision(CollisionCase collisionCase, GHGameTime time)
    {
        if (collisionCase.SelfPart == Entity.MainPart)
        {
            return;
        }

        base.OnCollision(collisionCase, time);
    }
}