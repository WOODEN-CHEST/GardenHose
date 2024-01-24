using GardenHose.Game.World.Entities.Ship;
using GardenHoseEngine.IO;
using GardenHoseEngine.Screen;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GardenHose.Game.World.Player;

internal class PlayerCamera
{
    // Internal fields.
    internal WorldPlayer GamePlayer { get; private init; }

    internal float TargetZoom { get; private set; } = 1f;

    internal Vector2 TargetPosition { get; private set; }

    internal const float CAMERA_MOVEMENT_SPEED = 5f;
    internal const float MOUSE_MOVEMENT_OFFSET_FACTOR = 0.1f;
    internal const float MOTION_OFFSET_FACTOR = 0.5f;
    internal const float ROTATION_OFFSET_AMOUNT = 75f;
    internal const float ZOOM_SPEED = 3f;
    internal const float ZOOM_AMOUNT = 1.2f;
    internal const float MAX_ZOOM = 10f;
    internal const float MIN_ZOOM = 0.75f;
    internal const float MOUSE_ZOOM_FACTOR = 0.005f;


    // Private fields.
    private IInputListener _zoomInListener;
    private IInputListener _zoomOutListener;


    // Constructors.
    internal PlayerCamera(WorldPlayer player)
    {
        GamePlayer = player ?? throw new ArgumentNullException(nameof(player));

        _zoomInListener = MouseListenerCreator.Scroll(this, true, ScrollDirection.Up, OnPlayerZoomInEvent);
        _zoomOutListener = MouseListenerCreator.Scroll(this, true, ScrollDirection.Down, OnPlayerZoomOutEvent);
        UserInput.AddListener(_zoomInListener);
        UserInput.AddListener(_zoomOutListener);
    }


    // Internal methods.
    internal void Tick()
    {
        Vector2 MouseOffsetFromCenter = (UserInput.VirtualMousePosition.Current - (Display.VirtualSize * 0.5f));

        // Position.
        Vector2 MouseOffset = MouseOffsetFromCenter * MOUSE_MOVEMENT_OFFSET_FACTOR * GamePlayer.World.InverseZoom;
        Vector2 MotionOffset = GamePlayer.SpaceShip.Motion * MOTION_OFFSET_FACTOR * GamePlayer.World.InverseZoom;
        Vector2 FacingOffset = Vector2.Transform(-Vector2.UnitY, Matrix.CreateRotationZ(GamePlayer.SpaceShip.Rotation))
            * ROTATION_OFFSET_AMOUNT * GamePlayer.World.InverseZoom;

        TargetPosition = GamePlayer.SpaceShip.Position + MouseOffset + FacingOffset;
        GamePlayer.World.CameraCenter += (TargetPosition - GamePlayer.World.CameraCenter) * 
            Math.Min(GamePlayer.World.PassedTimeSeconds * CAMERA_MOVEMENT_SPEED, 1f);

        // Zoom.
        float MouseOffsetZoom = 1f - (MouseOffsetFromCenter.Length() / (Display.VirtualSize.Length() * 0.5f) * MOUSE_ZOOM_FACTOR);
        GamePlayer.World.Zoom = (GamePlayer.World.Zoom +
            Math.Min((TargetZoom - GamePlayer.World.Zoom) * GamePlayer.World.PassedTimeSeconds * ZOOM_SPEED, 1f))
            * MouseOffsetZoom; 
    }


    // Private methods.
    private void OnPlayerZoomInEvent(object? sender, EventArgs args)
    {
        TargetZoom = Math.Min(TargetZoom * ZOOM_AMOUNT, MAX_ZOOM);
    }

    private void OnPlayerZoomOutEvent(object? sender, EventArgs args)
    {
        TargetZoom = Math.Max(TargetZoom / ZOOM_AMOUNT, MIN_ZOOM);
    }
}