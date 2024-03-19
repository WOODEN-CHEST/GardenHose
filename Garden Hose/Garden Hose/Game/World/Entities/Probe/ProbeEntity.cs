using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.Weapons;
using GardenHose.Game.World.Material;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeEntity : SpaceshipEntity
{
    // Internal fields.
    internal override ISpaceshipSystem ShipSystem { get; set; }

    internal PhysicalEntityPart? HeadPart { get; private set; }
    internal ThrusterPart? LeftThrusterPart { get; private set; }
    internal ThrusterPart? RightThrusterPart { get; private set; }
    internal ThrusterPart? MainThrusterPart { get; private set; }


    //  Private static fields.
    private const float OXYGEN_DRAIN_SPEED = 1.5f;

    /* Hit-box */
    private static Vector2 s_bodyHitboxSize = new(30f, 28f);
    private static Vector2 s_headHitboxSize = new(s_bodyHitboxSize.X, 15f);
    private static Vector2 s_sideThrusterHitboxSize = new(11f, 17f);
    private static Vector2 s_mainThrusterHitboxSize = new(s_bodyHitboxSize.X - 1f, 20f);


    // Constructors.
    public ProbeEntity() : base(EntityType.Probe)
    {
        PhysicalEntityPart Base = CreateBodyPart(this);
        HeadPart = CreateHeadPart(this);
        LeftThrusterPart = CreateSideThrusterPart(this, false);
        RightThrusterPart = CreateSideThrusterPart(this, true);
        MainThrusterPart = CreateMainThrusterPart(this);

        // A lot of magic numbers here are just offsets which were not calculated but just eyed until it looked right.
        Base.LinkPart(HeadPart, new(0f, (-s_bodyHitboxSize.Y * 0.5f) - (s_headHitboxSize.Y * 0.5f) + 1.25f), 30_000f);
        Base.LinkPart(RightThrusterPart, new(s_bodyHitboxSize.X * 0.5f + s_sideThrusterHitboxSize.X * 0.5f - 2.5f, 0f), 50_000);
        Base.LinkPart(LeftThrusterPart, -RightThrusterPart.ParentLink!.LinkDistance, 50_000f);
        Base.LinkPart(MainThrusterPart, new(0f, s_bodyHitboxSize.Y * 0.5f + s_mainThrusterHitboxSize.X * 0.5f - 13.5f), 50_000);

        MainPart = Base;

        WeaponSlots = new WeaponSlot[] { new WeaponSlot(WeaponType.Light, Base, Vector2.Zero) };
        WeaponSlots[0].SetWeapon(new A420(null));

        ShipSystem = new ProbeSystem(this);
    }



    // Private static methods.
    private static PhysicalEntityPart CreateBodyPart(PhysicalEntity entity)
    {
        PhysicalEntityPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_bodyHitboxSize)},
            WorldMaterial.Test,
            entity);
        Part.AddSprite(new(GHGameAnimationName.Ship_Probe_Base) { Size = s_bodyHitboxSize });

        return Part;
    }

    private static PhysicalEntityPart CreateHeadPart(PhysicalEntity entity)
    {
        PhysicalEntityPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_headHitboxSize) },
            WorldMaterial.Test,
            entity);
        Part.AddSprite(new(GHGameAnimationName.Ship_Probe_Head) { Size = s_headHitboxSize });

        return Part;
    }

    private static ThrusterPart CreateSideThrusterPart(PhysicalEntity entity, bool isRightPart)
    {
        const float FUEL = 8_000_000f;

        ThrusterPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_sideThrusterHitboxSize) },
            WorldMaterial.Test, entity)
        {
            ThrusterPower = 14993f,
            ThrusterThrottleChangeSpeed = 9.8f,
            MaxFuel = FUEL,
            Fuel = FUEL,
            FuelUsageRate = 3.16f,
            ForceDirection = 0f
        };

        Part.AddSprite(new(GHGameAnimationName.Ship_Probe_SideThruster) 
        { 
            Size = s_sideThrusterHitboxSize, 
            Effects = isRightPart ? SpriteEffects.None : SpriteEffects.FlipHorizontally
        });

        return Part;
    }

    private static ThrusterPart CreateMainThrusterPart(PhysicalEntity entity)
    {
        const float FUEL = 17_000_000;

        ThrusterPart Part = new(new ICollisionBound[] { new RectangleCollisionBound(s_mainThrusterHitboxSize) },
            WorldMaterial.Test, entity)
        {
            ThrusterPower = 36443,
            ThrusterThrottleChangeSpeed = 5.8f,
            MaxFuel = FUEL,
            Fuel = FUEL,
            FuelUsageRate = 2.51f,
            ForceDirection = 0f,
            ThrustSound = GHGameSoundName.Ship_Probe_MainThrusterThrust,
            ThrustSoundVolume = 0.2f
        };

        Part.AddSprite(new(GHGameAnimationName.Ship_Probe_MainThruster) {Size = s_mainThrusterHitboxSize });

        return Part;
    }


    // Inherited methods.
    protected override void AITick(GHGameTime time)
    {
        throw new InvalidOperationException("A Probe cannot be AI ticked.");
    }

    protected override void PlayerTick(GHGameTime time)
    {
        base.PlayerTick(time);
        ShipSystem.TargetNavigationPosition = World!.Player.Camera.ToWorldPosition(UserInput.VirtualMousePosition.Current);
    }

    internal override Entity CreateClone()
    {
        throw new NotImplementedException();
    }

    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);

        if ((HeadPart == null) && (Oxygen > MIN_OXYGEN))
        {
            Oxygen -= time.WorldTime.TotalTimeSeconds * OXYGEN_DRAIN_SPEED;
        }
    }

    internal override void OnCollision(CollisionEventArgs args)
    {
        base.OnCollision(args);

        if ((args.Case.SelfPart.MaterialInstance.Stage != WorldMaterialStage.Destroyed) && (args.Case.SelfPart.Entity == this))
        {
            return;
        }

        if (args.Case.SelfPart == HeadPart)
        {
            HeadPart = null;
        }
        else if (args.Case.SelfPart == MainThrusterPart)
        {
            MainThrusterPart = null;
        }
        else if (args.Case.SelfPart == LeftThrusterPart)
        {
            LeftThrusterPart = null;
        }
        else if (args.Case.SelfPart == RightThrusterPart)
        {
            RightThrusterPart = null;
        }
    }
}