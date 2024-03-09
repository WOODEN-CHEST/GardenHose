using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship.Weapons;

internal class ProjectileCollisionHandler : EntityCollisionHandler
{
    // Internal static fields.
    internal const float TIME_LEFT_AFTER_HIT = 1.5f;


    // Constructors.
    internal ProjectileCollisionHandler(PhysicalEntity entity) : base(entity) { }


    // Inherited methods.
    internal override void OnCollision(CollisionCase collisionCase, GHGameTime time)
    {
        base.OnCollision(collisionCase, time);

        ProjectileEntity Projectile = (ProjectileEntity)collisionCase.SelfEntity;
        Projectile.IsCollided = true;
        if (Projectile.TimeLeft > TIME_LEFT_AFTER_HIT)
        {
            Projectile.TimeLeft = TIME_LEFT_AFTER_HIT;
        }
    }
}