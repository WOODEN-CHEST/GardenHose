using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace GardenHose.Game.World.Entities.Probe;

internal class ProbeSystem : ISpaceshipSystem
{
    // Fields.
    public bool IsVisible { get; set; } = true;
    public Effect? Shader { get; set; }

    public bool IsEnabled { get; set; } = true;
    public bool IsPowered { get; set; } = true;
    public SpaceshipEntity Ship => _probe;
    


    // Internal fields.
    internal const float TARGET_ALTITUDE = 100f;
    internal const float SPEED_REDUCTION_ROLL = MathHelper.PiOver2;
    internal const float MAX_STATIONARY_SPEED = 3.5f;
    internal const float MAX_FALLING_SPEED = 30f;
    internal const float MAX_ANGULAR_MOTION = MathF.PI;
    internal const float MAX_FOLLOW_SPEED = 100f;
    internal const float MAX_NAVIGATION_POINT_DISTANCE_BEFORE_FOLLOW = 300f;
    internal const float FOLLOW_ROLL = MathHelper.PiOver4;


    internal ProbeAutopilotState AutopilotState { get; set; } = ProbeAutopilotState.FollowDirection;
    public Vector2 TargetNavigationPosition { get; set; }


    // Private fields.
    private readonly ProbeEntity _probe;

    private readonly ProbeErrorHandler _errorHandler;
    private readonly ProbeRollPanel _rollPanel;
    private readonly ProbeMeter _speedometer;
    private readonly ProbeMeter _altimeter;


    // Constructors.
    internal ProbeSystem(ProbeEntity probe)
    {
        _probe = probe ?? throw new ArgumentNullException(nameof(probe));
        _errorHandler = new ProbeErrorHandler();
        _rollPanel = new ProbeRollPanel();
        _speedometer = new(0f, 90f, GHGameAnimationName.Ship_Probe_MeterMarkingS, GHGameAnimationName.Ship_Probe_MeterDigitsS,
            (probe) => probe.Motion.Length() * 0.25f);
        _altimeter = new(0f, 450f, GHGameAnimationName.Ship_Probe_MeterMarkingA, GHGameAnimationName.Ship_Probe_MeterDigitsA,
            (probe) => probe.CommonMath.Altitude);
    }


    // Private methods.
    private void ControlShipInSpin(float upwardsFactor)
    {
        _probe.MainThrusterPart?.SetTargetThrottle(upwardsFactor);
        _probe.LeftThrusterPart?.SetTargetThrottle(Ship.AngularMotion < 0f ? 1f : 0f);
        _probe.RightThrusterPart?.SetTargetThrottle(Ship.AngularMotion > 0f ? 1f : 0f);
    }

    private void MaintainAltitude(float upwardsFactor)
    {
        if (Ship.CommonMath.AltitudeOneSecInFuture > TARGET_ALTITUDE)
        {
            float TargetThrottle = ((Ship.Motion.Length() > MAX_FALLING_SPEED)
                && (Ship.CommonMath.PlanetRelativeYSpeed < 0f)) ? 1f : 0f;
            _probe.MainThrusterPart?.SetTargetThrottle(TargetThrottle);
            _probe.RightThrusterPart?.SetTargetThrottle(TargetThrottle);
            _probe.LeftThrusterPart?.SetTargetThrottle(TargetThrottle);
            return;
        }

        _probe.MainThrusterPart?.SetTargetThrottle(upwardsFactor);
        if (Ship.CommonMath.RollOneSecInFuture > 0f)
        {
            _probe.RightThrusterPart?.SetTargetThrottle(1f);
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
        }
        else if (Ship.CommonMath.RollOneSecInFuture < 0f)
        {
            _probe.RightThrusterPart?.SetTargetThrottle(0f);
            _probe.LeftThrusterPart?.SetTargetThrottle(1f);
        }

        return;
    }

