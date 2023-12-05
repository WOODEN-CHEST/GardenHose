using GardenHose.Game.World.Entities.Physical;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Material;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

    internal float FuelEfficiency { get; set; } = 1f; // Lower values indicate better efficiency, range is (0;inf)

    internal bool IsFuelUsed { get; set; } = false;

    internal float PotentialFuelTime => MaxFuel / (FuelEfficiency * ThrusterPower);

    internal float EstimatedFuelTimeLeft => Fuel / (FuelEfficiency * ThrusterPower);


    /* Events. */
    internal event EventHandler? OutOfFuel;


    // Private fields.
    private float _currentThrusterThrottle = 0f;
    private float _targetThrusterThrottle = 0f;
    private float _fuel = DEFALT_FUEL;
    private float _maxFuel = DEFALT_FUEL;

    private const float DEFALT_FUEL = 20_000_000f;


    // Constructors.
    internal ThrusterPart(ICollisionBound[]? bounds, WorldMaterial material, PhysicalEntity entity)
        : base(bounds, material, entity) { }

    internal ThrusterPart(WorldMaterial material, PhysicalEntity entity) : this(null, material, entity) { }


    // Internal methods.
    internal void Refuel() => Fuel = MaxFuel;


    // Protected methods.
    [TickedFunction(false)]
    protected void ThrusterTick()
    {
        if (IsThrusterOn)
        {
            float Step = MathF.Sign(TargetThrusterThrottle - CurrentThrusterThrottle);
            float ThrottleChange = Step * ThrusterThrottleChangeSpeed * Entity.World!.PassedTimeSeconds;

            if (Math.Abs(ThrottleChange) > Math.Abs(TargetThrusterThrottle - CurrentThrusterThrottle))
            {
                ThrottleChange = TargetThrusterThrottle - CurrentThrusterThrottle;
            }

            CurrentThrusterThrottle += ThrottleChange;
        }
        else
        {
            CurrentThrusterThrottle -= ThrusterThrottleChangeSpeed * Entity.World!.PassedTimeSeconds;
        }
        

        if ((CurrentThrusterThrottle != 0f) && (Fuel != 0f))
        {
            Vector2 ForceDirectionVector = Vector2.TransformNormal(Vector2.UnitX, Matrix.CreateRotationZ(CombinedRotation + ForceDirection));
            Entity.ApplyForce(ForceDirectionVector * CurrentThrusterThrottle * ThrusterPower * Entity.World!.PassedTimeSeconds, Position);

            if (!IsFuelUsed)
            {
                Fuel -= CurrentThrusterThrottle * ThrusterPower * FuelEfficiency * Entity.World.PassedTimeSeconds;
            }
        }
    }


    // Inherited methods.
    [TickedFunction(false)]
    internal override void ParallelTick()
    {
        base.ParallelTick();
        ThrusterTick();
    }
}