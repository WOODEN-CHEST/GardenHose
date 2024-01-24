using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeEntity : SpaceshipEntity
{
    // Internal fields.
    internal override ISpaceshipSystem ShipSystem { get; init; }

    internal const float SPRITE_SCALING = 0.217f;

    internal PhysicalEntityPart? HeadPart { get; private set; }

    internal ThrusterPart? LeftThrusterPart { get; private set; }

    internal ThrusterPart? RightThrusterPart { get; private set; }

    internal ThrusterPart? MainThrusterPart { get; private set; }


    //  Private static fields.
    /* Hit-box */
    private static Vector2 s_bodyHitboxSize = new(30f, 28f);
    private static Vector2 s_headHitboxSize = new(s_bodyHitboxSize.X, 15f);
    private static Vector2 s_sideThrusterHitboxSize = new(11f, 17f);
    private static Vector2 s_mainThrusterHitboxSize = new(s_bodyHitboxSize.X - 1f, 20f);

    /* Sprite. */
    private static Vector2 SpriteScale = new(0.2f);


    // Private fields.
    /* Parts. */
    

    /* System. */
    private ProbeSystem _system;



    // Constructors.
    public ProbeEntity() : base(EntityType.Probe, null)
    {
        _system = new ProbeSystem(this);
        ShipSystem = _system;

        PhysicalEntityPart BasePart = CreateBodyPart(this);

        HeadPart = CreateHeadPart(this);
        LeftThrusterPart = CreateSideThrusterPart(this, false);
        RightThrusterPart = CreateSideThrusterPart(this, true);
        MainThrusterPart = CreateMainThrusterPart(this);

        // A lot of magic numbers here are just offsets which were not calculated but just eyed until it looked right.
        BasePart.LinkPart(HeadPart, new(0f, (-s_bodyHitboxSize.Y * 0.5f) - (s_headHitboxSize.Y * 0.5f) + 1.25f), 30_000f);
        BasePart.LinkPart(RightThrusterPart, new(s_bodyHitboxSize.X * 0.5f + s_sideThrusterHitboxSize.X * 0.5f - 2.5f, 0f), 30_000);
        BasePart.LinkPart(LeftThrusterPart, -RightThrusterPart.ParentLink!.LinkDistance, 30_000f);
        BasePart.LinkPart(MainThrusterPart, new(0f, s_bodyHitboxSize.Y * 0.5f + s_mainThrusterHitboxSize.X * 0.5f - 13.5f), 30_000);

        MainPart = BasePart;
        IsInvulnerable = false;
        Pilot = SpaceshipPilot.Player;
    }

    

    protected override void AIParallelTick()
    {

    }

    protected override void AISequentialTick()
    {

    }

    protected override void PlayerParallelTick()
    {

    }

    protected override void PlayerSequentialTick()
    {

    }


    // Private static methods.
    private static PhysicalEntityPart CreateBodyPart(PhysicalEntity entity)
    {
        PhysicalEntityPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_bodyHitboxSize)},
            WorldMaterial.Test,
            entity);
        Part.AddSprite(new("ship_probe_base") { Scale = SpriteScale });

        return Part;
    }

    private static PhysicalEntityPart CreateHeadPart(PhysicalEntity entity)
    {
        PhysicalEntityPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_headHitboxSize) },
            WorldMaterial.Test,
            entity);
        Part.AddSprite(new("ship_probe_head") { Scale = SpriteScale });

        return Part;
    }

    private static ThrusterPart CreateSideThrusterPart(PhysicalEntity entity, bool isRightPart)
    {
        const float THRUSTER_POWER = 7993f;
        const float THRUSTER_FUEL = 10_000_000f;
        const float THRUSTER_FUEL_EFFICIENCY = 3.16f;
        const float THRUSTER_CHANGE_SPEED = 9.8f;

        ThrusterPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_sideThrusterHitboxSize) },
            WorldMaterial.Test, entity)
        {
            ThrusterPower = THRUSTER_POWER,
            ThrusterThrottleChangeSpeed = THRUSTER_CHANGE_SPEED,
            MaxFuel = THRUSTER_FUEL,
            Fuel = THRUSTER_FUEL,
            FuelEfficiency = THRUSTER_FUEL_EFFICIENCY,
            ForceDirection = (MathF.PI / -2f)
        };

        Part.AddSprite(new("ship_probe_sidethruster") 
        { 
            Scale = SpriteScale, 
            Effects = isRightPart ? SpriteEffects.None : SpriteEffects.FlipHorizontally
        });

        return Part;
    }

    private static ThrusterPart CreateMainThrusterPart(PhysicalEntity entity)
    {
        const float THRUSTER_POWER = 18412;
        const float THRUSTER_FUEL = 20_000_000f;
        const float THRUSTER_FUEL_EFFICIENCY = 2.51f;
        const float THRUSTER_CHANGE_SPEED = 5.8f;

        ThrusterPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_mainThrusterHitboxSize) },
            WorldMaterial.Test, entity)
        {
            ThrusterPower = THRUSTER_POWER,
            ThrusterThrottleChangeSpeed = THRUSTER_CHANGE_SPEED,
            MaxFuel = THRUSTER_FUEL,
            Fuel = THRUSTER_FUEL,
            FuelEfficiency = THRUSTER_FUEL_EFFICIENCY,
            ForceDirection = (MathF.PI / -2f)
        };

        Part.AddSprite(new("ship_probe_mainthruster") {Scale = SpriteScale });

        return Part;
    }


    // Inherited methods.
    internal override void SequentialTick()
    {
        base.SequentialTick();
        _system.SequentialTick(Pilot == SpaceshipPilot.Player);
    }
    internal override void ParallelTick()
    {
        base.ParallelTick();
        _system.ParallelTick(Pilot == SpaceshipPilot.Player);
    }
}