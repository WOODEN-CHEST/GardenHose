using GardenHose.Game.GameAssetManager;
using GardenHose.Game.World.Entities.Physical.Collision;
using GardenHose.Game.World.Entities.Ship;
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
    internal const float ALTITUDE_BUFFER = 50f;
    internal const float MAX_MAINTAIN_ALTITUDE_ROLL = 0.1f;
    internal const float MAX_ALTITUDE = 1000f;
    internal const float SPEED_REDUCTION_ROLL = MathHelper.PiOver2;
    internal const float MAX_STATIONARY_SPEED = 3.5f;
    internal const float MAX_FALLING_SPEED = -30f;
    internal const float MAX_ANGULAR_MOTION = MathF.PI;
    internal const float MAX_FOLLOW_SPEED = 100f;
    internal const float MAX_NAVIGATION_POINT_DISTANCE_BEFORE_FOLLOW = 300f;
    internal const float FOLLOW_ROLL = MathHelper.PiOver4;

    internal ProbeAutopilotState AutopilotState { get; set; } = ProbeAutopilotState.FollowDirection;
    public Vector2 TargetNavigationPosition { get; set; }
    internal float TargetAltitude
    {
        get
        {
            if (Ship.World!.Planet == null)
            {
                return 0f;
            }

            return Ship.World!.Planet!.CollisionHandler.BoundingRadius - Ship.World.Planet.Radius + ALTITUDE_BUFFER;
        }
    }
    


    // Private fields.
    private readonly ProbeEntity _probe;

    private readonly ProbeErrorHandler _errorHandler;
    private readonly ProbeRollPanel _rollPanel;
    private readonly ProbeMeter _speedometer;
    private readonly ProbeMeter _altimeter;
    private readonly ProbeThrusterPanel _thrusterPanel;
    private readonly ProbePowerButton _powerButton;
    private readonly ProbeAutopilotSwitch _autopilotSwitch;
    private readonly ProbeFuelGauge _fuelGauge;

    private float _systemStartupTimer = STARTUP_TIME;

    private const float ERROR_HANDLER_TIME = 0.52f;
    private const float ROLL_PANEL_TIME = 0.63f;
    private const float SPEEDOMETER_TIME = 0.80f;
    private const float ALTIMETER_TIME = 0.97f;
    private const float THRUSTER_PANEL_TIME = 1.87f;
    private const float FUEL_GAUGE_TIME = 2.21f;
    private const float STARTUP_TIME = 3.4f;


    // Constructors.
    internal ProbeSystem(ProbeEntity probe)
    {
        _probe = probe ?? throw new ArgumentNullException(nameof(probe));
        _errorHandler = new ProbeErrorHandler();
        _rollPanel = new ProbeRollPanel();
        _speedometer = new(0f, 90f, GHGameAnimationName.Ship_Probe_MeterMarkingS, (probe) => probe.Motion.Length() * 0.25f);
        _altimeter = new(0f, 450f, GHGameAnimationName.Ship_Probe_MeterMarkingA, (probe) => probe.CommonMath.Altitude);
        _powerButton = new(IsPowered ? ProbeSystemState.On : ProbeSystemState.Off);
        _autopilotSwitch = new(AutopilotState);
        _fuelGauge = new(1f);


        _thrusterPanel = new(probe.LeftThrusterPart!.IsThrusterOn, probe.MainThrusterPart!.IsThrusterOn, probe.RightThrusterPart!.IsThrusterOn);
        _thrusterPanel.EngineSwitch += OnEngineButtonSwitchEvent;
        _probe.CollisionHandler.PartDamage += OnProbeDamageEvent;
        _probe.LeftThrusterPart!.EngineSwitch += OnEngineSwitchEvent;
        _probe.MainThrusterPart!.EngineSwitch += OnEngineSwitchEvent;
        _probe.RightThrusterPart!.EngineSwitch += OnEngineSwitchEvent;
        _autopilotSwitch.AutopilotChange += OnAutopilotSwitch;
    }


    // Private methods.


    /* Flight controls. */
    /* The autopilot has shitty code because lack of understanding of how AI should work, but it does the job. */
    private void AutopilotTick(Vector2 position)
    {
        if (_probe.World!.Planet == null)
        {
            return;
        }


        // Control spin or move towards planet.
        float UpwardsFactor = Math.Max((MathHelper.PiOver2 - Math.Abs(Ship.CommonMath.Roll)) / MathHelper.PiOver2, 0f);
        if (Math.Abs(Ship.AngularMotion) >= MAX_ANGULAR_MOTION)
        {
            ControlShipInSpin(UpwardsFactor);
            return;
        }
        else if (Ship.CommonMath.Altitude > Math.Max(MAX_ALTITUDE, TargetAltitude * 3f))
        {
            MoveTowardsPlanet(UpwardsFactor);
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

    private void ControlShipInSpin(float upwardsFactor)
    {
        _probe.MainThrusterPart?.SetTargetThrottle(upwardsFactor);
        _probe.LeftThrusterPart?.SetTargetThrottle(Ship.AngularMotion < 0f ? 1f : 0f);
        _probe.RightThrusterPart?.SetTargetThrottle(Ship.AngularMotion > 0f ? 1f : 0f);
    }

    private void ReachTargetRoll(float upwardsFactor, float targetRoll)
    {
        ThrusterPart? RollThruster = Ship.CommonMath.PlanetRelativeXSpeed > 0f ? _probe.RightThrusterPart : _probe.LeftThrusterPart;
        ThrusterPart? RollReductionThruster = Ship.CommonMath.PlanetRelativeXSpeed > 0f ? _probe.LeftThrusterPart : _probe.RightThrusterPart;

        float OffsetFromTargetRoll = (Ship.CommonMath.RollOneSecInFuture - targetRoll) * Math.Sign(targetRoll);
        bool UnderAltitude = Ship.CommonMath.AltitudeOneSecInFuture < TargetAltitude;
        float UnderAltitudeFactor = (UnderAltitude ? 1f : 0f) * upwardsFactor;

        _probe.MainThrusterPart?.SetTargetThrottle(UnderAltitude || 
            Ship.CommonMath.PlanetRelativeYSpeed < MAX_FALLING_SPEED ? 1f : (1f - upwardsFactor));
        if (OffsetFromTargetRoll > 0f)
        {
            RollReductionThruster?.SetTargetThrottle(1f);
            RollThruster?.SetTargetThrottle(UnderAltitudeFactor);
        }
        else
        {
            RollReductionThruster?.SetTargetThrottle(UnderAltitudeFactor);
            RollThruster?.SetTargetThrottle(1f);
        }
    }

    private void MaintainAltitude(float upwardsFactor)
    {
        if ((Ship.CommonMath.AltitudeOneSecInFuture < TargetAltitude) || (Ship.CommonMath.PlanetRelativeYSpeed <= MAX_FALLING_SPEED))
        {
            _probe.MainThrusterPart?.SetTargetThrottle(upwardsFactor);

            if (MathF.Abs(_probe.CommonMath.Roll) > MAX_MAINTAIN_ALTITUDE_ROLL)
            {
                _probe.RightThrusterPart?.SetTargetThrottle(_probe.CommonMath.RollOneSecInFuture > 0f ? 1f : 0f);
                _probe.LeftThrusterPart?.SetTargetThrottle(_probe.CommonMath.RollOneSecInFuture > 0f ? 0f : 1f);
            }
            else
            {
                _probe.RightThrusterPart?.SetTargetThrottle(1f);
                _probe.LeftThrusterPart?.SetTargetThrottle(1f);
            }
        }
        else
        {
            _probe.MainThrusterPart?.SetTargetThrottle(0f);
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
            _probe.RightThrusterPart?.SetTargetThrottle(0f);

            if (MathF.Abs(_probe.CommonMath.Roll) > MAX_MAINTAIN_ALTITUDE_ROLL)
            {
                _probe.RightThrusterPart?.SetTargetThrottle(_probe.CommonMath.RollOneSecInFuture > 0f ? 1f : 0f);
                _probe.LeftThrusterPart?.SetTargetThrottle(_probe.CommonMath.RollOneSecInFuture > 0f ? 0f : 1f);
            }
        }
    }

    private void MoveTowardsPlanet(float upwardsFactor)
    {
        if (Ship.CommonMath.PlanetRelativeYSpeed < MAX_FALLING_SPEED)
        {
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
            _probe.MainThrusterPart?.SetTargetThrottle(0f);
            _probe.RightThrusterPart?.SetTargetThrottle(0f);
            return;
        }

        float OffsetFromTargetRoll = (MathF.PI * Math.Sign(Ship.CommonMath.RollOneSecInFuture)) - Ship.CommonMath.RollOneSecInFuture;

        _probe.MainThrusterPart?.SetTargetThrottle(1f - upwardsFactor);
        _probe.LeftThrusterPart?.SetTargetThrottle((OffsetFromTargetRoll > 0f
            && Ship.AngularMotion < 1f) ? 1f : 0f);
        _probe.RightThrusterPart?.SetTargetThrottle((OffsetFromTargetRoll < 0f
             && Ship.AngularMotion > -1f) ? 1f : 0f);
    }

    private void RemainStationary(float upwardsFactor)
    {
        // Try to maintain target altitude if stationary enough.
        if (Math.Abs(Ship.CommonMath.PlanetRelativeXSpeed) < MAX_STATIONARY_SPEED)
        {
            MaintainAltitude(upwardsFactor);
            return;
        }

        // Otherwise try to reduce speed.
        float TargetRollMultiplier = MathF.Min(MathF.Abs(Ship.CommonMath.PlanetRelativeXSpeed) / 125f, 1f);
        float TargetRoll = (Ship.CommonMath.PlanetRelativeXSpeed > 0f ? -SPEED_REDUCTION_ROLL : SPEED_REDUCTION_ROLL) * TargetRollMultiplier;
        ReachTargetRoll(upwardsFactor, TargetRoll);
    }

    private void FollowPoint(Vector2 point, Vector2 shipFixedLocation, float upwardsFactor)
    {
        //// More math.
        //float TargetRotation = MathF.Atan2(point.X, -point.Y);
        //Vector2 RelativeFixedShipLocation = Vector2.Transform(shipFixedLocation, Matrix.CreateRotationZ(-TargetRotation));
        //float RelativeShipRotation = MathF.Atan2(RelativeFixedShipLocation.X, -RelativeFixedShipLocation.Y);

        //float TargetRoll = RelativeShipRotation > 0f ? -FOLLOW_ROLL : FOLLOW_ROLL;
        //float OffsetFromTargetRoll = (Ship.CommonMath.RollOneSecInFuture - TargetRoll) * Math.Sign(TargetRoll);
        //bool IsMovingTowardsPoint = -Math.Sign(RelativeShipRotation) == Math.Sign(Ship.CommonMath.PlanetRelativeXSpeed);

        //// Rest the ship if too high, moving towards the target and has the correct roll.
        //if ((Ship.CommonMath.AltitudeOneSecInFuture > TARGET_ALTITUDE)
        //    && (Ship.CommonMath.Roll <= FOLLOW_ROLL) && (IsMovingTowardsPoint))
        //{
        //    _probe.MainThrusterPart?.SetTargetThrottle(0f);
        //    _probe.LeftThrusterPart?.SetTargetThrottle(0f);
        //    _probe.RightThrusterPart?.SetTargetThrottle(0f);
        //    return;
        //} 
        
        //// Try to reduce altitude (Maintain altitude also reduces it if too high)
        //if ((Ship.CommonMath.Altitude > TARGET_ALTITUDE * 1.3f) && (Math.Abs(Ship.CommonMath.PlanetRelativeXSpeed) < 10f))
        //{
        //    MaintainAltitude(upwardsFactor);
        //    return;
        //}

        //// Try to slow down if moving away from point.
        //if (!IsMovingTowardsPoint)
        //{
        //    //RemainStationary();
        //    TargetRoll *= 2.5f;
        //    OffsetFromTargetRoll = (Ship.CommonMath.RollOneSecInFuture - TargetRoll) * Math.Sign(TargetRoll);
        //}

        //ThrusterPart? RollThruster = RelativeShipRotation > 0f ? _probe.RightThrusterPart : _probe.LeftThrusterPart;
        //ThrusterPart? AntiRollThruster = RelativeShipRotation > 0f ? _probe.LeftThrusterPart : _probe.RightThrusterPart;

        //_probe.MainThrusterPart?.SetTargetThrottle(Ship.CommonMath.AltitudeOneSecInFuture <= TARGET_ALTITUDE || !IsMovingTowardsPoint ? 1f : 0f);
        //if (OffsetFromTargetRoll < 0f)
        //{
        //    RollThruster?.SetTargetThrottle(1f);
        //    AntiRollThruster?.SetTargetThrottle(0f);
        //}
        //else
        //{
        //    RollThruster?.SetTargetThrottle(0f);
        //    AntiRollThruster?.SetTargetThrottle(1f);
        //}
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


    /* Event handlers. */
    private void OnProbeDamageEvent(object? sender, CollisionEventArgs args)
    {
        if (args.Case.SelfPart.Entity == _probe)
        {
            return;
        }

        if ((args.Case.SelfPart == _probe.LeftThrusterPart) || (args.Case.SelfPart == _probe.MainThrusterPart)
            || (args.Case.SelfPart == _probe.RightThrusterPart))
        {
            ((ThrusterPart)args.Case.SelfPart).EngineSwitch -= OnEngineSwitchEvent;
        }
    }

    private void OnEngineSwitchEvent(object? sender, bool isOn)
    {
        if (sender == _probe.LeftThrusterPart)
        {
            _thrusterPanel.SetLeftThrusterPanelState(isOn);
        }
        else if (sender == _probe.MainThrusterPart)
        {
            _thrusterPanel.SetMainThrusterPanelState(isOn);
        }
        else
        {
            _thrusterPanel.SetRightThrusterPanelState(isOn);
        }
    }

    private void OnEngineButtonSwitchEvent(object? sender, EngineButtonSwitchEventArgs args)
    {
        switch (args.Thruster)
        {
            case ProbeThruster.LeftThruster:
                if (_probe.LeftThrusterPart != null)
                {
                    _probe.LeftThrusterPart.IsThrusterOn = args.IsOn;
                }
                break;

            case ProbeThruster.MainThruster:
                if (_probe.MainThrusterPart != null)
                {
                    _probe.MainThrusterPart.IsThrusterOn = args.IsOn;
                }
                break;

            case ProbeThruster.RightThruster:
                if (_probe.RightThrusterPart != null)
                {
                    _probe.RightThrusterPart.IsThrusterOn = args.IsOn;
                }
                break;
        }
    }

    private void OnPowerButtonPress(object? sender, EventArgs args)
    {
        IsPowered = !IsPowered;
        if (!IsPowered)
        {
            _systemStartupTimer = 0f;
            _probe.LeftThrusterPart?.SetTargetThrottle(0f);
            _probe.MainThrusterPart?.SetTargetThrottle(0f);
            _probe.RightThrusterPart?.SetTargetThrottle(0f);
        }
    }

    private void OnAutopilotSwitch(object? sender, ProbeAutopilotState state)
    {
        AutopilotState = state;
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
        _thrusterPanel.Draw(info);
        _powerButton.Draw(info);
        _autopilotSwitch.Draw(info);
        _fuelGauge.Draw(info);
    }


    /* Tick. */
    [TickedFunction(false)]
    public void Tick(GHGameTime time)
    {
        _powerButton.SystemState = IsPowered ?
            (_systemStartupTimer < STARTUP_TIME ? ProbeSystemState.StartingUp : ProbeSystemState.On) : ProbeSystemState.Off;
        _powerButton.Tick(time, _probe, true);

        _rollPanel.Tick(time, _probe, _systemStartupTimer > ROLL_PANEL_TIME);
        _errorHandler.Tick(time, _probe, _systemStartupTimer > ERROR_HANDLER_TIME);
        _speedometer.Tick(time, _probe, _systemStartupTimer > SPEEDOMETER_TIME);
        _altimeter.Tick(time, _probe, _systemStartupTimer > ALTIMETER_TIME);
        _thrusterPanel.Tick(time, _probe, _systemStartupTimer > THRUSTER_PANEL_TIME);
        _fuelGauge.Tick(time, _probe, _systemStartupTimer > FUEL_GAUGE_TIME);

        _autopilotSwitch.Tick(time, _probe, true);
        _autopilotSwitch.State = AutopilotState;

        if ((_systemStartupTimer < STARTUP_TIME) && (IsPowered))
        {
            _systemStartupTimer = Math.Clamp(_systemStartupTimer + time.WorldTime.PassedTimeSeconds, 0f, STARTUP_TIME);
        }

        if (_systemStartupTimer >= STARTUP_TIME)
        {
            if (Ship.Pilot == SpaceshipPilot.Player)
            {
                HandleManualThrusterInput();
            }
            if (AutopilotState != ProbeAutopilotState.Disabled)
            {
                AutopilotTick(TargetNavigationPosition);
            }
        }
    }

    public void Load(GHGameAssetManager assetManager)
    {
        _errorHandler.Load(assetManager);
        _rollPanel.Load(assetManager);
        _speedometer.Load(assetManager);
        _altimeter.Load(assetManager);
        _thrusterPanel.Load(assetManager);
        _powerButton.Load(assetManager);
        _autopilotSwitch.Load(assetManager);
        _fuelGauge.Load(assetManager);

        Vector2 PaddingX = new(10f, 0f);
        Vector2 PaddingY = new(0f, 10f);

        _speedometer.Position = Display.VirtualSize - (ProbeMeter.PANEL_SIZE * 0.5f) - PaddingX - PaddingY;
        _altimeter.Position = _speedometer.Position - (ProbeMeter.PANEL_SIZE * new Vector2(1f, 0f)) - PaddingX;

        _errorHandler.Position = new Vector2(_altimeter.Position.X - (ProbeMeter.PANEL_SIZE.X * 0.5f) - (ProbeErrorHandler.PANEL_SIZE.X * 0.5f),
            Display.VirtualSize.Y - (ProbeErrorHandler.PANEL_SIZE.Y * 0.5f)) - PaddingX - PaddingY;

        _rollPanel.Position = new Vector2(_altimeter.Position.X - (ProbeMeter.PANEL_SIZE.X * 0.5f) - (ProbeRollPanel.PANEL_SIZE.X * 0.5f), 
            _errorHandler.Position.Y - (ProbeErrorHandler.PANEL_SIZE.Y * 0.5f) - (ProbeRollPanel.PANEL_SIZE.Y * 0.5f)) - PaddingX - PaddingY;

        _thrusterPanel.Position = (Display.VirtualSize * new Vector2(0f, 1f)) + (ProbeThrusterPanel.PANEL_SIZE * new Vector2(0.5f, -0.5f))
            + PaddingX - PaddingY;

        _powerButton.Position = new Vector2(Display.VirtualSize.X - ProbePowerButton.PANEL_SIZE.X * 0.5f,
            _altimeter.Position.Y - ProbeMeter.PANEL_SIZE.Y * 0.5f  - ProbePowerButton.PANEL_SIZE.Y * 0.5f)
            - PaddingX - PaddingY;
        _powerButton.PowerSwitch += OnPowerButtonPress;

        _autopilotSwitch.Position = new Vector2(ProbeAutopilotSwitch.PANEL_SIZE.X * 0.5f, _thrusterPanel.Position.Y
            - (ProbeThrusterPanel.PANEL_SIZE.Y * 0.5f) - ProbeAutopilotSwitch.PANEL_SIZE.Y * 0.5f) - PaddingY + PaddingX;

        _fuelGauge.Position = new Vector2(_thrusterPanel.Position.X + (ProbeThrusterPanel.PANEL_SIZE.X * 0.5f)
            + (ProbeFuelGauge.PANEL_SIZE.X * 0.5f), Display.VirtualSize.Y - (ProbeFuelGauge.PANEL_SIZE.Y * 0.5f))
            + PaddingX - PaddingY;
    }

    public void OnPilotChange(SpaceshipPilot newPilot) { }
}