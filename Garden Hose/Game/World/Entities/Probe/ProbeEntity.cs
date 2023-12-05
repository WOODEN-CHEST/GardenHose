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
    private PhysicalEntityPart? _headPart;
    private ThrusterPart? _leftThrusterPart;
    private ThrusterPart? _rightThrusterPart;
    private ThrusterPart? _mainThrusterPart;

    /* System. */
    private ProbeSystem _system;



    // Constructors.
    public ProbeEntity() : base(EntityType.Probe, null)
    {
        _system = new ProbeSystem(this);
        ShipSystem = _system;

        PhysicalEntityPart BasePart = CreateBodyPart(this);

        _headPart = CreateHeadPart(this);
        _leftThrusterPart = CreateSideThrusterPart(this, false);
        _rightThrusterPart = CreateSideThrusterPart(this, true);
        _mainThrusterPart = CreateMainThrusterPart(this);

        // A lot of magic numbers here are just offsets which were not calculated but just eyed until it looked right.
        BasePart.LinkPart(_headPart, new(0f, (-s_bodyHitboxSize.Y * 0.5f) - (s_headHitboxSize.Y * 0.5f) + 1.25f), 10_000f);
        BasePart.LinkPart(_rightThrusterPart, new(s_bodyHitboxSize.X * 0.5f + s_sideThrusterHitboxSize.X * 0.5f - 2.5f, 0f), 10_000f);
        BasePart.LinkPart(_leftThrusterPart, -_rightThrusterPart.ParentLink!.LinkDistance, 10_000f);
        BasePart.LinkPart(_mainThrusterPart, new(0f, s_bodyHitboxSize.Y * 0.5f + s_mainThrusterHitboxSize.X * 0.5f - 13.5f), 10_000f);

        MainPart = BasePart;
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
        const float THRUSTER_CHANGE_SPEED = 7993f;

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
        const float THRUSTER_POWER = 7993f;
        const float THRUSTER_FUEL = 20_000_000f;
        const float THRUSTER_FUEL_EFFICIENCY = 2.51f;
        const float THRUSTER_CHANGE_SPEED = 17342;

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
}