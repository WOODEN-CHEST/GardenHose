using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;

namespace GardenHose.Game.World.Entities.Ship;

internal class ThrusterPart : PhysicalEntityPart
{
    // Internal fields.
    /* Thrusters. */
    internal bool IsThrusterOn { get; set; } = true;

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

    internal float ThrusterThrottleChangeSpeed { get; init; }
    internal float ThrusterPower { get; init; }
    internal float ForceDirection { get; set; } = 0f;


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

    internal float FuleUsageRate { get; set; } = 1f; // Lower values indicate better efficiency, range is (0;inf)
    internal bool IsFuelUsed { get; set; } = true;
    internal float PotentialFuelTime => MaxFuel / (FuleUsageRate * ThrusterPower);
    internal float EstimatedFuelTimeLeft => Fuel / (FuleUsageRate * ThrusterPower);


    /* Events. */
    internal event EventHandler? OutOfFuel;


    // Private fields.
    private float _currentThrusterThrottle = 0f;
    private float _targetThrusterThrottle = 0f;
    private float _fuel = DEFALT_FUEL;
    private float _maxFuel = DEFALT_FUEL;

    private const float DEFALT_FUEL = 20_000_000f;


    // Constructors.
    internal ThrusterPart(ICollisionBound[] bounds, WorldMaterial material, PhysicalEntity entity)
        : base(bounds, material, entity) { }

    internal ThrusterPart(WorldMaterial material, PhysicalEntity entity) : this(Array.Empty<ICollisionBound>(), material, entity) { }


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

            if (!IsFuelUsed)
            {
                Fuel -= CurrentThrusterThrottle * ThrusterPower * FuleUsageRate * time.WorldTime.PassedTimeSeconds;
            }
        }
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void Tick(GHGameTime time)
    {
        base.Tick(time);
        ThrusterTick(time);
    }
}