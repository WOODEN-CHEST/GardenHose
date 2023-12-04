using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeMainThrusterPart : PhysicalEntityPart, IThrusterPart
{
    // Fields.
    public bool IsThrusterOn { get; set; } = true;

    public float TargetThrusterThrottle
    {
        get => _targetThrusterThrottle;
        set => _targetThrusterThrottle = Math.Clamp(value, 0f, 1f);
    }

    public float CurrentThrusterThrottle { get; private set; }

    public float ThrusterThrottleChangeSpeed => 1.856f;

    public float ThrusterPower => 17_342f;


    // Private fields.
    private float _targetThrusterThrottle = 0f;  


    // Internal static fields.
    internal static Vector2 HitboxSize { get; } = new(ProbeMainPart.HitboxSize.X + -1f, 20f);
    


    // Constructors.
    public ProbeMainThrusterPart(ProbeEntity entity) : base(WorldMaterial.Test, entity)
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
        Sprites.Add(new(assetManager.GetAnimation("ship_probe_mainthruster")!) { Scale = ProbeMainPart.SpriteScale });
    }
}