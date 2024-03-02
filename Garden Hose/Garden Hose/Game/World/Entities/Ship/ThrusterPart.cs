using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Particle;
using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using GardenHoseEngine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GardenHose.Game.World.Entities.Ship;

internal class ThrusterPart : PhysicalEntityPart
{
    // Internal static fields.
    internal static ParticleSettings FuelLeakParticle { get; } = new(WorldMaterial.Test, GHGameAnimationName.Particle_Fuel1,
        GHGameAnimationName.Particle_Fuel2, GHGameAnimationName.Particle_Fuel3, GHGameAnimationName.Particle_Fuel4)
    {
        LifetimeMin = 6f,
        LifetimeMax = 12f,
        SizeMin = new(4f),
        SizeMax = new(8f),
        ScaleChangePerSecondMin = 0f,
        ScaleChangePerSecondMax = 0.05f,
        CollisionRadius = 9f,
        RotationMin = -MathF.PI,
        RotationMax = MathF.PI,
        AngularMotionMin = -MathF.PI * 0.25f,
        AngularMotionMax = MathF.PI * 0.25f,
        FadeInTime = 0.1f,
        FadeOutTime = 3f
    };


    // Internal fields.
    /* Thrusters. */
    internal bool IsThrusterOn
    {
        get => _isThrusterOn;
        set
        {
            if  (value != _isThrusterOn)
            {
                _isThrusterOn = value;
                EngineSwitch?.Invoke(this, value);
            }
        }
    }

    internal float TargetThrusterThrottle
    {
        get => _targetThrusterThrottle;
        set
        {
            _targetThrusterThrottle = Math.Clamp(value, 0f, 1f);
        }
    }

    internal float CurrentThrusterThrottle
    {
        get => _currentThrusterThrottle;
        set
        {
            _currentThrusterThrottle = Math.Clamp(value, 0f, 1f);
        }
    }

    internal float ThrusterThrottleChangeSpeed { get; set; }
    internal float ThrusterPower { get; set; }
    internal float ForceDirection { get; set; } = 0f;
    internal override float Mass => base.Mass + Fuel * FUEL_MASS;
    internal const float FUEL_MASS = 0.000_01f;


    /* Fuel. */
    internal float Fuel
    {
        get => _fuel;
        set
        {
            _fuel = Math.Clamp(value, 0f, MaxFuel);
        }
    }

    internal float MaxFuel
    {
        get => _maxFuel;
        set
        {
            _maxFuel = value;
            _fuel = Math.Clamp(value, 0f, MaxFuel);
        }
    }

    internal float FuelUsageRate { get; set; } = 1f; // Lower values indicate better efficiency, range is (0;inf)
    internal bool IsFuelUsed { get; set; } = true;
    internal float PotentialFuelTime => MaxFuel / (FuelUsageRate * ThrusterPower);
    internal float EstimatedFuelTimeLeft => Fuel / (FuelUsageRate * ThrusterPower);
    internal bool IsLeaking => _fuelLeakLocations.Count > 0;


    /* Events. */
    internal event EventHandler? OutOfFuel;
    internal event EventHandler<bool>? EngineSwitch;


    // Private fields.
    private bool _isThrusterOn = true;

    private float _currentThrusterThrottle = 0f;
    private float _targetThrusterThrottle = 0f;
    private float _fuel = DEFALT_FUEL;
    private float _maxFuel = DEFALT_FUEL;

    private readonly List<FuelLeakLocation> _fuelLeakLocations = new();

    private const float DEFALT_FUEL = 20_000_000f;
    internal const float TIME_PER_LEAK = 0.2f;
    private const float FUEL_LOSS_PER_LEAK = 1_000_000 * TIME_PER_LEAK;


    // Constructors.
    internal ThrusterPart(ICollisionBound[] bounds, WorldMaterial material, PhysicalEntity entity)
        : base(bounds, material, entity) { }

    internal ThrusterPart(WorldMaterial material, PhysicalEntity? entity) : this(Array.Empty<ICollisionBound>(), material, entity) { }


    // Internal methods.
    internal void Refuel() => Fuel = MaxFuel;

