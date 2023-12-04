using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeMainPart : PhysicalEntityPart
{
    // Internal static fields.
    internal static Vector2 HitboxSize = new(30f, 28f);
    internal static Vector2 SpriteScale = new(0.2f);


    // Internal fields.

    // Private fields.


    // Constructors.
    public ProbeMainPart(ProbeEntity entity) : base(WorldMaterial.Test, entity)
    {
        CollisionBounds = new ICollisionBound[] { new RectangleCollisionBound(HitboxSize, Vector2.Zero) };
    }


    // Inherited methods.
    protected override void OnPartBreakOff(Vector2 collisionLocation, float forceApplied)
    {

    }

    protected override void OnPartDamage(Vector2 collisionLocation, float forceApplied)
    {

    }

    protected override void OnPartDestroy(Vector2 collisionLocation, float forceApplied)
    {

    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        Sprites.Add(new(assetManager.GetAnimation("ship_probe_base")!) { Scale = SpriteScale });
    }
}