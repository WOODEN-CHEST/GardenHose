using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeMainPart : PhysicalEntityPart
{
    // Internal static fields.
    


    // Internal fields.

    // Private fields.


    // Constructors.
    public ProbeMainPart(ProbeEntity entity) : base(WorldMaterial.Test, entity)
    {
        CollisionBounds = new ICollisionBound[] { new RectangleCollisionBound(HitboxSize, Vector2.Zero) };
    }


    // Inherited methods.
    internal override void Load(GHGameAssetManager assetManager)
    {
        Sprites.Add(new(assetManager.GetAnimation("ship_probe_base")!) { Scale = SpriteScale });
    }
}