using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Particle;

internal class ParticleCollisionHandler : EntityCollisionHandler
{
    // Internal fields.
    internal bool IsKilledByPlanets { get; set; } = true;


    // Constructors.
    internal ParticleCollisionHandler(PhysicalEntity entity) : base(entity) { }


    // Inherited methods.
    internal override void OnCollision(CollisionCase collisionCase, GHGameTime time)
    {
        ParticleEntity ParentEntity = (ParticleEntity)Entity;

        if (IsKilledByPlanets && (collisionCase.TargetEntity.EntityType == EntityType.Planet)
            && (ParentEntity.TimeAlive < ParentEntity.Lifetime))
        {
            ParentEntity.TimeAlive = ParentEntity.Lifetime;
        }

        base.OnCollision(collisionCase, time);
    }
}