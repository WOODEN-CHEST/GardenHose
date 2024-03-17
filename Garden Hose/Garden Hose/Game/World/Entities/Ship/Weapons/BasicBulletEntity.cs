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

internal class BasicBulletEntity : ProjectileEntity
{
    // Constructors.
    internal BasicBulletEntity(float timeLeft, RectangleCollisionBound rectBound, WorldMaterial material) : base(timeLeft)
    {
        MainPart = new PhysicalEntityPart(new ICollisionBound[] { rectBound }, material, this);
    }

    internal BasicBulletEntity(float timeLeft, BallCollisionBound ballBound, WorldMaterial material) : base(timeLeft)
    {
        MainPart = new PhysicalEntityPart(new ICollisionBound[] { ballBound }, material, this);
    }
}