    private void RemainStationary(float upwardsFactor)
    {
        // First try to control the ship if it's in a spin.
        if (Math.Abs(Ship.AngularMotion) >= MAX_ANGULAR_MOTION)
        {
            ControlShipInSpin(upwardsFactor);
            return;
        }

        // Then try to maintain altitude if stationary enough.
        if (Math.Abs(Ship.CommonMath.PlanetRelativeXSpeed) < MAX_STATIONARY_SPEED) 
        {
            MaintainAltitude(upwardsFactor);
            return;
        }

        // Otherwise try to reduce speed.
        ThrusterPart? RollThruster = Ship.CommonMath.PlanetRelativeXSpeed > 0f ? _probe.RightThrusterPart : _probe.LeftThrusterPart;
        ThrusterPart? RollReductionThruster = Ship.CommonMath.PlanetRelativeXSpeed > 0f ? _probe.LeftThrusterPart : _probe.RightThrusterPart;
        float TargetRollMultiplier = MathF.Min(MathF.Abs(Ship.CommonMath.PlanetRelativeXSpeed) / 125f, 1.75f);
        float TargetRoll = (Ship.CommonMath.PlanetRelativeXSpeed > 0f ? -SPEED_REDUCTION_ROLL : SPEED_REDUCTION_ROLL) * TargetRollMultiplier;
        float OffsetFromTargetRoll = (Ship.CommonMath.RollOneSecInFuture - TargetRoll) * Math.Sign(TargetRoll);

        _probe.MainThrusterPart?.SetTargetThrottle(TARGET_ALTITUDE / Ship.CommonMath.AltitudeOneSecInFuture);
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

    private void FollowPoint(Vector2 point, Vector2 shipFixedLocation, float upwardsFactor)
    {
        // More math.
        float TargetRotation = MathF.Atan2(point.X, -point.Y);
        Vector2 RelativeFixedShipLocation = Vector2.Transform(shipFixedLocation, Matrix.CreateRotationZ(-TargetRotation));
        float RelativeShipRotation = MathF.Atan2(RelativeFixedShipLocation.X, -RelativeFixedShipLocation.Y);

        float TargetRoll = RelativeShipRotation > 0f ? -FOLLOW_ROLL : FOLLOW_ROLL;
        float OffsetFromTargetRoll = (Ship.CommonMath.RollOneSecInFuture - TargetRoll) * Math.Sign(TargetRoll);
        bool IsMovingTowardsPoint = -Math.Sign(RelativeShipRotation) == Math.Sign(Ship.CommonMath.PlanetRelativeXSpeed);

        // Rest the ship if too high, moving towards the target and has the correct roll.
        if ((Ship.CommonMath.AltitudeOneSecInFuture > TARGET_ALTITUDE)
            && (Ship.CommonMath.Roll <= FOLLOW_ROLL) && (IsMovingTowardsPoint))
        {
            _probe.MainThrusterPart?.SetTargetThrottle(0f);
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
            _probe.RightThrusterPart?.SetTargetThrottle(0f);
            return;
        } 
        
        // Try to reduce altitude (Maintain altitude also reduces it if too high)
        if ((Ship.CommonMath.Altitude > TARGET_ALTITUDE * 1.3f) && (Math.Abs(Ship.CommonMath.PlanetRelativeXSpeed) < 10f))
        {
            MaintainAltitude(upwardsFactor);
            return;
        }

        // Try to slow down if moving away from point.
        if (!IsMovingTowardsPoint)
        {
            //RemainStationary();
            TargetRoll *= 2.5f;
            OffsetFromTargetRoll = (Ship.CommonMath.RollOneSecInFuture - TargetRoll) * Math.Sign(TargetRoll);
        }

        ThrusterPart? RollThruster = RelativeShipRotation > 0f ? _probe.RightThrusterPart : _probe.LeftThrusterPart;
        ThrusterPart? AntiRollThruster = RelativeShipRotation > 0f ? _probe.LeftThrusterPart : _probe.RightThrusterPart;

        _probe.MainThrusterPart?.SetTargetThrottle(Ship.CommonMath.AltitudeOneSecInFuture <= TARGET_ALTITUDE || !IsMovingTowardsPoint ? 1f : 0f);
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
            AutopilotState = ProbeAutopilotState.Disabled;
            _probe.MainThrusterPart?.SetTargetThrottle(1f);
        }
        else if (AutopilotState == ProbeAutopilotState.Disabled)
        {
            _probe.MainThrusterPart?.SetTargetThrottle(0f);
        }

        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.Left))
        {
            AutopilotState = ProbeAutopilotState.Disabled;
            _probe.LeftThrusterPart?.SetTargetThrottle(1f);
        }
        else if (AutopilotState == ProbeAutopilotState.Disabled)
        {
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
        }

        if (UserInput.KeyboardState.Current.IsKeyDown(Keys.Right))
        {
            AutopilotState = ProbeAutopilotState.Disabled;
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

        _errorHandler.Draw(info);
        _rollPanel.Draw(info);
        _speedometer.Draw(info);
        _altimeter.Draw(info);
    }

    /* Navigating. */
    public void NavigateToPosition(Vector2 position)
    {
        if (_probe.World!.Planet == null)
        {
            return;
        }


        // Control spin.
        float UpwardsFactor = (MathHelper.PiOver2 - Math.Abs(Ship.CommonMath.Roll)) / MathHelper.PiOver2;
        if (Math.Abs(Ship.AngularMotion) >= MAX_ANGULAR_MOTION)
        {
            ControlShipInSpin(UpwardsFactor);
            return;
        }

        // Do target autopilot action.
        Vector2 PlanetRelativeTargetLocation = Ship.World!.Planet!.GetPositionAboveSurface(TargetNavigationPosition, Ship.CommonMath.Altitude);

        if ((AutopilotState == ProbeAutopilotState.StayStationary) ||
            (Vector2.Distance(position, PlanetRelativeTargetLocation)
            <= MAX_NAVIGATION_POINT_DISTANCE_BEFORE_FOLLOW))
        {
            RemainStationary(UpwardsFactor);
        }
        else
        {
            FollowPoint(position, PlanetRelativeTargetLocation, UpwardsFactor);
        }
    }

    /* Tick. */
    [TickedFunction(false)]
    public void Tick()
    {
        if (!IsPowered)
        {
            return;
        }

        _rollPanel.Tick(_probe);
        _errorHandler.Tick(_probe);
        _speedometer.Tick(_probe);
        _altimeter.Tick(_probe);

        if (Ship.Pilot ==  SpaceshipPilot.Player)
        {
            HandleManualThrusterInput();
        }

        if (AutopilotState != ProbeAutopilotState.Disabled)
        {
            NavigateToPosition(TargetNavigationPosition);
        }
    }

    public void Load(GHGameAssetManager assetManager)
    {
        _errorHandler.Load(assetManager);
        _rollPanel.Load(assetManager);
        _speedometer.Load(assetManager);
        _altimeter.Load(assetManager);

        Vector2 Padding = new(10f);
        _rollPanel.Position = Display.VirtualSize - (ProbeRollPanel.PANEL_SIZE * 0.5f) - Padding;
        _errorHandler.Position = Display.VirtualSize - (ProbeErrorHandler.PANEL_SIZE * 0.5f) - Padding * 2f
            - new Vector2(0f, ProbeRollPanel.PANEL_SIZE.Y);
        _speedometer.Position = _rollPanel.Position - (ProbeRollPanel.PANEL_SIZE * 0.5f) 
            - (ProbeMeter.PANEL_SIZE * 0.5f) - new Vector2(10f, 0f);
        _altimeter.Position = _speedometer.Position - ProbeMeter.PANEL_SIZE - new Vector2(10f, 0f);
    }

    public void OnPilotChange(SpaceshipPilot newPilot)
    {

    }
}