using GardenHose.Frames.Global;
using GardenHose.Game.AssetManager;
using GardenHose.Game.World.Entities.Ship;
using GardenHose.Game.World.Entities.Ship.System;
using GardenHoseEngine;
using GardenHoseEngine.Frame.Item.Text;
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
    public bool IsEnabled { get; set; } = true;

    public SpaceshipEntity Ship => _probe;

    public bool IsVisible { get; set; } = true;

    public Effect? Shader { get; set; }


    // Internal fields.
    internal const float TARGET_ALTITUDE = 100f;
    internal const float MAX_SPEED = 150f;
    internal const float MAX_ROLL = MathHelper.PiOver4;
    internal const float MAX_STATIONARY_SPEED = 5f;
    internal const float MAX_FALLING_SPEED = 30f;

    

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


    // Private fields.
    private ProbeEntity _probe;


    private SimpleTextBox _shipInfoText;
    private ProbeDashboard _dashboard;
    private ProbeThrusterPanel _thrusterPanel;



    // Constructors.
    internal ProbeSystem(ProbeEntity probe)
    {
        _probe = probe ?? throw new ArgumentNullException(nameof(probe));

        _shipInfoText = new(GlobalFrame.GeEichFont, "")
        {
            IsShadowEnabled = true,
            Mask = Color.White,
            Origin = Origin.BottomLeft,
            Scale = new Vector2(0.8f)
        };
        _shipInfoText.Position.Vector = new Vector2(20f, Display.VirtualSize.Y - 20f);
        _dashboard = new(this);
        _thrusterPanel = new(this);
    }


    // Private methods.
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

        float AltitudeOneSecondInFuture = Altitude +
            Vector2.Dot(_probe.Motion, Vector2.Normalize(_probe.Position - _probe.World!.Planet!.Position));
        float RollOneSecondInFuture = Roll + _probe.AngularMotion;

        // Actual autopilot code (bad as shit but does the job).

        // First try to control the ship if it's in a spin.
        if (Math.Abs(Ship.AngularMotion) >= MathF.PI)
        {
            _probe.MainThrusterPart?.SetTargetThrottle(UpwardsFactor);
            _probe.LeftThrusterPart?.SetTargetThrottle(Ship.AngularMotion < 0f ? 1f : 0f);
            _probe.RightThrusterPart?.SetTargetThrottle(Ship.AngularMotion > 0f ? 1f : 0f);
            return;
        }


        // Then try to maintain altitude if stationary enough.
        //if (Math.Abs(Roll) > ((Math.PI * 2f) / 3f))
        //{
        //    return; // Thrusters cannot move the ship backwards, doomed in this scenario so just give up and return.
        //}

        if ((Math.Abs(RelativeXSpeed) < MAX_STATIONARY_SPEED) || (Altitude > TARGET_ALTITUDE * 4f)) 
        {
            if (Altitude > TARGET_ALTITUDE)
            {
                float TargetThrottle = ((Ship.Motion.Length() > MAX_FALLING_SPEED) 
                    && (RelativeYSpeed < 0f)) ? 1f : 0f;
                _probe.MainThrusterPart?.SetTargetThrottle(TargetThrottle);
                _probe.RightThrusterPart?.SetTargetThrottle(TargetThrottle);
                _probe.LeftThrusterPart?.SetTargetThrottle(TargetThrottle);
                return;
            }

            _probe.MainThrusterPart?.SetTargetThrottle(UpwardsFactor);
            if (RollOneSecondInFuture > 0f)
            {
                _probe.RightThrusterPart?.SetTargetThrottle(1f);
                _probe.LeftThrusterPart?.SetTargetThrottle(0f);
            }
            else if (RollOneSecondInFuture < 0f)
            {
                _probe.RightThrusterPart?.SetTargetThrottle(0f);
                _probe.LeftThrusterPart?.SetTargetThrottle(1f);
            }

            return;
        }

        // Otherwise try to reduce speed.
        ThrusterPart? RollThruster = RelativeXSpeed > 0f ? _probe.RightThrusterPart : _probe.LeftThrusterPart;
        ThrusterPart? RollReductionThruster = RelativeXSpeed > 0f ? _probe.LeftThrusterPart : _probe.RightThrusterPart;
        float TargetRollMultiplier = Math.Min(1f, MathF.Abs(RelativeXSpeed) / 125f);
        float TargetRoll = (RelativeXSpeed > 0f ? -MAX_ROLL : MAX_ROLL) * TargetRollMultiplier;
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
    public void Draw()
    {
        if (!IsVisible) return;

        _shipInfoText.Text = $"Altitude: {(AltitudeRelativeToGround * 0.1f).ToString("0")} dp\n" +
            $"Speed: {(_probe.Motion.Length() * 0.1f).ToString("0")} dp/s\n\n";
            
        _shipInfoText.Draw();
        _dashboard.Draw();
        _thrusterPanel.Draw();
    }

    /* Navigating. */
    public void NavigateToPosition(Vector2 position)
    {
        RemainStationary();
    }

    /* Tick. */
    [TickedFunction(false)]
    public void ParallelTick(bool isPlayerTick)
    {
        HandleManualThrusterInput();
        if (AutopilotState == ProbeAutopilotState.Disabled)
        {
            return;
        }

        if (_probe.World!.Planet != null && isPlayerTick)
        {
            Vector2 TargetPosition = _probe.World!.ToWorldPosition(UserInput.VirtualMousePosition.Current) - _probe.World!.Planet!.Position;
            if (TargetPosition.LengthSquared() == 0)
            {
                TargetPosition = _probe.Position;
            }
            Vector2 NormalizedDirection = Vector2.Normalize(TargetPosition);
            TargetPosition = (NormalizedDirection * _probe.World!.Planet!.Radius) + (NormalizedDirection * TARGET_ALTITUDE);

            NavigateToPosition(TargetPosition);
        }
    }

    [TickedFunction(false)]
    public void SequentialTick(bool isPlayerTick) { }

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