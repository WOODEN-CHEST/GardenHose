using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Ship.Weapons;

internal abstract class ShipWeapon : PhysicalEntityPart
{
    // Internal fields.
    internal Vector2 TargetLocation { get; set; }

    internal event EventHandler<WeaponFireEventArgs>? WeaponFire;


    // Constructors.
    internal ShipWeapon(ICollisionBound[] collisionBounds, WorldMaterial material, PhysicalEntity? entity) : base(collisionBounds, material, entity)
    {
    }


    // Internal methods.
    internal abstract void Fire();
}