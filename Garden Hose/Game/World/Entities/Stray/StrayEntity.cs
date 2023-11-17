using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Material;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities;

internal class StrayEntity : PhysicalEntity
{
    // Constructors.
    public StrayEntity(GameWorld? world) : base(EntityType.Stray, world)
    {

    }

    // Inherited methods.
    internal override void Delete() { }

    internal override void Load(GHGameAssetManager assetManager) { }
}