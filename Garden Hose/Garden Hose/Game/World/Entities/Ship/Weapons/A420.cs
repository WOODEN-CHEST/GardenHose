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

internal class A420 : ShipWeapon
{
    // Internal static fields.
    internal const float BALL_BOUND_RADIUS = 10f;
    internal static Vector2 CANNON_BOUND_SIZE { get; } = new(4.5f, 27.5f);


    // Constructors.
    public A420(PhysicalEntity? entity, WeaponType type) 
        : base(new ICollisionBound[]
        {
            new BallCollisionBound(BALL_BOUND_RADIUS),
            new RectangleCollisionBound(CANNON_BOUND_SIZE, CANNON_BOUND_SIZE * new Vector2(0f, -1f)),
        }, WorldMaterial.A420Bullet, entity, type)
    {

    }


    // Inherited methods.
    internal override void Trigger()
    {
        
    }
}