using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Planet.Buildings;

internal class ResidentalBuilding : PlanetBuilding
{
    public ResidentalBuilding(ICollisionBound[]? collisionBounds, WorldMaterial material, PhysicalEntity? entity) : base(collisionBounds, material, entity)
    {
    }
}