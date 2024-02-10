using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeSystem : ISpaceshipSystem
{
    // Fields.
    public bool IsEnabled { get; set; } = true;

    public SpaceshipEntity Ship => _probe;

    public bool IsVisible { get; set; } = true;

    public Effect? Shader { get; set; }


    // Internal fields.
    internal const float TARGET_ALTITUDE = 100f;
    internal const float SPEED_REDUCTION_ROLL = MathHelper.PiOver2;
    internal const float MAX_STATIONARY_SPEED = 3.5f;
    internal const float MAX_FALLING_SPEED = 30f;
    internal const float MAX_ANGULAR_MOTION = MathF.PI;
    internal const float MAX_FOLLOW_SPEED = 100f;
    internal const float MAX_NAVIGATION_POINT_DISTANCE_BEFORE_FOLLOW = 300f;
    internal const float FOLLOW_ROLL = MathHelper.PiOver4;



    internal float RollRelativeToGround
    {
        get
        {
            if (_probe.World!.Planet == null)
            {
                return 0f;
            }

            if (_probe.Position == _probe.World!.Planet.Position)
            {
                return 0f;
            }

            // There definitely exists a better way to do this, but after hours of failing I gave up. This is just how it's gonna be.
            float UpAngleAtPosition = MathF.Atan2(_probe.Position.X, -_probe.Position.Y);
            Vector2 Direction = Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(_probe.Rotation - UpAngleAtPosition));
            return MathF.Atan2(Direction.X, -Direction.Y);
        }
    }

    internal float AltitudeRelativeToGround
    {
        get
        {
            if (_probe.World!.Planet == null)
            {
                return float.PositiveInfinity;
            }

            return Vector2.Distance(_probe.Position, _probe.World!.Planet.Position) - _probe.World.Planet.Radius;
        }
    }

    internal ProbeAutopilotState AutopilotState { get; set; } = ProbeAutopilotState.FollowDirection;
    public Vector2 TargetNavigationPosition { get; set; }


    // Private fields.
    private ProbeEntity _probe;


    private ProbeDashboard _dashboard;
    private ProbeThrusterPanel _thrusterPanel;



    // Constructors.
    internal ProbeSystem(ProbeEntity probe)
    {
        _probe = probe ?? throw new ArgumentNullException(nameof(probe));
        _dashboard = new(this);
        _thrusterPanel = new(this);
    }


    // Private methods.
    private void ControlShipInSpin(float upwardsFactor)
    {
        _probe.MainThrusterPart?.SetTargetThrottle(upwardsFactor);
        _probe.LeftThrusterPart?.SetTargetThrottle(Ship.AngularMotion < 0f ? 1f : 0f);
        _probe.RightThrusterPart?.SetTargetThrottle(Ship.AngularMotion > 0f ? 1f : 0f);
    }

    private void MaintainAltitude(float altitudeInFuture, float rollInFuture, float upwardsFactor, float relativeYSpeed)
    {
        if (altitudeInFuture > TARGET_ALTITUDE)
        {
            float TargetThrottle = ((Ship.Motion.Length() > MAX_FALLING_SPEED)
                && (relativeYSpeed < 0f)) ? 1f : 0f;
            _probe.MainThrusterPart?.SetTargetThrottle(TargetThrottle);
            _probe.RightThrusterPart?.SetTargetThrottle(TargetThrottle);
            _probe.LeftThrusterPart?.SetTargetThrottle(TargetThrottle);
            return;
        }

        _probe.MainThrusterPart?.SetTargetThrottle(upwardsFactor);
        if (rollInFuture > 0f)
        {
            _probe.RightThrusterPart?.SetTargetThrottle(1f);
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
        }
        else if (rollInFuture < 0f)
        {
            _probe.RightThrusterPart?.SetTargetThrottle(0f);
            _probe.LeftThrusterPart?.SetTargetThrottle(1f);
        }

        return;
    }

    private void RemainStationary()
    {
        if (_probe.World!.Planet == null)
        {
            return;
        }

        // Bunch of calculations to get required data.
        float Altitude = AltitudeRelativeToGround;
        float Roll = RollRelativeToGround;
        float UpwardsFactor = (MathHelper.PiOver2 - Math.Abs(Roll)) / MathHelper.PiOver2;
        Vector2 SurfaceNormal = Ship.Position - Ship.World!.Planet!.Position;
        if (SurfaceNormal.LengthSquared() is 0f or -0f)
        {
            SurfaceNormal = Vector2.UnitX;
        }
        SurfaceNormal = Vector2.Normalize(SurfaceNormal);
        Vector2 Surface = GHMath.PerpVectorClockwise(SurfaceNormal);
        float RelativeXSpeed = Vector2.Dot(Surface, Ship.Motion);
        float RelativeYSpeed = Vector2.Dot(SurfaceNormal, Ship.Motion);
        float AltitudeOneSecondInFuture = Altitude + RelativeYSpeed;
        float RollOneSecondInFuture = Roll + _probe.AngularMotion;

        // Actual autopilot code.
        // First try to control the ship if it's in a spin.
        if (Math.Abs(Ship.AngularMotion) >= MAX_ANGULAR_MOTION)
        {
            ControlShipInSpin(UpwardsFactor);
            return;
        }

        // Then try to maintain altitude if stationary enough.
        if (Math.Abs(RelativeXSpeed) < MAX_STATIONARY_SPEED) 
        {
            MaintainAltitude(AltitudeOneSecondInFuture, RollOneSecondInFuture, UpwardsFactor, RelativeYSpeed);
            return;
        }

        // Otherwise try to reduce speed.
        ThrusterPart? RollThruster = RelativeXSpeed > 0f ? _probe.RightThrusterPart : _probe.LeftThrusterPart;
        ThrusterPart? RollReductionThruster = RelativeXSpeed > 0f ? _probe.LeftThrusterPart : _probe.RightThrusterPart;
        float TargetRollMultiplier = MathF.Min(MathF.Abs(RelativeXSpeed) / 125f, 1.75f);
        float TargetRoll = (RelativeXSpeed > 0f ? -SPEED_REDUCTION_ROLL : SPEED_REDUCTION_ROLL) * TargetRollMultiplier;
        float OffsetFromTargetRoll = (RollOneSecondInFuture - TargetRoll) * Math.Sign(TargetRoll);

        _probe.MainThrusterPart?.SetTargetThrottle(TARGET_ALTITUDE / AltitudeOneSecondInFuture);
        if (OffsetFromTargetRoll > 0f)
        {
            RollReductionThruster?.SetTargetThrottle(1f);
            RollThruster?.SetTargetThrottle(0f);
        }
        else
        {
            RollReductionThruster?.SetTargetThrottle(0f);
            RollThruster?.SetTargetThrottle(1f);
        }
    }

    private void FollowPoint(Vector2 point, Vector2 shipFixedLocation)
    {
        // Pre-calculate required stuff.
        float Altitude = AltitudeRelativeToGround;
        float Roll = RollRelativeToGround;
        float UpwardsFactor = (MathHelper.PiOver2 - Math.Abs(Roll)) / MathHelper.PiOver2;
        Vector2 SurfaceNormal = Ship.Position - Ship.World!.Planet!.Position;
        if (SurfaceNormal.LengthSquared() is 0f or -0f)
        {
            SurfaceNormal = Vector2.UnitX;
        }
        SurfaceNormal = Vector2.Normalize(SurfaceNormal);
        Vector2 Surface = GHMath.PerpVectorClockwise(SurfaceNormal);
        float RelativeXSpeed = Vector2.Dot(Surface, Ship.Motion);
        float RelativeYSpeed = Vector2.Dot(SurfaceNormal, Ship.Motion);
        float AltitudeOneSecondInFuture = Altitude + RelativeYSpeed;
        float RollOneSecondInFuture = Roll + _probe.AngularMotion;


        // Control spin.
        if (Math.Abs(Ship.AngularMotion) >= MAX_ANGULAR_MOTION)
        {
            ControlShipInSpin(UpwardsFactor);
            return;
        }

        // More math.
        float TargetRotation = MathF.Atan2(point.X, -point.Y);
        Vector2 RelativeFixedShipLocation = Vector2.Transform(shipFixedLocation, Matrix.CreateRotationZ(-TargetRotation));
        float RelativeShipRotation = MathF.Atan2(RelativeFixedShipLocation.X, -RelativeFixedShipLocation.Y);

        float TargetRoll = RelativeShipRotation > 0f ? -FOLLOW_ROLL : FOLLOW_ROLL;
        float OffsetFromTargetRoll = (RollOneSecondInFuture - TargetRoll) * Math.Sign(TargetRoll);
        bool IsMovingTowardsPoint = -Math.Sign(RelativeShipRotation) == Math.Sign(RelativeXSpeed);

        // Rest the ship if too high, moving towards the target and has the correct roll.
        if ((AltitudeOneSecondInFuture > TARGET_ALTITUDE)
            && (Roll <= FOLLOW_ROLL) && (IsMovingTowardsPoint))
        {
            _probe.MainThrusterPart?.SetTargetThrottle(0f);
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
            _probe.RightThrusterPart?.SetTargetThrottle(0f);
            return;
        } 
        
        // Try to reduce altitude (Maintain altitude also reduces it if too high)
        if ((Altitude > TARGET_ALTITUDE * 1.3f) && (Math.Abs(RelativeXSpeed) < 10f))
        {
            MaintainAltitude(AltitudeOneSecondInFuture, RollOneSecondInFuture, UpwardsFactor, RelativeYSpeed);
            return;
        }

        // Try to slow down if moving away from point.
        if (!IsMovingTowardsPoint)
        {
            //RemainStationary();
            TargetRoll *= 2.5f;
            OffsetFromTargetRoll = (RollOneSecondInFuture - TargetRoll) * Math.Sign(TargetRoll);
        }

        ThrusterPart? RollThruster = RelativeShipRotation > 0f ? _probe.RightThrusterPart : _probe.LeftThrusterPart;
        ThrusterPart? AntiRollThruster = RelativeShipRotation > 0f ? _probe.LeftThrusterPart : _probe.RightThrusterPart;

        _probe.MainThrusterPart?.SetTargetThrottle(AltitudeOneSecondInFuture <= TARGET_ALTITUDE || !IsMovingTowardsPoint ? 1f : 0f);
        if (OffsetFromTargetRoll < 0f)
        {
            RollThruster?.SetTargetThrottle(1f);
            AntiRollThruster?.SetTargetThrottle(0f);
        }
        else
        {
            RollThruster?.SetTargetThrottle(0f);
            AntiRollThruster?.SetTargetThrottle(1f);
        }
    }

    private void HandleManualThrusterInput()
    {
        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.Up))
        {
            _thrusterPanel.SetAutopilotState(ProbeAutopilotState.Disabled);
            _probe.MainThrusterPart?.SetTargetThrottle(1f);
        }
        else if (AutopilotState == ProbeAutopilotState.Disabled)
        {
            _probe.MainThrusterPart?.SetTargetThrottle(0f);
        }

        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.Left))
        {
            _thrusterPanel.SetAutopilotState(ProbeAutopilotState.Disabled);
            _probe.LeftThrusterPart?.SetTargetThrottle(1f);
        }
        else if (AutopilotState == ProbeAutopilotState.Disabled)
        {
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
        }

        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.Right))
        {
            _thrusterPanel.SetAutopilotState(ProbeAutopilotState.Disabled);
            _probe.RightThrusterPart?.SetTargetThrottle(1f);
        }
        else if (AutopilotState == ProbeAutopilotState.Disabled)
        {
            _probe.RightThrusterPart?.SetTargetThrottle(0f);
        }
    }


    // Inherited methods.
    /* Drawing. */
    public void Draw(IDrawInfo info)
    {
        if (!IsVisible) return;
            
        _dashboard.Draw(info);
        _thrusterPanel.Draw(info);
    }

    /* Navigating. */
    public void NavigateToPosition(Vector2 position)
    {
        Vector2 FixedShipLocation = Ship.Position - Ship.World!.Planet!.Position;
        if (FixedShipLocation.LengthSquared() is 0f or -0f)
        {
            FixedShipLocation = Vector2.UnitY;
        }
        FixedShipLocation = Vector2.Normalize(FixedShipLocation);
        FixedShipLocation = FixedShipLocation * Ship.World.Planet.Radius;

        if ((Vector2.Distance(position, FixedShipLocation) * Ship.World.Player.Camera.Zoom <= MAX_NAVIGATION_POINT_DISTANCE_BEFORE_FOLLOW) 
            || AutopilotState == ProbeAutopilotState.StayStationary)
        {
            RemainStationary();
        }
        else
        {
            FollowPoint(position, FixedShipLocation);
        }
    }

    /* Tick. */
    [TickedFunction(false)]
    public void Tick()
    {
        HandleManualThrusterInput();

        return;
        if (AutopilotState == ProbeAutopilotState.Disabled)
        {
            return;
        }

        NavigateToPosition(TargetNavigationPosition);
    }

    public void ParallelTick(bool isPlayerTick)
    {
        
    }

    public void Load(GHGameAssetManager assetManager)
    {
        _dashboard.Load(assetManager);
        _thrusterPanel.Load(assetManager);
    }

    public void OnPilotChange(SpaceshipPilot newPilot)
    {
        _thrusterPanel.SetInputListeners(newPilot != SpaceshipPilot.Player);
    }
}