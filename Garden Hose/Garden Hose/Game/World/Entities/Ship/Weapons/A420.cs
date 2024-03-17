using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship.Weapons;

internal class A420 : ShipWeapon
{
    // Internal static fields.
    internal const float BALL_BOUND_RADIUS = 10f;
    internal static Vector2 CANNON_BOUND_SIZE { get; } = new(4.5f, 27.5f);
    internal static Vector2 BULLET_SIZE { get; } = new(4f, 7f);

    internal const float BULLET_SPEED = 240f;
    internal const float TIME_BETWEEN_SHOTS = 0.413265f;
    internal const float SPREAD = 0.035f;


    // Constructors.
    public A420(PhysicalEntity? entity) 
        : base(new ICollisionBound[]
        {
            new BallCollisionBound(BALL_BOUND_RADIUS),
            new RectangleCollisionBound(CANNON_BOUND_SIZE, CANNON_BOUND_SIZE * new Vector2(0f, 1f)),
        }, WorldMaterial.A420Bullet, entity, WeaponType.Light)
    {
        TurretMaxAngle = float.PositiveInfinity;
        TurretTurnSpeed = 2.147f;

        Name = "A420 40/70 Autocannon";
        Description = "The A420 is a simple yet effective autocannon which can spray enemies with deadly bullets. " +
            "It is not too heavy, turns swiftly and is fairly accurate, making it a decent choice for a weapon.";
    }


    // Inherited methods.
    internal override void Trigger(GHGameTime time)
    {
        if ((LastFireTime == null) || (time.WorldTime.TotalTimeSeconds - LastFireTime.Value >= TIME_BETWEEN_SHOTS))
        {
            Fire(time);
        }
    }

    internal override void Fire(GHGameTime time)
    {
        base.Fire(time);

        BasicBulletEntity Bullet = new(ProjectileEntity.DEFAULT_LIFETIME,
            new RectangleCollisionBound(BULLET_SIZE), WorldMaterial.A420Bullet)
        {
            Position = Position + Vector2.Transform(CANNON_BOUND_SIZE 
            * new Vector2(0f, 1f), Matrix.CreateRotationZ(CombinedRotation)),
            Rotation = CombinedRotation,
            Motion = Entity!.Motion + Vector2.TransformNormal(Vector2.UnitY,
            Matrix.CreateRotationZ(GetAngleWithSpread(CombinedRotation, SPREAD))) * BULLET_SPEED
        };
        Bullet.CollisionHandler.AddCollisionIgnorable(Entity!);
        Entity!.World!.AddEntity(Bullet);

        Recoil(Bullet);
        HeatFromFiring((BULLET_SPEED * BULLET_SPEED) * 0.5f);
    }
}