    internal void SetTargetThrottle(float throttle) => TargetThrusterThrottle = throttle;


    // Protected methods.
    [TickedFunction(false)]
    protected void ThrusterTick(GHGameTime time)
    {
        if (IsThrusterOn)
        {
            float Step = MathF.Sign(TargetThrusterThrottle - CurrentThrusterThrottle);
            float ThrottleChange = Step * ThrusterThrottleChangeSpeed * time.WorldTime.PassedTimeSeconds;

            if (Math.Abs(ThrottleChange) > Math.Abs(TargetThrusterThrottle - CurrentThrusterThrottle))
            {
                ThrottleChange = TargetThrusterThrottle - CurrentThrusterThrottle;
            }

            CurrentThrusterThrottle += ThrottleChange;
        }
        else
        {
            CurrentThrusterThrottle -= ThrusterThrottleChangeSpeed * time.WorldTime.PassedTimeSeconds;
        }
        

        if ((CurrentThrusterThrottle != 0f) && (Fuel != 0f))
        {
            Vector2 ForceDirectionVector = Vector2.TransformNormal(-Vector2.UnitY, Matrix.CreateRotationZ(CombinedRotation + ForceDirection));
            Entity!.ApplyForce(ForceDirectionVector * CurrentThrusterThrottle * ThrusterPower * time.WorldTime.PassedTimeSeconds, Position);

            if (IsFuelUsed)
            {
                Fuel -= CurrentThrusterThrottle * ThrusterPower * FuelUsageRate * time.WorldTime.PassedTimeSeconds;
            }
        }
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);
        ThrusterTick(time);


        if (Fuel <= 0f)
        {
            return;
        }

        foreach (FuelLeakLocation LeakLocation in _fuelLeakLocations)
        {
            LeakLocation.TimeSinceLastLeak += time.WorldTime.PassedTimeSeconds;
            if (LeakLocation.TimeSinceLastLeak >= TIME_PER_LEAK)
            {
                Fuel -= FUEL_LOSS_PER_LEAK;
                LeakLocation.TimeSinceLastLeak = 0f;
                ParticleEntity.CreateParticles(Entity!.World!, FuelLeakParticle, new Range(1, 1), 
                    LeakLocation.GetLocation(this), Entity.Motion - GHMath.NormalizeOrDefault(Entity.Motion) * 3f, 0.2f,
                    MathHelper.PiOver4, Entity);
            }
        }
    }

    internal override PhysicalEntityPart CloneDataToObject(PhysicalEntityPart part, PhysicalEntity? parentEntity)
    {
        base.CloneDataToObject(part, parentEntity);

        ThrusterPart Thruster = (ThrusterPart)part;
        Thruster.IsThrusterOn = IsThrusterOn;
        Thruster.TargetThrusterThrottle = TargetThrusterThrottle;
        Thruster.CurrentThrusterThrottle = CurrentThrusterThrottle;
        Thruster.ThrusterThrottleChangeSpeed = ThrusterThrottleChangeSpeed;
        Thruster.ThrusterPower = ThrusterPower;
        Thruster.ForceDirection = ForceDirection;
        Thruster.Fuel = Fuel;
        Thruster.MaxFuel = MaxFuel;
        Thruster.FuelUsageRate = FuelUsageRate;
        Thruster.IsFuelUsed = IsFuelUsed;

        return part;
    }

    internal override PhysicalEntityPart CreateClone(PhysicalEntity? parentEntity)
    {
        return CloneDataToObject(new ThrusterPart(MaterialInstance.Material, parentEntity), parentEntity);
    }

    internal override void OnCollision(CollisionEventArgs args)
    {
        base.OnCollision(args);

        if (args.ForceApplied < args.Case.SelfPart.MaterialInstance.Material.Resistance)
        {
            return;
        } 

        if (Random.Shared.NextSingle() <= (args.ForceApplied / args.Case.SelfPart.MaterialInstance.Material.Strength))
        {
            _fuelLeakLocations.Add(new(this, args.Case.AverageCollisionPoint));
        }
    }
}