using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using GardenHoseEngine.Frame.Item;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeHeadPart : PhysicalEntityPart
{
    // Internal static fields.
    internal static Vector2 HitboxSize = new(ProbeMainPart.HitboxSize.X, 15f);


    // Private fields.


    // Constructors.
    public ProbeHeadPart(ProbeEntity entity) : base(WorldMaterial.Test, entity)
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
        Sprites.Add(new(assetManager.GetAnimation("ship_probe_head")!) { Scale = ProbeMainPart.SpriteScale });
    }
}