using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Ship.Weapons;

internal abstract class ShipWeapon : PhysicalEntityPart
{
    // Internal fields.
    internal WeaponType Type { get; set; }
    internal Vector2 AimLocation { get; set; }
    internal float? LastFireTime { get; set; } = null;
    internal float TurretTurnSpeed { get; set; } = 1f;
    internal float TurretMaxAngle { get; set; } = float.PositiveInfinity;

    internal event EventHandler<WeaponFireEventArgs>? WeaponFire;


    // Constructors.
    internal ShipWeapon(ICollisionBound[] collisionBounds, WorldMaterial material, PhysicalEntity? entity, WeaponType type)
        : base(collisionBounds, material, entity)
    {
        Type = type;
    }


    // Internal methods.
    internal virtual void Fire(GHGameTime time)
    {
        LastFireTime = time.WorldTime.TotalTimeSeconds;
    }

    internal abstract void Trigger();


    // Protected methods.
    protected virtual void Recoil(ProjectileEntity projectile)
    {
        Entity?.ApplyForce(Position, -GHMath.NormalizeOrDefault(projectile.Motion) * (projectile.Motion.Length() * projectile.Mass));
    }


    // Inherited methods.
    internal override void Tick(GHGameTime time)
    {
        Vector2 Offset = AimLocation - Position;
        float TargetRotation = Math.Clamp(MathF.Atan2(-Offset.X, Offset.Y), -TurretMaxAngle, TurretMaxAngle);

        float OffsetFromTargetRotation = MathF.Asin(MathF.Sin(TargetRotation - CombinedRotation));

        float TargetMovement = TurretTurnSpeed * time.WorldTime.PassedTimeSeconds * OffsetFromTargetRotation;
        if (Math.Abs(TargetMovement) > Math.Abs(OffsetFromTargetRotation))
        {
            TargetMovement = OffsetFromTargetRotation;
        }
        SelfRotation += TargetMovement;

        base.Tick(time);
    }
}