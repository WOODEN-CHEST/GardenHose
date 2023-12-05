using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeSideThrusterPart : PhysicalEntityPart, ThrusterPart
{
    // Internal static fields.
    internal static Vector2 HitboxSize { get; } = new(11f, 17f);


    // Internal fields.
    internal Origin Side { get; set; }

    public bool IsThrusterOn { get; set; } = true;

    public float TargetThrusterThrottle
    {
        get => _targetThrusterThrottle;
        set => _targetThrusterThrottle = Math.Clamp(value, 0f, 1f); 
    }

    public float CurrentThrusterThrottle { get; private set; }

    public float ThrusterThrottleChangeSpeed => 5.3f;

    public float ThrusterPower => 7993f;


    // Private fields.
    private float _targetThrusterThrottle = 0f;


    // Constructors.
    public ProbeSideThrusterPart(ProbeEntity entity, Origin side) : base(WorldMaterial.Test, entity)
    {
        if (side is not Origin.CenterLeft and not Origin.CenterRight)
        {
            throw new ArgumentException($"Side must be center left or center right, got {side}", nameof(side));
        }

        Side = side;
        CollisionBounds = new ICollisionBound[] { new RectangleCollisionBound(HitboxSize, Vector2.Zero) };
        TargetThrusterThrottle = 1f;
    }


    // Inherited methods.

    // Private methods.
    [TickedFunction(false)]
    private void ThrusterTick()
    {
        float Step = MathF.Sign(TargetThrusterThrottle - CurrentThrusterThrottle);
        float ThrottleChange = Step * ThrusterThrottleChangeSpeed * Entity.World!.PassedTimeSeconds;
        if (Math.Abs(ThrottleChange) > Math.Abs(TargetThrusterThrottle - CurrentThrusterThrottle))
        {
            ThrottleChange = TargetThrusterThrottle - CurrentThrusterThrottle;
        }
        CurrentThrusterThrottle += ThrottleChange;



        if (CurrentThrusterThrottle != 0f)
        {
            Vector2 ForceDirection = Vector2.TransformNormal(-Vector2.UnitY, Matrix.CreateRotationZ(CombinedRotation));
            Entity.ApplyForce(ForceDirection * CurrentThrusterThrottle * ThrusterPower * Entity.World!.PassedTimeSeconds, Position);
        }
        
    }

    internal override void Load(GHGameAssetManager assetManager)
    {
        PhysicalEntityPartSprite Sprite = new(assetManager.GetAnimation("ship_probe_sidethruster")!);
        Sprite.Sprite.Effects = Side == Origin.CenterRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Sprite.Scale = ProbeMainPart.SpriteScale;

        Sprites.Add(Sprite);
    }

    internal override void ParallelTick()
    {
        base.ParallelTick();

        if (IsThrusterOn)
        {
            ThrusterTick();
        }
    }
}